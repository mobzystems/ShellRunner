using System;
using System.Text;

using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace ShellRunner
{
  /*
   * This file contains the ConsoleProcess and related classes.
   */

  /// <summary>
  /// ConsoleProcess. Controller class for console applications.
  /// </summary>
  class ConsoleProcess
  {
    #region "Windows 32 API"

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct PROCESS_INFORMATION
    {
      public IntPtr hProcess;
      public IntPtr hThread;
      public uint dwProcessId;
      public uint dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct PROCESS_BASIC_INFORMATION
    {
      public int ExitStatus;
      public int PebBaseAddress;
      public int AffinityMask;
      public int BasePriority;
      public uint UniqueProcessId;
      public uint InheritedFromUniqueProcessId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct STARTUPINFO
    {
      public uint cb;
      public string lpReserved;
      public string lpDesktop;
      public string lpTitle;
      public uint dwX;
      public uint dwY;
      public uint dwXSize;
      public uint dwYSize;
      public uint dwXCountChars;
      public uint dwYCountChars;
      public uint dwFillAttribute;
      public uint dwFlags;
      public short wShowWindow;
      public short cbReserved2;
      public IntPtr lpReserved2;
      public IntPtr hStdInput;
      public IntPtr hStdOutput;
      public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct SECURITY_ATTRIBUTES
    {
      public int length;
      public IntPtr lpSecurityDescriptor;
      public uint bInheritHandle;
    }

    [DllImport("kernel32.dll")]
    static extern uint CreateProcess(
      string lpApplicationName,
      string lpCommandLine,
      /* ref SECURITY_ATTRIBUTES */ IntPtr lpProcessAttributes,
      /* ref SECURITY_ATTRIBUTES */ IntPtr lpThreadAttributes,
      uint bInheritHandles,
      uint dwCreationFlags,
      IntPtr lpEnvironment,
      string lpCurrentDirectory,
      ref STARTUPINFO lpStartupInfo,
      out PROCESS_INFORMATION lpProcessInformation
    );

    [DllImport("kernel32.dll")]
    static extern uint FreeConsole();

    [DllImport("kernel32.dll")]
    static extern uint AttachConsole(uint dwProcessID);

    [DllImport("kernel32.dll")]
    static extern uint ResumeThread(IntPtr hThread);

    [DllImport("kernel32.dll")]
    static extern uint CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES sattr, uint size);

    [DllImport("kernel32.dll")]
    static extern uint SetHandleInformation(IntPtr hHandle, uint mask, uint flags);

    [DllImport("kernel32.dll")]
    static extern uint CloseHandle(IntPtr hHandle);

    [DllImport("kernel32.dll")]
    static extern uint ReadFile(IntPtr handle, ref byte buffer, uint bufsize, out uint dwRead, IntPtr OverlappedRead);

    [DllImport("kernel32.dll")]
    static extern uint WriteFile(IntPtr handle, ref byte buffer, uint bufsize, out uint dwWritten, IntPtr OverlappedRead);

    [DllImport("kernel32.dll")]
    static extern uint WaitForMultipleObjects(uint count, ref IntPtr handles, short waitAll, uint dwMilliSeconds);

    [DllImport("kernel32.dll")]
    static extern uint GetExitCodeProcess(IntPtr hProcess, out int exitCode);

    [DllImport("kernel32.dll")]
    static extern uint TerminateProcess(IntPtr hProcess, int exitCode);

    [DllImport("kernel32.dll")]
    static extern uint TerminateThread(IntPtr hThread, int exitCode);

    [DllImport("ntdll.dll")]
    static extern int NtQueryInformationProcess(IntPtr hProcess, int processInformationClass /* 0 */,
      ref PROCESS_BASIC_INFORMATION processBasicInformation, uint processInformationLength, out uint returnLength);

    #endregion

    // The name of the command to execute
    private string commandFileName;

    // The default timeout (infinite) in milliseconds
    private uint timeout = 0xFFFFFFFF;

    // The results of reading standard output and standard error
    // Available during execution as well as afterwards
    private StringBuilder standardOutput;
    private StringBuilder standardError;

    // The currently running process
    // private Process runningProcess;
    private PROCESS_INFORMATION processInformation = new PROCESS_INFORMATION();

    // Handles to pipes
    private IntPtr hChildStdinRd, hChildStdinWr, hChildStdoutRd, hChildStdoutWr, hChildStdErrorRd, hChildStdErrorWr;

    private string currentOutputLine;
    private string currentErrorLine;

    // Read buffer size
    private const int BUFSIZE = 1;

    /// <summary>
    /// Delegate for the OnOutputReceived and OnErrorReceived events
    /// </summary>
    public delegate void OutputReceivedHandler(object sender, string output);

    /// <summary>
    /// Raised when output is available on standard output
    /// </summary>
    public event OutputReceivedHandler OnOutputReceived;
    private void RaiseOnOutputReceived(string output)
    {
      if (this.OnOutputReceived != null)
        this.OnOutputReceived(this, output);
    }

    /// <summary>
    /// Raised when output is available on standard error
    /// </summary>
    public event OutputReceivedHandler OnErrorReceived;
    private void RaiseOnErrorReceived(string output)
    {
      if (this.OnErrorReceived != null)
        this.OnErrorReceived(this, output);
    }

    /// <summary>
    /// Property CommandFileName (string). The file name of the console executable.
    /// Must include a path if not in PATH.
    /// </summary>
    public string CommandFileName
    {
      get
      {
        return this.commandFileName;
      }
      set
      {
        this.commandFileName = value;
      }
    }

    /// <summary>
    /// Property Timeout (int). Get/Set the timeout in ms
    /// </summary>
    public uint Timeout
    {
      get
      {
        return this.timeout;
      }
      set
      {
        this.timeout = value;
      }
    }

    /// <summary>
    /// Execute a console command in a hidden window with a timeout and standard input feed
    /// <param name="commandline">The command line to ecexute</param>
    /// <param name="timeoutMilliSeconds">A timeout in milliseconds.</param>
    /// <param name="standardInput">The string to feed to standard input of the process</param>
    /// <param name="currentDir">The current directory for the process</param>
    /// <returns>The exit code of the process</returns>
    /// </summary>
    public int ExecuteCommand(string commandline, uint timeoutMilliSeconds, string standardInput, string currentDir)
    {
      if (this.processInformation.hProcess != IntPtr.Zero)
        throw new ApplicationException("Cannot execute a command when another command is still executing");

      try
      {
        // Create security attributes to be able to set pipes to NO INHERIT
        SECURITY_ATTRIBUTES saAttr = new SECURITY_ATTRIBUTES();

        saAttr.length = Marshal.SizeOf(saAttr);
        saAttr.bInheritHandle = 1; // true
        saAttr.lpSecurityDescriptor = IntPtr.Zero;

        // Create a pipe for the child process's STDIN. 
        if (CreatePipe(out this.hChildStdinRd, out this.hChildStdinWr, ref saAttr, 0) == 0)
          throw new ApplicationException("Could not redirect stdin");

        // Ensure the write handle to the pipe for STDIN is not inherited. 
        SetHandleInformation(this.hChildStdinWr, 1 /* HANDLE_FLAG_INHERIT */, 0);

        // Create a pipe for the child process's STDOUT. 
        if (CreatePipe(out this.hChildStdoutRd, out this.hChildStdoutWr, ref saAttr, 0) == 0)
          throw new ApplicationException("Could not redirect stdout");

        // Ensure the read handle to the pipe for STDOUT is not inherited.
        SetHandleInformation(hChildStdoutRd, 1 /* HANDLE_FLAG_INHERIT */, 0);

        // Create a pipe for the child process's STDERR. 
        if (CreatePipe(out this.hChildStdErrorRd, out this.hChildStdErrorWr, ref saAttr, 0) == 0)
          throw new ApplicationException("Could not redirect stderr");

        // Ensure the write handle to the pipe for STDIN is not inherited. 
        SetHandleInformation(this.hChildStdErrorRd, 1 /* HANDLE_FLAG_INHERIT */, 0);

        // Now create the child process in a suspended state
        // with handles redirected to pipes
        STARTUPINFO si = new STARTUPINFO();
        si.cb = (uint)Marshal.SizeOf(si);
        si.hStdInput = this.hChildStdinRd;
        si.hStdOutput = this.hChildStdoutWr;
        si.hStdError = this.hChildStdErrorWr;
        si.dwFlags |= 0x100; /*  STARTF_USESTDHANDLES; */
        si.wShowWindow = 0;

        this.processInformation = new PROCESS_INFORMATION();

        string effectiveCommandLine = "\"" + CommandFileName + "\"";
        if (!string.IsNullOrEmpty(commandline))
          effectiveCommandLine += " " + commandline;

        if (CreateProcess(
          CommandFileName,
          effectiveCommandLine,
          IntPtr.Zero,  /* process attributes */
          IntPtr.Zero,  /* thread attributes */
          1 /* true */, /* inherit handles */
          4 /* suspended*/ | 0x08000000 /* CREATE_NO_WINDOW */,
          IntPtr.Zero,  /* environment */
          currentDir,   /* current dir */
          ref si,       /* Startup-info */
          out this.processInformation)
         == 0)
          throw new ApplicationException("Could not create process");

        // Close the write end of the pipes before reading from the 
        // read end of the pipes. 

        if (CloseHandle(hChildStdoutWr) == 0)
          throw new ApplicationException("Could not close stdout");

        if (CloseHandle(hChildStdErrorWr) == 0)
          throw new ApplicationException("Could not close stderr");

        this.standardOutput = new StringBuilder();
        this.standardError = new StringBuilder();

        this.currentOutputLine = "";
        this.currentErrorLine = "";

        // Start a new thread to write to standard input if necessary

        Thread standardInputWriter = null;
        if (standardInput != null)
        {
          // Print to standard input:
          standardInputWriter = new Thread(new ParameterizedThreadStart(WriteStandardInput));
          standardInputWriter.Start(standardInput);
        }

        // Start two new threads to read standard output and standard error on the process

        // Create the 'standard output reader' thread
        Thread standardOutputReader = new Thread(new ThreadStart(ReadStandardOutput));
        standardOutputReader.Start();

        // Create the 'standard error reader' thread
        Thread standardErrorReader = new Thread(new ThreadStart(ReadStandardError));
        standardErrorReader.Start();

        // Resume the thread, i.e. start the process
        ResumeThread(this.processInformation.hThread);

        // Wait for the process to end
        switch (WaitForMultipleObjects(1, 
          ref this.processInformation.hProcess, 
          1 /* true */, 
          timeoutMilliSeconds)
        )
        {
          case 0:
            // The process ended normally, or it was Terminate()d
            break;
          case 0x102: // There was a timeout
            TerminateThread(this.processInformation.hThread, -1);
            TerminateProcess(this.processInformation.hProcess, -1);
            break;
          default:
            // This is unexpected!
            break;
        }

        // Wait for the two threads to rejoin:
        standardOutputReader.Join();
        standardErrorReader.Join();

        // Kill the standard input writer at the end of the process
        if (standardInputWriter != null)
        {
          standardInputWriter.Abort();
        }

        // Return exit code of the process
        int exitCode;

        if (GetExitCodeProcess(this.processInformation.hProcess, out exitCode) == 0)
          throw new ApplicationException("Could not retrieve exit code");

        return exitCode;
      }
      finally
      {
        // Clean up when exception happens
        if (this.processInformation.hProcess != IntPtr.Zero)
          CloseHandle(this.processInformation.hProcess);
        if (this.processInformation.hThread != IntPtr.Zero)
          CloseHandle(this.processInformation.hThread);

        // Reset the running process
        this.processInformation = new PROCESS_INFORMATION();
      }
    }

    /// <summary>
    /// Execute a console command in a hidden window with a timeout and standard input feed
    /// <param name="commandline">The command line to ecexute</param>
    /// <param name="timeoutMilliSeconds">A timeout in milliseconds.</param>
    /// <param name="standardInput">The string to feed to standard input of the process</param>
    /// <returns>The exit code of the process</returns>
    /// </summary>
    public int ExecuteCommand(string commandline, uint timeoutMilliSeconds, string standardInput)
    {
      return ExecuteCommand(commandline, timeoutMilliSeconds, standardInput, null);
    }

    /// <summary>
    /// Execute a console command in a hidden window
    /// </summary>
    /// <param name="commandline">The command line to ecexute</param>
    /// <param name="timeoutMilliSeconds">A timeout in milliseconds.</param>
    /// <returns>The exit code of the process</returns>
    public int ExecuteCommand(string commandline, uint timeoutMilliSeconds)
    {
      return ExecuteCommand(commandline, timeoutMilliSeconds, null);
    }

    /// <summary>
    /// Execute a console command with a default timeout
    /// </summary>
    /// <param name="commandline"></param>
    /// <returns></returns>
    public int ExecuteCommand(string commandline)
    {
      return ExecuteCommand(commandline, this.timeout);
    }

    /// <summary>
    /// Force the command to end with an exit code
    /// </summary>
    /// <param name="exitCode"></param>
    public void Terminate(int exitCode)
    {
      if (this.processInformation.hProcess == IntPtr.Zero)
        throw new ApplicationException("Process is not running");

      // Terminate the process TREE here
      TerminateProcessTree(this.processInformation.hProcess, this.processInformation.dwProcessId, exitCode);
    }

    /// <summary>
    /// Terminate a process tree
    /// </summary>
    /// <param name="hProcess">The handle of the process</param>
    /// <param name="processID">The ID of the process</param>
    /// <param name="exitCode">The exit code of the process</param>
    public void TerminateProcessTree(IntPtr hProcess, uint processID, int exitCode)
    {
      // Retrieve all processes on the system
      Process[] processes = Process.GetProcesses();
      foreach (Process p in processes)
      {
        // Get some basic information about the process
        PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
        try
        {
          uint bytesWritten;
          NtQueryInformationProcess(p.Handle,
            0, ref pbi, (uint)Marshal.SizeOf(pbi),
            out bytesWritten); // == 0 is OK

          // Is it a child process of the process we're trying to terminate?
          if (pbi.InheritedFromUniqueProcessId == processID)
            // The terminate the child process and its child processes
            TerminateProcessTree(p.Handle, pbi.UniqueProcessId, exitCode);
        }
        catch (Exception /* ex */)
        {
          // Ignore, most likely 'Access Denied'
        }
      }

      // Finally, termine the process itself:
      TerminateProcess(hProcess, exitCode);
    }

    /// <summary>
    /// Property StandardOutput (string).
    /// Returns the result of the command execution
    /// </summary>
    public string StandardOutput
    {
      get
      {
        lock (this)
        {
          return this.standardOutput.ToString();
        }
      }
    }

    /// <summary>
    /// Property StandardError (string).
    /// Returns the error result of the command execution
    /// </summary>
    public string StandardError
    {
      get
      {
        lock (this)
        {
          return this.standardError.ToString();
        }
      }
    }

    /// <summary>
    /// Write a string to standard input of the process.
    /// This method is called on its own thread
    /// </summary>
    /// <param name="input">The string to write to standard input</param>
    private void WriteStandardInput(object input)
    {
      string standardInput = (string)input;
      byte[] buf = Encoding.ASCII.GetBytes(standardInput);
      uint dwWritten;
      WriteFile(hChildStdinWr, ref buf[0], (uint)buf.Length, out dwWritten, IntPtr.Zero);
      CloseHandle(hChildStdinWr);
    }

    /// <summary>
    /// Read standard output of the process.
    /// This method is called on its own thread
    /// </summary>
    private void ReadStandardOutput()
    {
      uint dwRead;
      byte[] chBuf = new byte[BUFSIZE];

      for (; ; )
      {
        if (ReadFile(hChildStdoutRd, ref chBuf[0], BUFSIZE, out dwRead, IntPtr.Zero) == 0)
          break;

        if (dwRead != 0)
        {
          string output = Encoding.ASCII.GetString(chBuf, 0, (int)dwRead);

          this.currentOutputLine += output;

          lock (this)
          {
            int p;
            while ((p = this.currentOutputLine.IndexOf("\r\n")) >= 0)
            {
              string line = this.currentOutputLine.Substring(0, p);
              if (line.Length > 0)
                this.RaiseOnOutputReceived(line);
              if (p + 2 >= this.currentOutputLine.Length)
              {
                this.currentOutputLine = "";
                break;
              }
              else
                this.currentOutputLine = this.currentOutputLine.Substring(p + 2);
            }
          }

          this.standardOutput.Append(output);
        }
      }
    }

    /// <summary>
    /// Read standard error output of the process.
    /// This method is called on its own thread
    /// </summary>
    private void ReadStandardError()
    {
      uint dwRead;
      byte[] chBuf = new byte[BUFSIZE];

      for (; ; )
      {
        if (ReadFile(hChildStdErrorRd, ref chBuf[0], BUFSIZE, out dwRead, IntPtr.Zero) == 0)
          break;

        if (dwRead != 0)
        {
          string error = System.Text.Encoding.ASCII.GetString(chBuf, 0, (int)dwRead);

          this.currentErrorLine += error;

          lock (this)
          {
            int p;
            while ((p = this.currentErrorLine.IndexOf("\r\n")) >= 0)
            {
              string line = this.currentErrorLine.Substring(0, p);
              if (line.Length > 0)
                this.RaiseOnErrorReceived(line);
              if (p + 2 >= this.currentErrorLine.Length)
              {
                this.currentErrorLine = "";
                break;
              }
              else
                this.currentErrorLine = this.currentErrorLine.Substring(p + 2);
            }
          }

          this.standardError.Append(error);
        }
      }
    }
  }
}
