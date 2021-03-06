﻿namespace SimpleBackup
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
            this.m_tabMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.m_pnlMain = new System.Windows.Forms.Panel();
            this.m_ToolStripMain = new System.Windows.Forms.ToolStrip();
            this.m_btnNewGroup = new System.Windows.Forms.ToolStripButton();
            this.m_btnRenameGroup = new System.Windows.Forms.ToolStripButton();
            this.m_btnDeleteGroup = new System.Windows.Forms.ToolStripButton();
            this.m_tabMain.SuspendLayout();
            this.m_pnlMain.SuspendLayout();
            this.m_ToolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_tabMain
            // 
            this.m_tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tabMain.Controls.Add(this.tabPage1);
            this.m_tabMain.Controls.Add(this.tabPage2);
            this.m_tabMain.ImageList = this.imageList1;
            this.m_tabMain.Location = new System.Drawing.Point(0, 3);
            this.m_tabMain.Name = "m_tabMain";
            this.m_tabMain.SelectedIndex = 0;
            this.m_tabMain.Size = new System.Drawing.Size(800, 413);
            this.m_tabMain.TabIndex = 1;
            this.m_tabMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_tabMain_MouseDoubleClick);
            // 
            // tabPage1
            // 
            this.tabPage1.ImageIndex = 5;
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 386);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.ImageIndex = 3;
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 386);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Blacklisted.ico");
            this.imageList1.Images.SetKeyName(1, "Shared.ico");
            this.imageList1.Images.SetKeyName(2, "Syncing.ico");
            this.imageList1.Images.SetKeyName(3, "Provider.ico");
            this.imageList1.Images.SetKeyName(4, "propertysheets.ico");
            this.imageList1.Images.SetKeyName(5, "computer.ico");
            // 
            // m_pnlMain
            // 
            this.m_pnlMain.Controls.Add(this.m_tabMain);
            this.m_pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_pnlMain.Location = new System.Drawing.Point(0, 25);
            this.m_pnlMain.Name = "m_pnlMain";
            this.m_pnlMain.Size = new System.Drawing.Size(800, 416);
            this.m_pnlMain.TabIndex = 5;
            // 
            // m_ToolStripMain
            // 
            this.m_ToolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_ToolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_btnNewGroup,
            this.m_btnRenameGroup,
            this.m_btnDeleteGroup});
            this.m_ToolStripMain.Location = new System.Drawing.Point(0, 0);
            this.m_ToolStripMain.Name = "m_ToolStripMain";
            this.m_ToolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.m_ToolStripMain.Size = new System.Drawing.Size(800, 25);
            this.m_ToolStripMain.TabIndex = 6;
            this.m_ToolStripMain.Text = "toolStrip1";
            // 
            // m_btnNewGroup
            // 
            this.m_btnNewGroup.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNewGroup.Image")));
            this.m_btnNewGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNewGroup.Name = "m_btnNewGroup";
            this.m_btnNewGroup.Size = new System.Drawing.Size(87, 22);
            this.m_btnNewGroup.Text = "New Group";
            this.m_btnNewGroup.Click += new System.EventHandler(this.m_btnAdd_Click);
            // 
            // m_btnRenameGroup
            // 
            this.m_btnRenameGroup.Image = ((System.Drawing.Image)(resources.GetObject("m_btnRenameGroup.Image")));
            this.m_btnRenameGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnRenameGroup.Name = "m_btnRenameGroup";
            this.m_btnRenameGroup.Size = new System.Drawing.Size(106, 22);
            this.m_btnRenameGroup.Text = "Rename Group";
            this.m_btnRenameGroup.Click += new System.EventHandler(this.m_btnEdit_Click);
            // 
            // m_btnDeleteGroup
            // 
            this.m_btnDeleteGroup.Image = ((System.Drawing.Image)(resources.GetObject("m_btnDeleteGroup.Image")));
            this.m_btnDeleteGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnDeleteGroup.Name = "m_btnDeleteGroup";
            this.m_btnDeleteGroup.Size = new System.Drawing.Size(96, 22);
            this.m_btnDeleteGroup.Text = "Delete Group";
            this.m_btnDeleteGroup.Click += new System.EventHandler(this.m_btnRemove_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(800, 441);
            this.Controls.Add(this.m_pnlMain);
            this.Controls.Add(this.m_ToolStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simple Backup";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.m_tabMain.ResumeLayout(false);
            this.m_pnlMain.ResumeLayout(false);
            this.m_ToolStripMain.ResumeLayout(false);
            this.m_ToolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl m_tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel m_pnlMain;
        private System.Windows.Forms.ToolStrip m_ToolStripMain;
        private System.Windows.Forms.ToolStripButton m_btnNewGroup;
        private System.Windows.Forms.ToolStripButton m_btnRenameGroup;
        private System.Windows.Forms.ToolStripButton m_btnDeleteGroup;
    }
}

