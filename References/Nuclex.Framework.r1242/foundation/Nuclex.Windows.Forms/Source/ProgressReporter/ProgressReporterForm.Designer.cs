namespace Nuclex.Windows.Forms {
  partial class ProgressReporterForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.cancelButton = new System.Windows.Forms.Button();
      this.progressBar = new Nuclex.Windows.Forms.AsyncProgressBar();
      this.statusLabel = new System.Windows.Forms.Label();
      this.controlCreationTimer = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(151, 55);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 0;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelClicked);
      // 
      // progressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(12, 26);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(352, 23);
      this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.progressBar.TabIndex = 1;
      // 
      // statusLabel
      // 
      this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.statusLabel.Location = new System.Drawing.Point(12, 9);
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size(352, 14);
      this.statusLabel.TabIndex = 2;
      this.statusLabel.Text = "Please Wait...";
      this.statusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // controlCreationTimer
      // 
      this.controlCreationTimer.Enabled = true;
      this.controlCreationTimer.Interval = 1;
      this.controlCreationTimer.Tick += new System.EventHandler(this.controlCreationTimerTicked);
      // 
      // ProgressReporterForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(376, 90);
      this.ControlBox = false;
      this.Controls.Add(this.statusLabel);
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.cancelButton);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ProgressReporterForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Progress";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button cancelButton;
    private Nuclex.Windows.Forms.AsyncProgressBar progressBar;
    private System.Windows.Forms.Label statusLabel;
    private System.Windows.Forms.Timer controlCreationTimer;
  }
}