namespace ShellRunner
{
  partial class ShellRunnerForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShellRunnerForm));
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.standardOutputTextBox = new System.Windows.Forms.RichTextBox();
      this.standardErrorTextBox = new System.Windows.Forms.RichTextBox();
      this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
      this.mainStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.versionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.mainToolStrip = new System.Windows.Forms.ToolStrip();
      this.commandToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
      this.startToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.stopToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.copyLogToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.clearLogToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.saveLogToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.mainStatusStrip.SuspendLayout();
      this.mainToolStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 25);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.standardOutputTextBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.standardErrorTextBox);
      this.splitContainer.Size = new System.Drawing.Size(689, 490);
      this.splitContainer.SplitterDistance = 355;
      this.splitContainer.TabIndex = 3;
      // 
      // standardOutputTextBox
      // 
      this.standardOutputTextBox.BackColor = System.Drawing.Color.Black;
      this.standardOutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.standardOutputTextBox.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.standardOutputTextBox.ForeColor = System.Drawing.Color.White;
      this.standardOutputTextBox.Location = new System.Drawing.Point(0, 0);
      this.standardOutputTextBox.Name = "standardOutputTextBox";
      this.standardOutputTextBox.ReadOnly = true;
      this.standardOutputTextBox.Size = new System.Drawing.Size(689, 355);
      this.standardOutputTextBox.TabIndex = 0;
      this.standardOutputTextBox.Text = "";
      // 
      // standardErrorTextBox
      // 
      this.standardErrorTextBox.BackColor = System.Drawing.Color.Black;
      this.standardErrorTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.standardErrorTextBox.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.standardErrorTextBox.ForeColor = System.Drawing.Color.Red;
      this.standardErrorTextBox.Location = new System.Drawing.Point(0, 0);
      this.standardErrorTextBox.Name = "standardErrorTextBox";
      this.standardErrorTextBox.ReadOnly = true;
      this.standardErrorTextBox.Size = new System.Drawing.Size(689, 131);
      this.standardErrorTextBox.TabIndex = 1;
      this.standardErrorTextBox.Text = "";
      // 
      // mainStatusStrip
      // 
      this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainStatusLabel,
            this.versionStatusLabel});
      this.mainStatusStrip.Location = new System.Drawing.Point(0, 515);
      this.mainStatusStrip.Name = "mainStatusStrip";
      this.mainStatusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
      this.mainStatusStrip.ShowItemToolTips = true;
      this.mainStatusStrip.Size = new System.Drawing.Size(689, 22);
      this.mainStatusStrip.TabIndex = 5;
      // 
      // mainStatusLabel
      // 
      this.mainStatusLabel.Name = "mainStatusLabel";
      this.mainStatusLabel.Size = new System.Drawing.Size(478, 17);
      this.mainStatusLabel.Spring = true;
      this.mainStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // versionStatusLabel
      // 
      this.versionStatusLabel.IsLink = true;
      this.versionStatusLabel.LinkColor = System.Drawing.Color.Blue;
      this.versionStatusLabel.Name = "versionStatusLabel";
      this.versionStatusLabel.Size = new System.Drawing.Size(196, 17);
      this.versionStatusLabel.Text = "ShellRunner vX.Y.Z by MOBZystems";
      this.versionStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.versionStatusLabel.ToolTipText = "http://www.mobzystems.com/tools/ShellRunner.aspx";
      this.versionStatusLabel.VisitedLinkColor = System.Drawing.Color.Blue;
      this.versionStatusLabel.Click += new System.EventHandler(this.versionStatusLabel_Click);
      // 
      // mainToolStrip
      // 
      this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commandToolStripComboBox,
            this.startToolStripButton,
            this.stopToolStripButton,
            this.toolStripSeparator1,
            this.copyLogToolStripButton,
            this.clearLogToolStripButton,
            this.toolStripSeparator2,
            this.saveLogToolStripButton});
      this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
      this.mainToolStrip.Name = "mainToolStrip";
      this.mainToolStrip.Size = new System.Drawing.Size(689, 25);
      this.mainToolStrip.TabIndex = 6;
      this.mainToolStrip.Text = "toolStrip1";
      // 
      // commandToolStripComboBox
      // 
      this.commandToolStripComboBox.Name = "commandToolStripComboBox";
      this.commandToolStripComboBox.Size = new System.Drawing.Size(121, 25);
      this.commandToolStripComboBox.Visible = false;
      this.commandToolStripComboBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.commandToolStripComboBox_KeyUp);
      // 
      // startToolStripButton
      // 
      this.startToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.startToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("startToolStripButton.Image")));
      this.startToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.startToolStripButton.Name = "startToolStripButton";
      this.startToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.startToolStripButton.Text = "Start";
      this.startToolStripButton.ToolTipText = "Start";
      this.startToolStripButton.Click += new System.EventHandler(this.startToolStripButton_Click);
      // 
      // stopToolStripButton
      // 
      this.stopToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.stopToolStripButton.Enabled = false;
      this.stopToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("stopToolStripButton.Image")));
      this.stopToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.stopToolStripButton.Name = "stopToolStripButton";
      this.stopToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.stopToolStripButton.Text = "Stop";
      this.stopToolStripButton.ToolTipText = "Stop";
      this.stopToolStripButton.Click += new System.EventHandler(this.stopToolStripButton_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // copyLogToolStripButton
      // 
      this.copyLogToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.copyLogToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copyLogToolStripButton.Image")));
      this.copyLogToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.copyLogToolStripButton.Name = "copyLogToolStripButton";
      this.copyLogToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.copyLogToolStripButton.Text = "Copy";
      this.copyLogToolStripButton.Click += new System.EventHandler(this.copyToolStripButton_Click);
      // 
      // clearLogToolStripButton
      // 
      this.clearLogToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.clearLogToolStripButton.Enabled = false;
      this.clearLogToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("clearLogToolStripButton.Image")));
      this.clearLogToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.clearLogToolStripButton.Name = "clearLogToolStripButton";
      this.clearLogToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.clearLogToolStripButton.Text = "Clear";
      this.clearLogToolStripButton.ToolTipText = "Clear log";
      this.clearLogToolStripButton.Click += new System.EventHandler(this.clearLogToolStripButton_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
      // 
      // saveLogToolStripButton
      // 
      this.saveLogToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.saveLogToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveLogToolStripButton.Image")));
      this.saveLogToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.saveLogToolStripButton.Name = "saveLogToolStripButton";
      this.saveLogToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.saveLogToolStripButton.Text = "Save";
      this.saveLogToolStripButton.ToolTipText = "Save log";
      this.saveLogToolStripButton.Click += new System.EventHandler(this.saveLogToolStripButton_Click);
      // 
      // ShellRunnerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(689, 537);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.mainStatusStrip);
      this.Controls.Add(this.mainToolStrip);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "ShellRunnerForm";
      this.Text = "Shell Runner";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.mainStatusStrip.ResumeLayout(false);
      this.mainStatusStrip.PerformLayout();
      this.mainToolStrip.ResumeLayout(false);
      this.mainToolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.RichTextBox standardOutputTextBox;
    private System.Windows.Forms.RichTextBox standardErrorTextBox;
    private System.Windows.Forms.StatusStrip mainStatusStrip;
    private System.Windows.Forms.ToolStripStatusLabel mainStatusLabel;
    private System.Windows.Forms.ToolStrip mainToolStrip;
    private System.Windows.Forms.ToolStripButton startToolStripButton;
    private System.Windows.Forms.ToolStripButton stopToolStripButton;
    private System.Windows.Forms.ToolStripButton clearLogToolStripButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripButton copyLogToolStripButton;
    private System.Windows.Forms.ToolStripButton saveLogToolStripButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripComboBox commandToolStripComboBox;
    private System.Windows.Forms.ToolStripStatusLabel versionStatusLabel;
  }
}

