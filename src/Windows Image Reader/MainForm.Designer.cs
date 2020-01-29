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
            this.gbxDisk = new System.Windows.Forms.GroupBox();
            this.ddlDevice = new System.Windows.Forms.ComboBox();
            this.gbxImage = new System.Windows.Forms.GroupBox();
            this.btnBrowseImage = new System.Windows.Forms.Button();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.gbxProgress = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblSpeedValue = new System.Windows.Forms.Label();
            this.lblTimeTakenValue = new System.Windows.Forms.Label();
            this.lblPercentCompleteValue = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblPercentComplete = new System.Windows.Forms.Label();
            this.lblTimeTaken = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.gbxOptions = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ddlCompression = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlHash = new System.Windows.Forms.ComboBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.saveDlg = new System.Windows.Forms.SaveFileDialog();
            this.processTimer = new System.Windows.Forms.Timer(this.components);
            this.btnAbout = new System.Windows.Forms.Button();
            this.gbxDisk.SuspendLayout();
            this.gbxImage.SuspendLayout();
            this.gbxProgress.SuspendLayout();
            this.gbxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxDisk
            // 
            this.gbxDisk.Controls.Add(this.ddlDevice);
            this.gbxDisk.Location = new System.Drawing.Point(8, 8);
            this.gbxDisk.Name = "gbxDisk";
            this.gbxDisk.Size = new System.Drawing.Size(424, 64);
            this.gbxDisk.TabIndex = 1;
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
            // gbxImage
            // 
            this.gbxImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxImage.Controls.Add(this.btnBrowseImage);
            this.gbxImage.Controls.Add(this.txtFilename);
            this.gbxImage.Location = new System.Drawing.Point(8, 80);
            this.gbxImage.Name = "gbxImage";
            this.gbxImage.Size = new System.Drawing.Size(424, 64);
            this.gbxImage.TabIndex = 2;
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
            this.gbxProgress.TabIndex = 5;
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
            // gbxOptions
            // 
            this.gbxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxOptions.Controls.Add(this.label2);
            this.gbxOptions.Controls.Add(this.ddlCompression);
            this.gbxOptions.Controls.Add(this.label1);
            this.gbxOptions.Controls.Add(this.ddlHash);
            this.gbxOptions.Location = new System.Drawing.Point(8, 152);
            this.gbxOptions.Name = "gbxOptions";
            this.gbxOptions.Size = new System.Drawing.Size(424, 64);
            this.gbxOptions.TabIndex = 4;
            this.gbxOptions.TabStop = false;
            this.gbxOptions.Text = " Options ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Compression:";
            // 
            // ddlCompression
            // 
            this.ddlCompression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCompression.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlCompression.FormattingEnabled = true;
            this.ddlCompression.Items.AddRange(new object[] {
            "None",
            "GZip"});
            this.ddlCompression.Location = new System.Drawing.Point(304, 24);
            this.ddlCompression.Name = "ddlCompression";
            this.ddlCompression.Size = new System.Drawing.Size(104, 23);
            this.ddlCompression.TabIndex = 4;
            this.ddlCompression.SelectedIndexChanged += new System.EventHandler(this.ddlCompression_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Hash:";
            // 
            // ddlHash
            // 
            this.ddlHash.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlHash.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlHash.FormattingEnabled = true;
            this.ddlHash.Items.AddRange(new object[] {
            "None",
            "MD5",
            "SHA1",
            "SHA256",
            "SHA512"});
            this.ddlHash.Location = new System.Drawing.Point(56, 24);
            this.ddlHash.Name = "ddlHash";
            this.ddlHash.Size = new System.Drawing.Size(104, 23);
            this.ddlHash.TabIndex = 1;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(8, 400);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 6;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(88, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(360, 400);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(72, 23);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
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
            this.btnAbout.TabIndex = 9;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 434);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.gbxProgress);
            this.Controls.Add(this.gbxOptions);
            this.Controls.Add(this.gbxImage);
            this.Controls.Add(this.gbxDisk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Disk Image Reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gbxDisk.ResumeLayout(false);
            this.gbxImage.ResumeLayout(false);
            this.gbxImage.PerformLayout();
            this.gbxProgress.ResumeLayout(false);
            this.gbxProgress.PerformLayout();
            this.gbxOptions.ResumeLayout(false);
            this.gbxOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxDisk;
        private System.Windows.Forms.ComboBox ddlDevice;
        private System.Windows.Forms.GroupBox gbxImage;
        private System.Windows.Forms.Button btnBrowseImage;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.GroupBox gbxProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox gbxOptions;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ddlCompression;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlHash;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.SaveFileDialog saveDlg;
        private System.Windows.Forms.Timer processTimer;
        private System.Windows.Forms.Label lblTimeTakenValue;
        private System.Windows.Forms.Label lblPercentCompleteValue;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblPercentComplete;
        private System.Windows.Forms.Label lblTimeTaken;
        private System.Windows.Forms.Label lblSpeedValue;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnAbout;
    }
}

