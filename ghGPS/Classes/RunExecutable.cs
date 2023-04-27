using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.Classes
{
    /// <summary>
    /// Input and Output for the EXE application
    /// </summary>
    public class RunResults

    {
        public RunResults()
        {

        }

        /// <summary>
        /// Get the value that the associated process specified when it terminated 
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Exceptional messages created during the process
        /// </summary>
        public Exception RunException { get; set; }

        /// <summary>
        /// Output of the process
        /// </summary>
        public Action<string> Output { get; set; }

        /// <summary>
        /// Errors that were created during the process
        /// </summary>
        public Action<string> Error { get; set; }

    }

    /// <summary>
    /// Program class library to run external app (.exe)
    /// </summary>
    public static class ExeProgram
    {


        [DllImport("User32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(IntPtr ZeroOnly, string IpWindowName);

        [DllImport("User32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Allows to open a exe file within a windows
        /// </summary>
        /// <param name="hWndChild">The child window to open</param>
        /// <param name="hWndNewParent">The parent container to host the exe file</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


        /// <summary>
        /// Run the specified program
        /// </summary>
        /// <param name="executablePath">Path to the file</param>
        /// <param name="arguments">Arguments to parse</param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public static RunResults Run(string executablePath, string arguments)
        {

            RunResults runResults = new RunResults();
            
            try
            {
                //Check if file exist
                if (File.Exists(executablePath))
                {

                    using (Process process = new Process())
                    {

                        process.StartInfo.FileName = executablePath;
                        //process.StartInfo.Arguments = arguments;
                        process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.CreateNoWindow = true; //not diplay a windows

                        // Start process
                        process.Start();
                        process.WaitForInputIdle();

                        using (ManualResetEvent mreOut = new ManualResetEvent(false), mreErr = new ManualResetEvent(false))
                        {
                            process.OutputDataReceived += (o, e) => { if (e.Data == null) mreOut.Set(); else runResults.Output(e.Data); };
                            process.BeginOutputReadLine();
                            process.ErrorDataReceived += (o, e) => { if (e.Data == null) mreErr.Set(); else runResults.Error(e.Data); };
                            process.BeginErrorReadLine();
                        }
                        
                        // Read one element asynchronously
                        process.BeginErrorReadLine();
                        //* Read the other one synchronously
                        process.BeginOutputReadLine();

                        process.WaitForExit();

                        runResults.ExitCode = process.ExitCode;

                    }

                }
                else
                {
                    throw new ArgumentException("Invalid executable path.", "executablePath");
                }

            }
            catch (Exception ex)
            {

                runResults.RunException = ex;

            }

            return runResults;

        }

        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the evironment's path.
        /// </summary>
        /// <param name="exe">The name of the executable file</param>
        /// <returns>The fully-qualified path to the file</returns>
        /// <exception cref="System.IO.FileNotFoundException">Raised when the exe was not found</exception>
        public static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }


        /// <summary>
        /// Quotes all arguments that contain whitespace, or begin with a quote and returns a single
        /// argument string for use with Process.Start().
        /// </summary>
        /// <param name="args">A list of strings for arguments, may not contain null, '\0', '\r', or '\n'</param>
        /// <returns>The combined list of escaped/quoted strings</returns>
        /// <exception cref="System.ArgumentNullException">Raised when one of the arguments is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Raised if an argument contains '\0', '\r', or '\n'</exception>
        public static string EscapeArguments(params string[] args)
        {
            StringBuilder arguments = new StringBuilder();
            Regex invalidChar = new Regex("[\x00\x0a\x0d]");//  these can not be escaped
            Regex needsQuotes = new Regex(@"\s|""");//          contains whitespace or two quote characters
            Regex escapeQuote = new Regex(@"(\\*)(""|$)");//    one or more '\' followed with a quote or end of string
            for (int carg = 0; args != null && carg < args.Length; carg++)
            {
                if (args[carg] == null) { throw new ArgumentNullException("args[" + carg + "]"); }
                if (invalidChar.IsMatch(args[carg])) { throw new ArgumentOutOfRangeException("args[" + carg + "]"); }
                if (args[carg] == String.Empty) { arguments.Append("\"\""); }
                else if (!needsQuotes.IsMatch(args[carg])) { arguments.Append(args[carg]); }
                else
                {
                    arguments.Append('"');
                    arguments.Append(escapeQuote.Replace(args[carg], m =>
                    m.Groups[1].Value + m.Groups[1].Value +
                    (m.Groups[2].Value == "\"" ? "\\\"" : "")
                    ));
                    arguments.Append('"');
                }
                if (carg + 1 < args.Length)
                    arguments.Append(' ');
            }
            return arguments.ToString();
        }

        /// <summary>
        /// Runs the specified executable with the provided arguments and returns the process' exit code.
        /// </summary>
        /// /// <param name="exe">The executable to run, may be unqualified or contain environment variables</param>
        /// <param name="output">Recieves the output of either std/err or std/out</param>
        /// <param name="input">Provides the line-by-line input that will be written to std/in, null for empty</param>

        /// <param name="args">The list of unescaped arguments to provide to the executable</param>
        /// <returns>Returns process' exit code after the program exits</returns>
        /// <exception cref="System.IO.FileNotFoundException">Raised when the exe was not found</exception>
        /// <exception cref="System.ArgumentNullException">Raised when one of the arguments is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Raised if an argument contains '\0', '\r', or '\n'
        public static RunResults Run(string exe, TextReader input, params string[] args)
        {
            RunResults runResults = new RunResults();
            
            try
            {
                if (String.IsNullOrEmpty(exe))
                    throw new FileNotFoundException();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.ErrorDialog = false;
                psi.WorkingDirectory = Environment.CurrentDirectory;
                psi.FileName = FindExePath(exe); //see http://csharptest.net/?p=526
                psi.Arguments = EscapeArguments(args); // see http://csharptest.net/?p=529

                using (Process process = Process.Start(psi))
                using (ManualResetEvent mreOut = new ManualResetEvent(false), mreErr = new ManualResetEvent(false))
                {
                    process.OutputDataReceived += (o, e) => { if (e.Data == null) mreOut.Set(); else runResults.Output(e.Data); };
                    process.BeginOutputReadLine();
                    process.ErrorDataReceived += (o, e) => { if (e.Data == null) mreErr.Set(); else runResults.Error(e.Data); };
                    process.BeginErrorReadLine();

                    string line;
                    while (input != null && null != (line = input.ReadLine()))
                    {
                        process.StandardInput.WriteLine(line);
                    }

                    process.StandardInput.Flush();
                    process.StandardInput.Close();
                    process.WaitForExit();

                    mreOut.WaitOne();
                    mreErr.WaitOne();
                    runResults.ExitCode = process.ExitCode;
                    
                }
            }
            catch (Exception ex)
            {

                runResults.RunException = ex;
            }

            return runResults;
        }
                
    }
}
