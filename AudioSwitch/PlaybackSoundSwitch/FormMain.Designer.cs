﻿namespace PlaybackSoundSwitch
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.m_imageListSpeakers = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.m_status1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_status2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_imageListMic = new System.Windows.Forms.ImageList(this.components);
            this.m_pnlMain = new System.Windows.Forms.Panel();
            this.m_splitMain = new System.Windows.Forms.SplitContainer();
            this.m_tabDevices = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.m_imageListTabs = new System.Windows.Forms.ImageList(this.components);
            this.m_txtLog = new System.Windows.Forms.RichTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.m_volumeControlSpk = new PlaybackSoundSwitch.VolumeUserControl();
            this.m_volumeControlMic = new PlaybackSoundSwitch.VolumeUserControl();
            this.m_DeviceListPlayback = new PlaybackSoundSwitch.MediaDeviceListUserControl();
            this.m_DeviceListRecording = new PlaybackSoundSwitch.MediaDeviceListUserControl();
            this.statusStrip1.SuspendLayout();
            this.m_pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitMain)).BeginInit();
            this.m_splitMain.Panel1.SuspendLayout();
            this.m_splitMain.Panel2.SuspendLayout();
            this.m_splitMain.SuspendLayout();
            this.m_tabDevices.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_imageListSpeakers
            // 
            this.m_imageListSpeakers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageListSpeakers.ImageStream")));
            this.m_imageListSpeakers.TransparentColor = System.Drawing.Color.Transparent;
            this.m_imageListSpeakers.Images.SetKeyName(0, "Speaker5_16x16.png");
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_status1,
            this.m_status2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 414);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(743, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // m_status1
            // 
            this.m_status1.Name = "m_status1";
            this.m_status1.Size = new System.Drawing.Size(39, 17);
            this.m_status1.Text = "Ready";
            // 
            // m_status2
            // 
            this.m_status2.Name = "m_status2";
            this.m_status2.Size = new System.Drawing.Size(689, 17);
            this.m_status2.Spring = true;
            this.m_status2.Text = "...";
            // 
            // m_imageListMic
            // 
            this.m_imageListMic.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageListMic.ImageStream")));
            this.m_imageListMic.TransparentColor = System.Drawing.Color.Transparent;
            this.m_imageListMic.Images.SetKeyName(0, "Mic1.png");
            this.m_imageListMic.Images.SetKeyName(1, "MicMute1.png");
            // 
            // m_pnlMain
            // 
            this.m_pnlMain.AutoScroll = true;
            this.m_pnlMain.Controls.Add(this.m_splitMain);
            this.m_pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_pnlMain.Location = new System.Drawing.Point(0, 0);
            this.m_pnlMain.Name = "m_pnlMain";
            this.m_pnlMain.Size = new System.Drawing.Size(743, 414);
            this.m_pnlMain.TabIndex = 7;
            // 
            // m_splitMain
            // 
            this.m_splitMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitMain.Location = new System.Drawing.Point(0, 0);
            this.m_splitMain.Name = "m_splitMain";
            this.m_splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // m_splitMain.Panel1
            // 
            this.m_splitMain.Panel1.Controls.Add(this.m_volumeControlSpk);
            this.m_splitMain.Panel1.Controls.Add(this.m_volumeControlMic);
            this.m_splitMain.Panel1.Controls.Add(this.m_tabDevices);
            // 
            // m_splitMain.Panel2
            // 
            this.m_splitMain.Panel2.Controls.Add(this.m_txtLog);
            this.m_splitMain.Panel2MinSize = 20;
            this.m_splitMain.Size = new System.Drawing.Size(743, 414);
            this.m_splitMain.SplitterDistance = 376;
            this.m_splitMain.TabIndex = 7;
            // 
            // m_tabDevices
            // 
            this.m_tabDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tabDevices.Controls.Add(this.tabPage1);
            this.m_tabDevices.Controls.Add(this.tabPage2);
            this.m_tabDevices.ImageList = this.m_imageListTabs;
            this.m_tabDevices.Location = new System.Drawing.Point(2, 3);
            this.m_tabDevices.Name = "m_tabDevices";
            this.m_tabDevices.SelectedIndex = 0;
            this.m_tabDevices.Size = new System.Drawing.Size(592, 368);
            this.m_tabDevices.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.m_DeviceListPlayback);
            this.tabPage1.ImageIndex = 1;
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(584, 341);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Playback Devices";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.m_DeviceListRecording);
            this.tabPage2.ImageIndex = 0;
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(584, 341);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Recording Devices";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // m_imageListTabs
            // 
            this.m_imageListTabs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageListTabs.ImageStream")));
            this.m_imageListTabs.TransparentColor = System.Drawing.Color.Transparent;
            this.m_imageListTabs.Images.SetKeyName(0, "Mic1.png");
            this.m_imageListTabs.Images.SetKeyName(1, "Spk1.png");
            // 
            // m_txtLog
            // 
            this.m_txtLog.BackColor = System.Drawing.SystemColors.Info;
            this.m_txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.m_txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_txtLog.Location = new System.Drawing.Point(0, 0);
            this.m_txtLog.Name = "m_txtLog";
            this.m_txtLog.Size = new System.Drawing.Size(741, 32);
            this.m_txtLog.TabIndex = 0;
            this.m_txtLog.Text = "";
            // 
            // m_volumeControlSpk
            // 
            this.m_volumeControlSpk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_volumeControlSpk.Level = 0;
            this.m_volumeControlSpk.Location = new System.Drawing.Point(608, 19);
            this.m_volumeControlSpk.MaximumSize = new System.Drawing.Size(58, 600);
            this.m_volumeControlSpk.MinimumSize = new System.Drawing.Size(58, 200);
            this.m_volumeControlSpk.Name = "m_volumeControlSpk";
            this.m_volumeControlSpk.Size = new System.Drawing.Size(58, 353);
            this.m_volumeControlSpk.TabIndex = 10;
            this.m_volumeControlSpk.Title = "Spk";
            this.m_volumeControlSpk.Volume = 0;
            // 
            // m_volumeControlMic
            // 
            this.m_volumeControlMic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_volumeControlMic.Level = 0;
            this.m_volumeControlMic.Location = new System.Drawing.Point(672, 19);
            this.m_volumeControlMic.MaximumSize = new System.Drawing.Size(58, 600);
            this.m_volumeControlMic.MinimumSize = new System.Drawing.Size(58, 200);
            this.m_volumeControlMic.Name = "m_volumeControlMic";
            this.m_volumeControlMic.Size = new System.Drawing.Size(58, 353);
            this.m_volumeControlMic.TabIndex = 9;
            this.m_volumeControlMic.Title = "Mic";
            this.m_volumeControlMic.Volume = 0;
            // 
            // m_DeviceListPlayback
            // 
            this.m_DeviceListPlayback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_DeviceListPlayback.Location = new System.Drawing.Point(3, 3);
            this.m_DeviceListPlayback.Name = "m_DeviceListPlayback";
            this.m_DeviceListPlayback.Size = new System.Drawing.Size(578, 335);
            this.m_DeviceListPlayback.TabIndex = 0;
            // 
            // m_DeviceListRecording
            // 
            this.m_DeviceListRecording.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_DeviceListRecording.Location = new System.Drawing.Point(3, 3);
            this.m_DeviceListRecording.Name = "m_DeviceListRecording";
            this.m_DeviceListRecording.Size = new System.Drawing.Size(578, 335);
            this.m_DeviceListRecording.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(743, 436);
            this.Controls.Add(this.m_pnlMain);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(660, 360);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Active Audio End Point";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.m_pnlMain.ResumeLayout(false);
            this.m_splitMain.Panel1.ResumeLayout(false);
            this.m_splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitMain)).EndInit();
            this.m_splitMain.ResumeLayout(false);
            this.m_tabDevices.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList m_imageListSpeakers;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel m_status1;
        private System.Windows.Forms.ToolStripStatusLabel m_status2;
        private System.Windows.Forms.SplitContainer m_splitMain;
        private System.Windows.Forms.RichTextBox m_txtLog;
        private System.Windows.Forms.Panel m_pnlMain;
        private System.Windows.Forms.ImageList m_imageListMic;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabControl m_tabDevices;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private MediaDeviceListUserControl m_DeviceListPlayback;
        private MediaDeviceListUserControl m_DeviceListRecording;
        private System.Windows.Forms.ImageList m_imageListTabs;
        private VolumeUserControl m_volumeControlSpk;
        private VolumeUserControl m_volumeControlMic;
    }
}

