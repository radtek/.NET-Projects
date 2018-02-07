﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace MeditationStopWatch
{
	public partial class FormStopWatch : Form
	{
		private Options m_Options = new Options();
		private string m_sSettingsFile = "MeditationStopWatch.mz";
		private IList<string> m_FavoritesList = new List<string>();
		ImageFileUtil ImageInfo = new ImageFileUtil();
		ThumbnailCache m_ThumbnailCache;

		public FormStopWatch()
		{
			InitializeComponent();

			//designer problems
			this.m_splitContainerMain.Panel2MinSize = 220;

			string sDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			m_sSettingsFile = sDirectory + "\\" + m_sSettingsFile;

			m_ThumbnailCache = new ThumbnailCache(m_imageListThumbnails);
			m_ThumbnailCache.ProgressChanged += m_ThumbnailCache_ProgressChanged;
		}

		private void m_ThumbnailCache_ProgressChanged(object sender, CacheEventArgs e)
		{
			UpdateStatus(e.Percent, e.Status);
		}

		private void UpdateStatus(int percent, string status)
		{
			if (this.Disposing || this.IsDisposed) return;

			if (this.InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(delegate() { UpdateStatus(percent, status); }));
			}
			else
			{
				if (m_ThumbnailCache.CancelLoadingThumbnails)
				{
					m_toolStripStatusLabel1.Text = "Ready";
					m_toolStripStatusLabel2.Text = "";
					//m_toolStripProgressBar1.Value = 0;
					return;
				}

				m_toolStripStatusLabel1.Text = status;
				if (percent > 0)
					m_toolStripStatusLabel2.Text =
						percent + " % (" + percent * ImageInfo.AllImages.Count / 100 + " of " + ImageInfo.AllImages.Count + ")";
				else
					m_toolStripStatusLabel2.Text = "";

				m_toolStripProgressBar1.Value = percent;

                m_listThumbnails.Invalidate();
				
				Application.DoEvents();
			}
		}

		private void FormStopWatch_Load(object sender, EventArgs e)
		{
			if ( File.Exists(m_sSettingsFile) )
				OptionsSerializer.Load(m_sSettingsFile, m_Options);

			InitializeFavorites();
			ApplyOptions();

            //restore position
            if (m_Options.AppRectangle != null)
            {
                if(System.Windows.SystemParameters.VirtualScreenLeft < m_Options.AppRectangle.Location.X)
                    this.Location = m_Options.AppRectangle.Location;
                this.Size = m_Options.AppRectangle.Size;
            }

			if (m_Options.WindowState != FormWindowState.Minimized)
				WindowState = m_Options.WindowState;

			if (m_Options.PictureWidth > 25)
				m_splitContainerMain.SplitterDistance = m_Options.PictureWidth;

			if (m_Options.ClockHeight > 25)
				m_splitContainer.SplitterDistance = m_Options.ClockHeight;
			
			this.Visible = true;
			OpenImageDirectory(m_Options.LastImageFile);

			m_audioPlayerControl.AddToFileList(m_Options.PlayList, false);
			m_audioPlayerControl.ValueChanged += m_audioPlayerControl_ValueChanged;
		}

		private void FormStopWatch_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_ThumbnailCache.ProgressChanged -= m_ThumbnailCache_ProgressChanged;
			m_audioPlayerControl.ValueChanged -= m_audioPlayerControl_ValueChanged;
			m_ThumbnailCache.CancelLoadingThumbnails = true;
			
			SaveOptions();
		}

		private void FormStopWatch_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private int _iImageTimeoutCount = 0;
		private void m_audioPlayerControl_ValueChanged(object sender, EventArgs e)
		{
			//m_lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
			if ( _iImageTimeoutCount++ % m_Options.SlideShowTimeOut == 0) //every 6 seconds
			{
				if (m_btnSlideShow.Checked)
					m_btnNextImage_Click(sender, e);
			}
		}

		private void FormStopWatch_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void FormStopWatch_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop, true) != true)
				return;

			try
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, true);
				AddToFileList(files, true);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Error in DragDrop function: " + ex.Message);

				// don't show MessageBox here - Explorer is waiting !

			}
		}

		private void AddToFileList(string[] files, bool bPlayFirst)
		{
			m_audioPlayerControl.AddToFileList(files, bPlayFirst);
		}

		private void m_btnOpenImage_Click(object sender, EventArgs e)
		{
			if (m_openFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			OpenImageDirectory(m_openFileDialog.FileName);
		}

		private void OpenImageDirectory(string sFileName)
		{
			if (string.IsNullOrEmpty(sFileName) || !File.Exists(sFileName))
				return;

			ImageInfo.OpenImageDirectory(sFileName);
			m_listThumbnails.VirtualListSize = ImageInfo.AllImages.Count;
			OpenImage(ImageInfo.ImageInfo);

			m_ThumbnailCache.CancelLoadingThumbnails = true;
			m_ThumbnailCache.InitCache(ImageInfo.AllImages);

			EnsureVisible();
			UpdateStatusText();
		}


		private void OpenImage(FileInfo image)
		{
			_iImageTimeoutCount = 1;
			m_Options.LastImageFile = image.FullName;
			m_pictureBox1.Load(image.FullName);
		}

		private void m_mnuFile_Open_Click(object sender, EventArgs e)
		{
			m_btnOpenImage_Click(sender, e);
		}

		private void m_mnuFile_Exit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void m_mnuTools_Options_Click(object sender, EventArgs e)
		{
			m_Options.Volume = m_audioPlayerControl.Volume;
			m_Options.Loop = m_audioPlayerControl.Loop;
			
			FormOptions frm = new FormOptions(m_Options);
			frm.ShowDialog(this);
			
			ApplyOptions();
			SaveOptions();
		}

		private void m_mnuHelp_About_Click(object sender, EventArgs e)
		{
			FormAbout frm = new FormAbout();
			frm.ShowDialog(this);
		}

		private void ApplyOptions()
		{
			//m_pnlClock.BackColor = m_Options.ClockBackground;
			
			m_analogClock.BackColor = m_Options.ClockBackground;
			m_analogClock.HourHandColor = m_Options.HourHandColor;
			m_analogClock.MinuteHandColor = m_Options.MinuteHandColor;
			m_analogClock.SecondHandColor = m_Options.SecondHandColor;
			m_analogClock.TicksColor = m_Options.TicksColor;

            digitalClockCtrl1.ForeColor = m_Options.DigitalClockTextColor;
            digitalClockCtrl1.BackColor = m_Options.DigitalClockBackColor;
            digitalClockCtrl1.Font = m_Options.DigitalClockFont;


            m_audioPlayerControl.Options = m_Options;
		}

		private void SaveOptions()
		{
			m_Options.FavoritesList = m_FavoritesList.ToArray<string>();
			m_Options.PlayList = m_audioPlayerControl.PlayList;
			m_Options.Volume = m_audioPlayerControl.Volume;
			m_Options.Loop = m_audioPlayerControl.Loop;

			if (WindowState != FormWindowState.Minimized)
				m_Options.WindowState = WindowState;
			else
				m_Options.WindowState = FormWindowState.Normal;

            m_Options.AppRectangle = new Rectangle(this.Location, this.Size);

			m_Options.PictureWidth = m_splitContainerMain.SplitterDistance;
			m_Options.ClockHeight = m_splitContainer.SplitterDistance;

            OptionsSerializer.Save(m_sSettingsFile, m_Options);
		}

		private void InitializeFavorites()
		{
			if (m_Options.FavoritesList == null)
				return;

			m_FavoritesList = new List<string>(m_Options.FavoritesList);
			foreach (string file in m_FavoritesList)
			{
				AppendFavoritesMenu(file);
			}
		}

		private void AppendFavoritesMenu(string file)
		{
			ToolStripMenuItem mnu = new ToolStripMenuItem(file);
			mnu.Click += FavoriteItem_Click;
			m_mnuFavorites.DropDownItems.Add(mnu);
		}

		private void FavoriteItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem mnu = sender as ToolStripMenuItem;
			m_audioPlayerControl.AddToFileList(new string [] {mnu.Text}, true);
		}

		private void m_mnuFavorites_Add_Click(object sender, EventArgs e)
		{
			if (m_audioPlayerControl.PlayingFile == null)
				return;

			string add_name = m_audioPlayerControl.PlayingFile.FullName;

			foreach (string file in m_FavoritesList)
			{
				if (file == add_name)
					return; //already exists
			}

			m_FavoritesList.Add(add_name);
			AppendFavoritesMenu(add_name);
		}

		private void m_mnuFavorites_Organize_Click(object sender, EventArgs e)
		{

		}

		private void m_pictureBox1_Click(object sender, EventArgs e)
		{
			m_audioPlayerControl.Pause();
			m_pictureBox1.Focus();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			AdjustVolume(e.Delta);
			base.OnMouseWheel(e);
		}

		private void AdjustVolume(int delta)
		{
			delta = delta / 120;

			int vol = m_audioPlayerControl.Volume;
			if (vol > 100) delta *= 40;
			else delta *= 20;
			vol += delta;
			if (vol < 0) vol = 0;
			if (vol > 1000) vol = 1000;
			System.Diagnostics.Trace.WriteLine("Delta: " + delta + " Vol: " + vol);
			m_audioPlayerControl.Volume = vol;
		}

		private void m_btnPrevImage_Click(object sender, EventArgs e)
		{
			OpenImage(ImageInfo.Prev());
			EnsureVisible();
			UpdateStatusText();
		}

		private void m_btnNextImage_Click(object sender, EventArgs e)
		{
			OpenImage(ImageInfo.Next());
			EnsureVisible();
			UpdateStatusText();
		}

		private void UpdateStatusText()
		{
			int idx = ImageInfo.IndexOf(ImageInfo.ImageInfo);
			m_lblImageDesc.Text = string.Format("{0} of {1}", idx + 1, ImageInfo.AllImages.Count);
			m_txtImageIndex.Text = string.Format("{0}", idx + 1);
			//m_lblFileName.Text = ImageInfo.ImageInfo.FullName;
			//m_lblFileName.Visible = m_Options.ShowFileName;
			m_tsTxt_FileName.Text = ImageInfo.ImageInfo.FullName;
			m_toolStrip_Picture.Visible = m_Options.ShowFileName; ;
		}

		private void EnsureVisible()
		{
			int idx = ImageInfo.IndexOf(ImageInfo.ImageInfo);
			m_listThumbnails.Items[idx].Selected = true;
			m_listThumbnails.EnsureVisible(idx);
		}

		private void m_btnSlideShow_Click(object sender, EventArgs e)
		{
			m_btnSlideShow.Checked = !m_btnSlideShow.Checked;
		}

		private void m_listThumbnails_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
				int imgIdx = m_ThumbnailCache.GetImageIdx(e.ItemIndex);
				e.Item = new ListViewItem((e.ItemIndex + 1) + ". " + ImageInfo.AllImages[e.ItemIndex].Name, imgIdx);
		}

		//int m_iFirstVisibleThumb = -1;
		//bool m_bRetrievingItem = false;
		//private void GetFirstVisibleThumbnailIdx(int idx)
		//{
		//    if (m_bRetrievingItem == false)
		//    {
		//        m_bRetrievingItem = true;
		//        m_listThumbnails.GetItemAt(10, 10);
		//        m_bRetrievingItem = false;
		//    }
		//    else
		//    {
		//        System.Diagnostics.Trace.WriteLine("First: " + idx);
		//        m_iFirstVisibleThumb = idx;
		//    }
		//}

		private void m_listThumbnails_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_listThumbnails.SelectedIndices == null || m_listThumbnails.SelectedIndices.Count == 0)
				return;

			int idx = m_listThumbnails.SelectedIndices[0];
			ImageInfo.ImageInfo = ImageInfo.AllImages[idx];
			OpenImage(ImageInfo.ImageInfo);
			UpdateStatusText();
		}

		private void m_btnFitWindow_Click(object sender, EventArgs e)
		{

		}

		private void m_btnOrigSize_Click(object sender, EventArgs e)
		{

		}

		private void m_txtImageIndex_TextChanged(object sender, EventArgs e)
		{
			string idx = m_txtImageIndex.Text;
			if ( string.IsNullOrEmpty(idx))
				return;

			if (idx[idx.Length - 1] == '\n')
			{
			}
		}
	}
}