using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShellRunner
{
  /// <summary>
  /// The main form of this application
  /// </summary>
  public partial class ShellRunnerForm : Form
  {
    private BackgroundWorker worker;
    private ConsoleProcess consoleProcess;

    private string command = null;
    private string commandArguments = "";

    private bool addTimeStamps = false;
    private bool exitIfNoErrors = false;

    private DateTime startTime;
    private DateTime endTime;

    private string input = null;
    private bool commandWasTerminated = false;

    private string saveOutputLogFileName = null;
    private string saveErrorLogFileName = null;
    private string saveLogFileName = null;

    /// <summary>
    /// OnLoad - executed on form load time.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      InitializeComponent();

      Version v = new Version(Application.ProductVersion);
      this.versionStatusLabel.Text = "Shell Runner v" 
        + v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Build.ToString()
        + " (" + (IntPtr.Size * 8).ToString() + "-bit)"
        + " by MOBZystems";

      this.splitContainer.Panel2Collapsed = true;

      // Get the command line
      string[] args = Environment.GetCommandLineArgs();

      bool readingSwitches = true;

      for (int i = 1; i < args.Length; i++)
      {
        string argument = args[i];

        if (readingSwitches)
        {
          if (argument.StartsWith("/") || argument.StartsWith("-"))
          {
            switch (argument.Substring(1, 1).ToLowerInvariant())
            {
              case "e":
                this.splitContainer.Panel2Collapsed = false;
                break;
              case "t":
                this.addTimeStamps = true;
                break;
              case "i":
                if (!AssertColon(argument, 2, "required ':' missing"))
                  return;
                try
                {
                  this.input = File.ReadAllText(argument.Substring(3), System.Text.Encoding.Default);
                }
                catch (Exception ex)
                {
                  Usage(ex.Message);
                  return;
                }
                break;
              case "x":
                this.exitIfNoErrors = true;
                break;
              case "s":
                switch (argument.Substring(2, 1).ToLowerInvariant())
                {
                  case "o":
                    if (!AssertColon(argument, 3, "required ':' missing"))
                      return;
                    this.saveOutputLogFileName = argument.Substring(4);
                    break;
                  case "e":
                    if (!AssertColon(argument, 3, "required ':' missing"))
                      return;
                    this.saveErrorLogFileName = argument.Substring(4);
                    break;
                  case "l":
                    if (!AssertColon(argument, 3, "required ':' missing"))
                      return;
                    this.saveLogFileName = argument.Substring(4);
                    break;
                  default:
                    Usage("Invalid /s switch: '" + argument + "'");
                    break;
                }
                break;
              default:
                Usage("Invalid switch: '" + argument + "'");
                return;
            }
          }
          else
            readingSwitches = false;
        }

        if (!readingSwitches)
        {
          if (command == null)
            this.command = argument;
          else
            this.commandArguments += " " + argument;
        }
      }

      if (command == null)
      {
        Usage("No command specified.");
        return;
      }

      // this.commandToolStripComboBox.Text = command;

      Start();
    }

    /// <summary>
    /// Assert the the argument has a colon at the specified position
    /// </summary>
    private bool AssertColon(string s, int position, string message)
    {
      if (s.Length < position || s.Substring(position, 1) != ":")
      {
        Usage("'" + s + "': " + message);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Start the process
    /// </summary>
    private void Start()
    {
      // Set up the console process and events
      this.consoleProcess = new ConsoleProcess();
      this.consoleProcess.OnOutputReceived += new ConsoleProcess.OutputReceivedHandler(ConsoleProcess_OnOutputReceived);
      this.consoleProcess.OnErrorReceived += new ConsoleProcess.OutputReceivedHandler(ConsoleProcess_OnErrorReceived);

      // Set up the background worker and events
      this.worker = new BackgroundWorker();
      this.worker.DoWork += new DoWorkEventHandler(worker_DoWork);
      this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

      // Start the command!
      this.mainStatusLabel.Text = "Running '" + this.command + "'...";
      this.worker.RunWorkerAsync();
      this.startTime = DateTime.Now;
      this.Text = "Running '" + this.command + "' - Shell Runner";

      this.startToolStripButton.Enabled = false;
      this.stopToolStripButton.Enabled = true;
      this.copyLogToolStripButton.Enabled = false;
      this.clearLogToolStripButton.Enabled = false;
      this.saveLogToolStripButton.Enabled = false;
    }

    /// <summary>
    /// Show usage on the screen
    /// </summary>
    /// <param name="message">An explanatory message</param>
    private void Usage(string message)
    {
      this.mainStatusLabel.Text = message;

      Version v = new Version(Application.ProductVersion);

      AddOutput(standardOutputTextBox, "Shell Runner v" + v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Build.ToString() + "\n", false);

      AddOutput(standardOutputTextBox, message, true);

      AddOutput(standardOutputTextBox, @"
Usage: ShellRunner [switches] <command> [argument1 [argument2] ...]

Switches:
   /t         - Timestamp output
   /e         - show Errors pane
   /i:[file]  - write file to standard Input
   /sl:[file] - Save Log to file
   /se:[file] - Save Error output to file
   /so:[file] - Save Output to file
   /x         - eXit if no error output
", false);
    }

    /// <summary>
    /// Start the worker thread, i.e. run the console process
    /// </summary>
    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
      this.commandWasTerminated = false;

      this.consoleProcess.CommandFileName = this.command;

      try
      {
        RemoveLogFileIfPresent(this.saveOutputLogFileName);
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Error deleting output log file", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      try
      {
        RemoveLogFileIfPresent(this.saveErrorLogFileName);
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Error deleting error log file", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      try
      {
        RemoveLogFileIfPresent(this.saveLogFileName);
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Error deleting log file", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      RemoveLogFileIfPresent(this.saveErrorLogFileName);
      RemoveLogFileIfPresent(this.saveLogFileName);

      // Get the current directory from the supplied command argument
      string currentDir = Path.GetDirectoryName(this.command);
      if (currentDir.Length == 0)
        currentDir = null;

      int exitCode = this.consoleProcess.ExecuteCommand(this.commandArguments, this.consoleProcess.Timeout, this.input, currentDir); // 2>&1 = stderr --> stdout

      e.Result = exitCode;

      try
      {
        if (this.saveOutputLogFileName != null)
          File.WriteAllText(this.saveOutputLogFileName, this.consoleProcess.StandardOutput);
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Error saving output", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      try
      {
        if (this.saveErrorLogFileName != null)
          File.WriteAllText(this.saveErrorLogFileName, this.consoleProcess.StandardError);
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Error saving error output", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      try
      {
        if (this.saveLogFileName != null)
          File.WriteAllText(this.saveLogFileName, this.standardOutputTextBox.Text.Replace("\n", Environment.NewLine));
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.Message, "Error saving log", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    /// <summary>
    /// Remove a log file name if it exists
    /// </summary>
    /// <param name="filename"></param>
    private void RemoveLogFileIfPresent(string filename)
    {
      if (filename != null)
        if (File.Exists(filename))
          File.Delete(filename);
    }

    /// <summary>
    /// The worker process completed, e.g. the command terminated
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      this.endTime = DateTime.Now;

      bool mustExitApplication = false;

      string message;
      if (e.Error != null)
      {
        message = "Error running '" + this.command + "': " + e.Error.Message;
        this.Text = "Error '" + this.command + "' - Shell Runner";
      }
      else if (this.commandWasTerminated)
      {
        message = "Terminated after " + ((TimeSpan)(this.endTime.Subtract(this.startTime))).TotalSeconds.ToString("#,,0") + " seconds.";
        this.Text = "Terminated '" + this.command + "' - Shell Runner";
      }
      else
      {
        message = "'" + this.command + "' completed in " + ((TimeSpan)(this.endTime.Subtract(this.startTime))).TotalSeconds.ToString("#,,0") + " seconds. Result: " + e.Result.ToString();
        this.Text = "Completed '" + this.command + "' - Shell Runner";

        mustExitApplication = (this.exitIfNoErrors && this.consoleProcess.StandardError.Length == 0);
      }

      this.mainStatusLabel.Text = message;

      this.consoleProcess = null;

      this.startToolStripButton.Enabled = true;
      this.stopToolStripButton.Enabled = false;
      this.copyLogToolStripButton.Enabled = true;
      this.clearLogToolStripButton.Enabled = true;
      this.saveLogToolStripButton.Enabled = true;

      if (mustExitApplication)
        this.Close();
    }

    /// <summary>
    /// Delegate to support calling the AddOutput method via Invoke()
    /// </summary>
    /// <param name="textBox"></param>
    /// <param name="output"></param>
    private delegate void AddOutputDelegate(RichTextBox textBox, string output, bool isErrorOutput);

    private void AddOutput(RichTextBox textBox, string output, bool isErrorOutput)
    {
      int length = textBox.TextLength;

      if (this.addTimeStamps && output.Length > 0)
      {
        textBox.AppendText(DateTime.Now.ToString("yyyy\\-MM\\-dd HH\\:mm\\:ss") + " ");

        textBox.Select(length, textBox.TextLength - length);
        textBox.SelectionColor = Color.Cyan;
        length = textBox.TextLength;
      }

      textBox.AppendText(output);

      textBox.Select(length, textBox.TextLength - length);

      if (isErrorOutput)
      {
        textBox.SelectionColor = this.standardErrorTextBox.ForeColor;
      }
      else
      {
        textBox.SelectionColor = this.standardOutputTextBox.ForeColor;
      }

      textBox.SelectionStart = textBox.TextLength;
      textBox.SelectionLength = 0;

      textBox.AppendText(Environment.NewLine);
    }

    /// <summary>
    /// This method gets called when there is regular output available from the running console process
    /// </summary>
    /// <param name="sender">The console process</param>
    /// <param name="output">The line read, EXCLUDING NEWLINE</param>
    private void ConsoleProcess_OnOutputReceived(object sender, string output)
    {
      if (InvokeRequired)
      {
        Invoke(new AddOutputDelegate(this.AddOutput), new object[] { this.standardOutputTextBox, output, false });
      }
      else
      {
        AddOutput(standardOutputTextBox, output, false);
      }
    }

    /// <summary>
    /// This method gets called when there is error output available from the running console process
    /// </summary>
    /// <param name="sender">The console process</param>
    /// <param name="output">The line read, EXCLUDING NEWLINE</param>
    private void ConsoleProcess_OnErrorReceived(object sender, string output)
    {
      if (InvokeRequired)
      {
        Invoke(new AddOutputDelegate(this.AddOutput), new object[] { this.standardOutputTextBox, output, true });
      }
      else
      {
        AddOutput(standardOutputTextBox, output, true);
      }

      if (InvokeRequired)
      {
        Invoke(new AddOutputDelegate(this.AddOutput), new object[] { this.standardErrorTextBox, output, true });
      }
      else
      {
        AddOutput(standardErrorTextBox, output, true);
      }
    }

    /// <summary>
    /// Make sure we don't close the form when a command is still executing
    /// </summary>
    /// <param name="e"></param>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      if (this.consoleProcess != null)
      {
        MessageBox.Show(this, "Cannot stop while process is running. Stop process first.", "Shell Runner", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
      }

      base.OnFormClosing(e);
    }

    private void startToolStripButton_Click(object sender, EventArgs e)
    {
      Start();
    }

    private void stopToolStripButton_Click(object sender, EventArgs e)
    {
      this.consoleProcess.Terminate(-999);
      this.commandWasTerminated = true;
    }

    private void clearLogToolStripButton_Click(object sender, EventArgs e)
    {
      this.standardErrorTextBox.Clear();
      this.standardOutputTextBox.Clear();
    }

    private void copyToolStripButton_Click(object sender, EventArgs e)
    {
      Clipboard.SetText(this.standardOutputTextBox.Text.Replace("\n", Environment.NewLine));
    }

    private void saveLogToolStripButton_Click(object sender, EventArgs e)
    {
      using (SaveFileDialog saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Title = "Save log...";
        saveFileDialog.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files|*.*";
        saveFileDialog.OverwritePrompt = true;
        if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
        {
          System.IO.File.WriteAllText(saveFileDialog.FileName, this.standardOutputTextBox.Text.Replace("\n", Environment.NewLine));
        }
      }
    }

    private void commandToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        Start();
    }

    private void versionStatusLabel_Click(object sender, EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;
      try
      {
        System.Diagnostics.Process.Start("http://www.mobzystems.com/tools/ShellRunner.aspx");
      }
      catch
      {
        MessageBox.Show("There was an error starting the URL");
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }
  }
}