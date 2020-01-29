namespace Workshell.DiskImager
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.gbxImage = new System.Windows.Forms.GroupBox();
            this.btnBrowseImage = new System.Windows.Forms.Button();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.gbxDisk = new System.Windows.Forms.GroupBox();
            this.ddlDevice = new System.Windows.Forms.ComboBox();
            this.gbxHash = new System.Windows.Forms.GroupBox();
            this.btnClearHash = new System.Windows.Forms.Button();
            this.btnBrowseHash = new System.Windows.Forms.Button();
            this.txtHash = new System.Windows.Forms.TextBox();
            this.gbxProgress = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblSpeedValue = new System.Windows.Forms.Label();
            this.lblTimeTakenValue = new System.Windows.Forms.Label();
            this.lblPercentCompleteValue = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblPercentComplete = new System.Windows.Forms.Label();
            this.lblTimeTaken = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.openDlg = new System.Windows.Forms.OpenFileDialog();
            this.hashDlg = new System.Windows.Forms.OpenFileDialog();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.processTimer = new System.Windows.Forms.Timer(this.components);
            this.btnAbout = new System.Windows.Forms.Button();
            this.gbxImage.SuspendLayout();
            this.gbxDisk.SuspendLayout();
            this.gbxHash.SuspendLayout();
            this.gbxProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxImage
            // 
            this.gbxImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxImage.Controls.Add(this.btnBrowseImage);
            this.gbxImage.Controls.Add(this.txtFilename);
            this.gbxImage.Location = new System.Drawing.Point(8, 8);
            this.gbxImage.Name = "gbxImage";
            this.gbxImage.Size = new System.Drawing.Size(424, 64);
            this.gbxImage.TabIndex = 3;
            this.gbxImage.TabStop = false;
            this.gbxImage.Text = " Image ";
            // 
            // btnBrowseImage
            // 
            this.btnBrowseImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseImage.Location = new System.Drawing.Point(384, 24);
            this.btnBrowseImage.Name = "btnBrowseImage";
            this.btnBrowseImage.Size = new System.Drawing.Size(24, 23);
            this.btnBrowseImage.TabIndex = 1;
            this.btnBrowseImage.Text = "...";
            this.btnBrowseImage.UseVisualStyleBackColor = true;
            this.btnBrowseImage.Click += new System.EventHandler(this.btnBrowseImage_Click);
            // 
            // txtFilename
            // 
            this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilename.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilename.Location = new System.Drawing.Point(16, 24);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(360, 23);
            this.txtFilename.TabIndex = 0;
            // 
            // gbxDisk
            // 
            this.gbxDisk.Controls.Add(this.ddlDevice);
            this.gbxDisk.Location = new System.Drawing.Point(8, 80);
            this.gbxDisk.Name = "gbxDisk";
            this.gbxDisk.Size = new System.Drawing.Size(424, 64);
            this.gbxDisk.TabIndex = 4;
            this.gbxDisk.TabStop = false;
            this.gbxDisk.Text = " Disk ";
            // 
            // ddlDevice
            // 
            this.ddlDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlDevice.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlDevice.FormattingEnabled = true;
            this.ddlDevice.Location = new System.Drawing.Point(16, 24);
            this.ddlDevice.Name = "ddlDevice";
            this.ddlDevice.Size = new System.Drawing.Size(392, 23);
            this.ddlDevice.TabIndex = 0;
            // 
            // gbxHash
            // 
            this.gbxHash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxHash.Controls.Add(this.btnClearHash);
            this.gbxHash.Controls.Add(this.btnBrowseHash);
            this.gbxHash.Controls.Add(this.txtHash);
            this.gbxHash.Location = new System.Drawing.Point(8, 152);
            this.gbxHash.Name = "gbxHash";
            this.gbxHash.Size = new System.Drawing.Size(424, 64);
            this.gbxHash.TabIndex = 5;
            this.gbxHash.TabStop = false;
            this.gbxHash.Text = " Hash ";
            // 
            // btnClearHash
            // 
            this.btnClearHash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearHash.Location = new System.Drawing.Point(360, 24);
            this.btnClearHash.Name = "btnClearHash";
            this.btnClearHash.Size = new System.Drawing.Size(48, 23);
            this.btnClearHash.TabIndex = 2;
            this.btnClearHash.Text = "Clear";
            this.btnClearHash.UseVisualStyleBackColor = true;
            this.btnClearHash.Click += new System.EventHandler(this.btnClearHash_Click);
            // 
            // btnBrowseHash
            // 
            this.btnBrowseHash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseHash.Location = new System.Drawing.Point(336, 24);
            this.btnBrowseHash.Name = "btnBrowseHash";
            this.btnBrowseHash.Size = new System.Drawing.Size(24, 23);
            this.btnBrowseHash.TabIndex = 1;
            this.btnBrowseHash.Text = "...";
            this.btnBrowseHash.UseVisualStyleBackColor = true;
            this.btnBrowseHash.Click += new System.EventHandler(this.btnBrowseHash_Click);
            // 
            // txtHash
            // 
            this.txtHash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHash.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHash.Location = new System.Drawing.Point(16, 24);
            this.txtHash.Name = "txtHash";
            this.txtHash.ReadOnly = true;
            this.txtHash.Size = new System.Drawing.Size(312, 23);
            this.txtHash.TabIndex = 0;
            // 
            // gbxProgress
            // 
            this.gbxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProgress.Controls.Add(this.lblStatus);
            this.gbxProgress.Controls.Add(this.lblSpeedValue);
            this.gbxProgress.Controls.Add(this.lblTimeTakenValue);
            this.gbxProgress.Controls.Add(this.lblPercentCompleteValue);
            this.gbxProgress.Controls.Add(this.lblSpeed);
            this.gbxProgress.Controls.Add(this.lblPercentComplete);
            this.gbxProgress.Controls.Add(this.lblTimeTaken);
            this.gbxProgress.Controls.Add(this.progressBar);
            this.gbxProgress.Location = new System.Drawing.Point(8, 224);
            this.gbxProgress.Name = "gbxProgress";
            this.gbxProgress.Size = new System.Drawing.Size(424, 168);
            this.gbxProgress.TabIndex = 6;
            this.gbxProgress.TabStop = false;
            this.gbxProgress.Text = " Progress ";
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(16, 24);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(392, 23);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "???";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpeedValue
            // 
            this.lblSpeedValue.AutoSize = true;
            this.lblSpeedValue.Location = new System.Drawing.Point(128, 136);
            this.lblSpeedValue.Name = "lblSpeedValue";
            this.lblSpeedValue.Size = new System.Drawing.Size(25, 13);
            this.lblSpeedValue.TabIndex = 13;
            this.lblSpeedValue.Text = "???";
            // 
            // lblTimeTakenValue
            // 
            this.lblTimeTakenValue.AutoSize = true;
            this.lblTimeTakenValue.Location = new System.Drawing.Point(128, 88);
            this.lblTimeTakenValue.Name = "lblTimeTakenValue";
            this.lblTimeTakenValue.Size = new System.Drawing.Size(25, 13);
            this.lblTimeTakenValue.TabIndex = 11;
            this.lblTimeTakenValue.Text = "???";
            // 
            // lblPercentCompleteValue
            // 
            this.lblPercentCompleteValue.AutoSize = true;
            this.lblPercentCompleteValue.Location = new System.Drawing.Point(128, 112);
            this.lblPercentCompleteValue.Name = "lblPercentCompleteValue";
            this.lblPercentCompleteValue.Size = new System.Drawing.Size(25, 13);
            this.lblPercentCompleteValue.TabIndex = 10;
            this.lblPercentCompleteValue.Text = "???";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(16, 136);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(41, 13);
            this.lblSpeed.TabIndex = 8;
            this.lblSpeed.Text = "Speed:";
            // 
            // lblPercentComplete
            // 
            this.lblPercentComplete.AutoSize = true;
            this.lblPercentComplete.Location = new System.Drawing.Point(16, 112);
            this.lblPercentComplete.Name = "lblPercentComplete";
            this.lblPercentComplete.Size = new System.Drawing.Size(94, 13);
            this.lblPercentComplete.TabIndex = 7;
            this.lblPercentComplete.Text = "Percent Complete:";
            // 
            // lblTimeTaken
            // 
            this.lblTimeTaken.AutoSize = true;
            this.lblTimeTaken.Location = new System.Drawing.Point(16, 88);
            this.lblTimeTaken.Name = "lblTimeTaken";
            this.lblTimeTaken.Size = new System.Drawing.Size(67, 13);
            this.lblTimeTaken.TabIndex = 6;
            this.lblTimeTaken.Text = "Time Taken:";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(16, 48);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(392, 24);
            this.progressBar.TabIndex = 4;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(360, 400);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(72, 23);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(88, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(8, 400);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(75, 23);
            this.btnWrite.TabIndex = 9;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // processTimer
            // 
            this.processTimer.Interval = 1000;
            this.processTimer.Tick += new System.EventHandler(this.processTimer_Tick);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(280, 400);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(72, 23);
            this.btnAbout.TabIndex = 12;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 433);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.gbxProgress);
            this.Controls.Add(this.gbxHash);
            this.Controls.Add(this.gbxDisk);
            this.Controls.Add(this.gbxImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Disk Image Writer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gbxImage.ResumeLayout(false);
            this.gbxImage.PerformLayout();
            this.gbxDisk.ResumeLayout(false);
            this.gbxHash.ResumeLayout(false);
            this.gbxHash.PerformLayout();
            this.gbxProgress.ResumeLayout(false);
            this.gbxProgress.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxImage;
        private System.Windows.Forms.Button btnBrowseImage;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.GroupBox gbxDisk;
        private System.Windows.Forms.ComboBox ddlDevice;
        private System.Windows.Forms.GroupBox gbxHash;
        private System.Windows.Forms.Button btnBrowseHash;
        private System.Windows.Forms.TextBox txtHash;
        private System.Windows.Forms.GroupBox gbxProgress;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblSpeedValue;
        private System.Windows.Forms.Label lblTimeTakenValue;
        private System.Windows.Forms.Label lblPercentCompleteValue;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblPercentComplete;
        private System.Windows.Forms.Label lblTimeTaken;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.OpenFileDialog openDlg;
        private System.Windows.Forms.OpenFileDialog hashDlg;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer processTimer;
        private System.Windows.Forms.Button btnClearHash;
        private System.Windows.Forms.Button btnAbout;
    }
}

