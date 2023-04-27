using ghGPS.Forms;
using MetroSuite;
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
    /// Class for Process Results
    /// </summary>
    public sealed class ProcessedResults : IDisposable
    {
        /// <summary>
        /// Run and obtain results from a process
        /// </summary>
        /// <param name="process">The process to start or run</param>
        /// <param name="processStartTime">Time the process started</param>
        /// <param name="standardOutput">Results from the process(s)</param>
        /// <param name="standardError">Errors generated during the processs</param>
        public ProcessedResults(Process process, string[] standardOutput, string[] standardError)
        {
            Process = process;
            ExitCode = process.ExitCode;
            RunTime = process.ExitTime - process.StartTime;
            StandardOutput = standardOutput;
            StandardError = standardError;            
        }

        public Process Process { get; }
        public int ExitCode { get; }
        public TimeSpan RunTime { get; }        
        public string[] StandardOutput { get; }
        public string[] StandardError { get; }       
        public void Dispose() { Process.Dispose(); }
    }

   
    public partial class ProcessEx
    {
       
        /// <summary>
        /// Run an executive asynchronously
        /// </summary>
        /// <param name="fileName">Filename or path to file to run</param>
        /// <returns></returns>
        public static Task<ProcessedResults> RunAsync(string fileName)
            => RunAsync(new ProcessStartInfo(FindExePath(fileName)));

        /// <summary>
        /// Run an executive asynchronously
        /// </summary>
        /// <param name="fileName">Filename or path to file to run</param>
        /// <param name="arguments">Arguments to parse and run</param>
        /// <returns></returns>
        public static Task<ProcessedResults> RunAsync(string fileName, string arguments)
            => RunAsync(new ProcessStartInfo(FindExePath(fileName), EscapeArguments(arguments)));

        /// <summary>
        /// Run an executive asynchronously
        /// </summary>
        /// <param name="filename">Filename or path to file to run</param>
        /// <param name="arguments">Arguments to parse and run</param>
        /// <param name="timeout">The time to wait for exit</param>
        /// <returns></returns>
        public static Task<ProcessedResults> RunAsync(string fileName, string arguments, TimeSpan timeout)
            => RunAsync(new ProcessStartInfo(FindExePath(fileName), EscapeArguments(arguments)), new CancellationTokenSource(timeout).Token);
                
    }

    public static partial class ProcessEx
    {
        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the environment's path.
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

        public static Task<ProcessedResults> RunAsync(ProcessStartInfo processStartInfo)
            => RunAsync(processStartInfo, CancellationToken.None);
                
        public static async Task<ProcessedResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
        {
            // force some settings in the start info so we can capture the output
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            var tcs = new TaskCompletionSource<ProcessedResults>();

            var standardOutput = new List<string>();
            var standardError = new List<string>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true                
            };
            
            var standardOutputResults = new TaskCompletionSource<string[]>();
            process.OutputDataReceived += (sender, args) => {
                if (args.Data != null)
                    standardOutput.Add(args.Data);
                else
                    standardOutputResults.SetResult(standardOutput.ToArray());
            };

            var standardErrorResults = new TaskCompletionSource<string[]>();
            process.ErrorDataReceived += (sender, args) => {
                if (args.Data != null)
                    standardError.Add(args.Data);
                else
                    standardErrorResults.SetResult(standardError.ToArray());
            };

           

            process.Exited += async (sender, args) => {
                // Since the Exited event can happen asynchronously to the output and error events, 
                // we await the task results for stdout/stderr to ensure they both closed.  We must await
                // the stdout/stderr tasks instead of just accessing the Result property due to behavior on MacOS.  
                // For more details, see the PR at https://github.com/jamesmanning/RunProcessAsTask/pull/16/
                tcs.TrySetResult(new ProcessedResults(process, await standardOutputResults.Task, await standardErrorResults.Task));
            };

            using (cancellationToken.Register(
                () => {
                    tcs.TrySetCanceled();
                    try
                    {
                        if (!process.HasExited)
                            process.Kill();
                    }
                    catch (InvalidOperationException) { }
                }))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (process.Start() == false)
                    tcs.TrySetException(new InvalidOperationException("Failed to start process"));
                

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                return await tcs.Task;
            }
        }

        public static async Task<ProcessedResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken, string[] StandardInputs)
        {
            // force some settings in the start info so we can capture the output
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;
                    
            var tcs = new TaskCompletionSource<ProcessedResults>();

            var standardOutput = new List<string>();
            var standardError = new List<string>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };
             
            var standardOutputResults = new TaskCompletionSource<string[]>();
            process.OutputDataReceived += (sender, args) => {
                if (args.Data != null)
                    standardOutput.Add(args.Data);
                else
                    standardOutputResults.SetResult(standardOutput.ToArray());
            };

            var standardErrorResults = new TaskCompletionSource<string[]>();
            process.ErrorDataReceived += (sender, args) => {
                if (args.Data != null)
                    standardError.Add(args.Data);
                else
                    standardErrorResults.SetResult(standardError.ToArray());
            };


            process.Exited += async (sender, args) => {
                // Since the Exited event can happen asynchronously to the output and error events, 
                // we await the task results for stdout/stderr to ensure they both closed.  We must await
                // the stdout/stderr tasks instead of just accessing the Result property due to behavior on MacOS.  
                // For more details, see the PR at https://github.com/jamesmanning/RunProcessAsTask/pull/16/
                tcs.TrySetResult(new ProcessedResults(process, await standardOutputResults.Task, await standardErrorResults.Task));
            };

            using (cancellationToken.Register(
                () => {
                    tcs.TrySetCanceled();
                    try
                    {
                        if (!process.HasExited)
                            process.Kill();
                    }
                    catch (InvalidOperationException) { }
                }))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (process.Start() == false)
                    tcs.TrySetException(new InvalidOperationException("Failed to start process"));
                
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                return await tcs.Task;
            }
        }

        public static RunResults Run(string exe, string[] input, params string[] args)
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
                                        
                    foreach (var line in input)
                    {
                        process.StandardInput.WriteLineAsync(line);
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




        //public static async Task<string> RunCMD(string[] StandardInputs = null)
        //{
        //    ProcessStartInfo psi = new ProcessStartInfo("cmd");
        //    psi.UseShellExecute = false;
        //    psi.RedirectStandardOutput = true;
        //    psi.RedirectStandardInput = true;
        //    psi.CreateNoWindow = true;

        //    var proc = Process.Start(psi);

        //    var standardOutput = new List<string>();
        //    var standardError = new List<string>();

        //    foreach (var item in StandardInputs)
        //    {
        //        await proc.StandardInput.WriteLineAsync(item);
        //    }

        //    if (proc.HasExited)
        //    {

        //    }

        //    return await proc.StandardOutput.ReadToEndAsync();
        //}

        /// <summary>
        /// Final Result of the process
        /// </summary>
        public static double[] result { get; set; } = new double[4];


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

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        private static bool _start;
        private static IntPtr WinHandle = IntPtr.Zero;

        const int SW_HIDE = 0;
        /// <summary>
        /// Read and Process RINEX file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<double[]> RunRINEX(string filePath, MetroForm frm)
        {
            result[0] = result[1] = result[2] = result[3] = 0;
           
            try
            {
                if (String.IsNullOrEmpty(filePath))
                    throw new FileNotFoundException();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.CreateNoWindow = true;
                psi.FileName = FindExePath("rtkpost.exe"); //see http://csharptest.net/?p=526
                //psi.Arguments = EscapeArguments(args); // see http://csharptest.net/?p=529
                                
                // Read the appropriate line from the file.                   
                StringBuilder newFile = new StringBuilder();
                string temp = "";

                //Make sure the solution file is deleted
                if (File.Exists(Path.Combine(Environment.CurrentDirectory, "Solution.pos")))
                {
                    File.Delete(Path.Combine(Environment.CurrentDirectory, "Solution.pos"));
                }

                string[] file = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "rtkpost.ini"));

                WaitLoadingForm.ExtraStaus = "Loading Settings";

                foreach (string line in file)
                {
                    if (line.Contains("posmode=")) //Processing Mode
                    {
                        temp = "posmode=" + GNSS_Functions.ProcessingMode;    //0 = Single  1 = DGPS/DGNSS  2 = Kinematic  3 = Static DGNSS  4 = Moving Base  5 = Fixed  6 = PPP Kinematic  7 = PPP Static  8 = PPP Fixed 
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("freq=")) //Frequencies to be used
                    {
                        temp = "freq=" + GNSS_Functions.FreqBand;     //0 = L1, 1 = L1 + L2, 2 = L1 + L2 + L5, 3 = L1 + L5
                        newFile.Append(temp + "\r\n");  
                        continue;
                    }

                    if (line.Contains("inputfile1=")) //Rover or PPP input
                    {
                        temp = "inputfile1=" + filePath;
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("inputfile2=")) //Base input
                    {
                        temp = "inputfile2=" + GNSS_Functions.BaseOBSFilePath;
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("inputfile3=")) //Navigation input  Could be both for PPP and
                    {
                        temp = "inputfile3=" + GNSS_Functions.BaseNAVFilePath;
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("inputfile4=")) //Any RINEX additional file
                    {
                        temp = "inputfile4=" + "";
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("inputfile5=")) //Any RINEX additional file
                    {
                        temp = "inputfile5=" + "";
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("inputfile6=")) //Any RINEX additional file
                    {
                        temp = "inputfile6=" + "";
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("outputfile=")) //Solution Output file
                    {
                        temp = "outputfile=" + Path.Combine(Environment.CurrentDirectory, "Solution.pos");
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("refpostype=")) //Base Input type
                    {
                        
                        if (GNSS_Functions.UseAprroximatePos == "Input") //Manual Input
                        {
                            temp = "refpostype=2";
                        }
                        else if (GNSS_Functions.UseAprroximatePos == "No") //Use average single position
                        {
                            temp = "refpostype=3";
                        }
                        else if (GNSS_Functions.UseAprroximatePos == "Yes") //Use RINEX header position
                        {
                            temp = "refpostype=5";
                        }
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (GNSS_Functions.UseAprroximatePos == "Input")
                    {
                        if (line.Contains("refpos1=")) //Base Station X
                        {
                            temp = "refpos1=" + GNSS_Functions.BaseX;
                            newFile.Append(temp + "\r\n");
                            continue;
                        }

                        if (line.Contains("refpos2=")) //Base Station Y
                        {
                            temp = "refpos2=" + GNSS_Functions.BaseY;
                            newFile.Append(temp + "\r\n");
                            continue;
                        }

                        if (line.Contains("refpos3=")) //Base Station Z
                        {
                            temp = "refpos3=" + GNSS_Functions.BaseZ;
                            newFile.Append(temp + "\r\n");
                            continue;
                        }
                    }

                    if (line.Contains("[hist]"))
                    {
                        temp = "[hist]";
                        newFile.Append(temp + "\r\n");
                        break;
                    }
                    else
                    {
                        newFile.Append(line + "\r\n");
                    }
                }

                File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "rtkpost.ini"), newFile.ToString());
                WaitLoadingForm.ExtraStaus = "Processing data";
                Thread.Sleep(800);
                
                using (Process process = Process.Start(psi))
                using (ManualResetEvent mreOut = new ManualResetEvent(false), mreErr = new ManualResetEvent(false))
                {

                    process.WaitForInputIdle();
                    ShowWindow(process.MainWindowHandle, SW_HIDE);
                    IntPtr hwndChild = IntPtr.Zero;
                    
                    WinHandle = process.MainWindowHandle;

                    if (WinHandle == IntPtr.Zero) return result;

                    //Set the parent to hold of the app to open into
                    SetParent(process.MainWindowHandle, frm.Handle);

                    // Move the window to overlay it on this window
                    MoveWindow(WinHandle, 0, 0, frm.Width / 2, frm.Height, true);

                    IntPtr _pointer = FindWindow(new IntPtr(0), process.MainWindowTitle);
                    SetForegroundWindow(_pointer);

                    //Send the Key Press X
                    SendKeys(Keys.X);
                    Application.DoEvents();

                    process.WaitForInputIdle();

                    Thread.Sleep(10000);

                    process.Kill();
                    process.Dispose();
                    
                    using (StreamReader procFile = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Solution.pos")))
                    {

                        var pos = procFile.ReadLine();//.Split(new string[] { @"\n" }, StringSplitOptions.None);
                                                
                        string finalPos = pos;

                        if (GNSS_Functions.UseAprroximatePos != "Input")
                        {
                            
                        }
                        GNSS_Functions.BaseX = finalPos.Substring(14, 14).Trim();
                        GNSS_Functions.BaseY = finalPos.Substring(29, 14).Trim();
                        GNSS_Functions.BaseZ = finalPos.Substring(44, 13).Trim();

                        while (true)
                        {
                            finalPos = pos;
                            if ((pos = procFile.ReadLine()) == null)
                            {
                                break;
                            }
                        }

                        result[0] = double.Parse(finalPos.Substring(27, 15).Trim());
                        result[1] = double.Parse(finalPos.Substring(41, 15).Trim());
                        result[2] = double.Parse(finalPos.Substring(56, 15).Trim());
                        result[3] = double.Parse(finalPos.Substring(71, 3).Trim());
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nFrom here: " + filePath 
                    + "\n" + ex.Source);
            }

            return result;
        }

        /// <summary>
        /// Send Key Pressed to the exe
        /// </summary>
        /// <param name="keys"></param>
        public static void SendKeys(Keys keys)
        {
            int key = (int)keys;
            const int KEYEVENTF_KEYUP = 0x0002;
            keybd_event((byte)key, 0, 0, IntPtr.Zero);
            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        /// <summary>
        /// Get all module about the exe file
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static ProcessModule GetModule(Process p)
        {
            ProcessModule pm = null;
            try { pm = p.MainModule; }
            catch
            {
                return null;
            }
            return pm;
        }

        public static async Task<ProcessedResults> RunCMD(string[] StandardInputs = null)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd");
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.CreateNoWindow = true;

            WaitLoadingForm.ExtraStaus = "Process initialization started";

            var tcs = new TaskCompletionSource<ProcessedResults>();
            var process = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            CancellationToken cancellationToken = CancellationToken.None;
            var standardOutput = new List<string>();
            var standardError = new List<string>();
            var standardOutputResults = new TaskCompletionSource<string[]>();


            process.OutputDataReceived += (sender, args) =>
            {

                if (args.Data != null)
                {
                    standardOutput.Add(args.Data);

                    WaitLoadingForm.ExtraStaus = args.Data;
                }
                else
                {
                    standardOutputResults.SetResult(standardOutput.ToArray());
                }

            };


            process.Exited += async (sender, args) =>
            {
                // Since the Exited event can happen asynchronously to the output and error events, 
                // we await the task results for stdout/stderr to ensure they both closed.  We must await
                // the stdout/stderr tasks instead of just accessing the Result property due to behavior on MacOS.  
                // For more details, see the PR at https://github.com/jamesmanning/RunProcessAsTask/pull/16/
                WaitLoadingForm.ExtraStaus = "Preparing to exit";
                tcs.TrySetResult(new ProcessedResults(process, await standardOutputResults.Task, new string[] { }));
            };


            using (cancellationToken.Register(
                () => {
                    tcs.TrySetCanceled();
                    try
                    {
                        if (!process.HasExited)
                            process.Kill();
                    }
                    catch (InvalidOperationException) { }
                }))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (process.Start() == false)
                    tcs.TrySetException(new InvalidOperationException("Failed to start process"));


                int count = 0;
                foreach (var item in StandardInputs)
                {
                    WaitLoadingForm.ExtraStaus = string.Format("Working on item command: {0}", count++);
                    await process.StandardInput.WriteLineAsync(item);
                }

                process.BeginOutputReadLine();

                WaitLoadingForm.ExtraStaus = "Done";

                GC.Collect();
                return await tcs.Task;
            }

        }
        
    }
}
