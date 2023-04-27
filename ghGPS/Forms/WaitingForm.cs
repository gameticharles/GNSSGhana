using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ghGPS.Forms
{
    public partial class WaitingForm : MetroSuite.MetroForm
    {
        public WaitingForm(string rover = null)
        {
            InitializeComponent();

            //timer1.Start();

            RoverPath = rover;
        }

        public string ExtraStaus = "";
        /// <summary>
        /// Path to the Rover or PPP observation file 
        /// </summary>
        public static string RoverPath { get; set; } = string.Empty;

        /// <summary>
        /// Path to Base observation file 
        /// </summary>
        public static string BasePath { get; set; } = string.Empty;

        /// <summary>
        /// Final Result of the process
        /// </summary>
        public virtual double[] result { get; set; } = new double[4];

        /// <summary>
        /// Path to Navigation file to better process result
        /// </summary>
        public static string NavigationPath { get; set; } = string.Empty;

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

        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the evironment's path.
        /// </summary>
        /// <param name="exe">The name of the executable file</param>
        /// <returns>The fully-qualified path to the file</returns>
        /// <exception cref="System.IO.FileNotFoundException">Raised when the exe was not found</exception>
        public string FindExePath(string exe)
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
        public string EscapeArguments(params string[] args)
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

        const int SW_HIDE = 0;
        /// <summary>
        /// Read and Process RINEX file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<double[]> RunRINEX(string filePath)
        {            
            result[0] = result[1] = result[2] = result[3] = 0;

            try
            {
                if (String.IsNullOrEmpty(filePath))
                    throw new FileNotFoundException();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.UseShellExecute = false ; //Set to true if window should be hidden
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                psi.ErrorDialog = false;
                psi.WorkingDirectory = Environment.CurrentDirectory;
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

                foreach (string line in file)
                {

                    if (line.Contains("inputfile1=")) //Rover or PPP input
                    {
                        temp = "inputfile1=" + filePath;
                        newFile.Append(temp + "\r\n");
                        continue;
                    }

                    if (line.Contains("inputfile2=")) //Base input
                    {
                        temp = "inputfile2=" + @"C:\Users\Reindroid\Desktop\DATA\Sat Geodesy\Base.18O";
                        newFile.Append(temp + "\r\n");
                        continue;
                    }


                    if (line.Contains("inputfile3=")) //Navigation input  Could be both for PPP and
                    {
                        temp = "inputfile3=" + @"C:\Users\Reindroid\Desktop\DATA\Sat Geodesy\Base.18N";
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

                    newFile.Append(line + "\r\n");

                }

                File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "rtkpost.ini"), newFile.ToString());

                Thread.Sleep(1000);

                using (Process process = Process.Start(psi))
                using (ManualResetEvent mreOut = new ManualResetEvent(false), mreErr = new ManualResetEvent(false))
                {

                    process.WaitForInputIdle();
                    ShowWindow(process.MainWindowHandle, SW_HIDE);
                    IntPtr hwndChild = IntPtr.Zero;
                    //Process[] localAll = Process.GetProcesses();
                    //foreach (Process p in localAll)
                    //{
                    //    if (p.MainWindowHandle != IntPtr.Zero)
                    //    {
                    //        ProcessModule pm = GetModule(p);
                    //        if (pm != null && p.MainModule.FileName == exe)
                    //            handle = p.MainWindowHandle;
                    //    }
                    //}
                                        
                    WinHandle = process.MainWindowHandle;

                    if (WinHandle == IntPtr.Zero) return result;

                    //Set the parent to hold of the app to open into
                    SetParent(process.MainWindowHandle, this.Handle);

                    // Move the window to overlay it on this window
                    MoveWindow(WinHandle, 0, 0, this.Width / 2, this.Height, true);

                    IntPtr _pointer = FindWindow(new IntPtr(0), process.MainWindowTitle);
                    SetForegroundWindow(_pointer);

                    //Send the Key Press X
                    SendKeys(Keys.X);
                    Application.DoEvents();

                    process.WaitForInputIdle();
                    
                    Thread.Sleep(1000);

                    process.Kill();
                    process.Dispose();

                    using (StreamReader procFile = new StreamReader(Path.Combine(Environment.CurrentDirectory, "Solution.pos")))
                    {

                        var pos = await procFile.ReadLineAsync();//.Split(new string[] { @"\n" }, StringSplitOptions.None);
                        string finalPos = pos;
                        while (true)
                        {
                            finalPos = pos;
                            if ((pos = await procFile.ReadLineAsync()) == null)
                            {
                                break;
                            }
                            //.Split(new string[] { @"\n" }, StringSplitOptions.None);
                        }

                        result[0] = double.Parse(finalPos.Substring(27, 15).Trim());
                        result[1] = double.Parse(finalPos.Substring(42, 15).Trim());
                        result[2] = double.Parse(finalPos.Substring(57, 15).Trim());
                        result[3] = double.Parse(finalPos.Substring(72, 3).Trim());

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;

        }

        /// <summary>
        /// Send Key Pressed to the exe
        /// </summary>
        /// <param name="keys"></param>
        public void SendKeys(Keys keys)
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
        private ProcessModule GetModule(Process p)
        {
            ProcessModule pm = null;
            try { pm = p.MainModule; }
            catch
            {
                return null;
            }
            return pm;
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            RunRINEX(RoverPath);
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            //IntPtr appWin1;
            //IntPtr appWin2;

            //try
            //{
            //    ProcessStartInfo ps1 = new ProcessStartInfo("notepad.exe");
            //    ps1.WindowStyle = ProcessWindowStyle.Minimized;
            //    Process p1 = Process.Start(ps1);
            //    p1.WaitForInputIdle(); // Allow the process to open it's window
            //    appWin1 = p1.MainWindowHandle;
            //    // Put it into this form
            //    SetParent(appWin1, this.Handle);


            //    // Move the window to overlay it on this window
            //    MoveWindow(appWin1, 0, 0, this.Width / 2, this.Height, true);



            //    ProcessStartInfo ps2 = new ProcessStartInfo("notepad.exe");
            //    //ps2.WindowStyle = ProcessWindowStyle.Minimized;
            //    Process p2 = Process.Start(ps2);
            //    p2.WaitForInputIdle(); // Allow the process to open it's window
            //    appWin2 = p2.MainWindowHandle;
            //    // Put it into this form
            //    SetParent(p2.MainWindowHandle, this.Handle);
            //    MoveWindow(p2.MainWindowHandle, this.Width / 2, 0, this.Width / 2, this.Height, true);

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this, ex.Message, "Error");
            //}
        }

        private void WaitingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //timer1.Stop();
        }

        int DotNumber = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {

            //if (DotNumber == 6)
            //{
            //    DotNumber = 0;
            //}
            //DotNumber++;

            //lblStatus.Text = "Processing" + new string('.', DotNumber) + "\n" + ExtraStaus;
        }
    }
}
