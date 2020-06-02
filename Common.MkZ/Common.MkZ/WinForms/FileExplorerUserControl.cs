﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace MZ.WinForms
{
    public partial class FileExplorerUserControl : UserControl
    {
		private class FileData
		{
			public string FullPath { get; private set; }
			public string FileName { get; private set; }
			public bool IsDirectory { get; private set; }
			public FileInfo FileInfo { get; private set; }
			public DateTime CreatedTime { get; private set; }
			public DateTime ModifiedTime { get; private set; }
			public long FileSize { get; private set; }

			public FileData(string fullPath)
			{
				Init(fullPath);
			}

			public FileData(string fullPath, bool isDirectory)
			{
				Init(fullPath, isDirectory);
			}

			public void Init(string fullPath)
			{
				Init(fullPath, Directory.Exists(fullPath));
			}

			public void Init(string fullPath, bool isDirectory)
			{ 
				FullPath = fullPath;
				IsDirectory = isDirectory;
				FileName = Path.GetFileName(fullPath);

				FileInfo = new FileInfo(fullPath);
				if(!IsDirectory)
					FileSize = FileInfo.Length;

				//check if file is in local current day light saving time
				if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(FileInfo.CreationTime) == false)
				{
					//not in day light saving time adjust time
					CreatedTime = FileInfo.CreationTime.AddHours(1);
				}
				else
				{
					//is in day light saving time adjust time
					CreatedTime = FileInfo.CreationTime;
				}

				//check if file is in local current day light saving time
				if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(FileInfo.LastWriteTime) == false)
				{
					//not in day light saving time adjust time
					ModifiedTime = FileInfo.LastWriteTime.AddHours(1);
				}
				else
				{
					//not in day light saving time adjust time
					ModifiedTime = FileInfo.LastWriteTime;
				}
			}

			//for special folder name 'up'
			internal void SetName(string name)
			{
				FileName = name;
			}
		}

		private class FileSystemWatcherHelper : IDisposable
		{
			private FileSystemWatcher _fileSystemWatcher;
			private readonly List<FileData> _list;

			public Action<string> OnChangeAction = (fullPath) => { };

			public FileSystemWatcherHelper(List<FileData> list)
			{
				_list = list;
			}

			public void StartWatching(string fullPath)
			{
				Dispose();

				_fileSystemWatcher = new FileSystemWatcher(fullPath);

				_fileSystemWatcher.Created += OnFileCreated;
				_fileSystemWatcher.Renamed += OnFileRenamed;
				_fileSystemWatcher.Deleted += OnFileDeleted;

				_fileSystemWatcher.EnableRaisingEvents = true;
			}

			public void Dispose()
			{
				if (_fileSystemWatcher != null)
				{
					_fileSystemWatcher.Created -= OnFileCreated;
					_fileSystemWatcher.Renamed -= OnFileRenamed;
					_fileSystemWatcher.Deleted -= OnFileDeleted;

					_fileSystemWatcher.EnableRaisingEvents = false;
					_fileSystemWatcher = null;
				}
			}

			private void OnFileDeleted(object sender, FileSystemEventArgs e)
			{
				FileData file = _list.FirstOrDefault(f => f.FullPath == e.FullPath);
				if (file != null)
				{
					_list.Remove(file);
					OnChangeAction(e.FullPath);
				}
			}

			private void OnFileRenamed(object sender, RenamedEventArgs e)
			{
				FileData file = _list.FirstOrDefault(f => f.FullPath == e.OldFullPath);
				if (file != null)
				{
					file.Init(e.FullPath);
					SortList();
					OnChangeAction(e.FullPath);
				}
			}

			private void OnFileCreated(object sender, FileSystemEventArgs e)
			{
				_list.Add(new FileData(e.FullPath));
				SortList();
				OnChangeAction(e.FullPath);
			}

			private void SortList() //folders first
			{
				List<FileData> folders = _list.Where(f => f.IsDirectory).ToList();
				folders.Sort((f1, f2) => string.Compare(f1.FileName, f2.FileName, true));
				
				List<FileData> files = _list.Where(f => !f.IsDirectory).ToList();
				files.Sort((f1, f2) => string.Compare(f1.FileName, f2.FileName, true));

				_list.Clear();
				_list.AddRange(folders);
				_list.AddRange(files);
			}
		}

		private readonly List<FileData> _list = new List<FileData>();
		private readonly FileSystemWatcherHelper _fileSystemWatcherHelper;

		public Action<string> OpenFolderAction = (fullPath) => { };

        public FileExplorerUserControl()
        {
            InitializeComponent();

			_fileSystemWatcherHelper = new FileSystemWatcherHelper(_list);
			_fileSystemWatcherHelper.OnChangeAction = (path) =>
			{
				MZ.Tools.CommonUtils.ExecuteOnUIThread(() => 
				{
					m_listFiles.VirtualListSize = _list.Count;
					m_listFiles.Refresh();
				}, this);
			};
		}

		private void FileExplorerUserControl_Load(object sender, EventArgs e)
        {
            InitListView();
		}

		protected void InitListView()
        {
            //init ListView control
            m_listFiles.Clear();    //clear control
			_list.Clear();
			m_listFiles.VirtualListSize = 0;

									//create column header for ListView
			m_listFiles.Columns.Add("Name", 150, System.Windows.Forms.HorizontalAlignment.Left);
            m_listFiles.Columns.Add("Size", 75, System.Windows.Forms.HorizontalAlignment.Right);
            m_listFiles.Columns.Add("Created", 140, System.Windows.Forms.HorizontalAlignment.Left);
            m_listFiles.Columns.Add("Modified", 140, System.Windows.Forms.HorizontalAlignment.Left);
        }

		private void m_btnRoot_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(m_txtPath.Text))
				return;

			string root = Path.GetPathRoot(m_txtPath.Text);
			if (Directory.Exists(root))
				PopulateFiles(root);
		}

		private void m_btnBrowse_Click(object sender, EventArgs e)
		{
			FormBrowseForFolder frm = new FormBrowseForFolder();
			frm.SelectedFolder = m_txtPath.Text;
			frm.Description = "Choose folder: ";

			if (frm.ShowDialog(this) == DialogResult.OK)
			{
				m_txtPath.Text = frm.SelectedFolder;
				PopulateFiles(m_txtPath.Text);
			}
		}

		private void m_btnUp_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(m_txtPath.Text))
				return;

			string parentFolder = Path.GetDirectoryName(m_txtPath.Text);
			if(Directory.Exists(parentFolder))
				PopulateFiles(parentFolder);
		}

		public void PopulateFiles(string fullPath)
		{
			//clear list
			InitListView();

			//check path
			if (!string.IsNullOrEmpty(fullPath) && Directory.Exists(fullPath))
			{

				try
				{
					string parentFolder = Path.GetDirectoryName(fullPath);
					if (Directory.Exists(parentFolder))
					{
						FileData parent = new FileData(parentFolder, true);
						parent.SetName("..");
						_list.Add(parent);
						m_btnUp.Enabled = true;
					}
					else
					{
						m_btnUp.Enabled = false;
					}

					string[] folders = Directory.GetDirectories(fullPath);
					List<string> dirs = folders.OrderBy(s => s).ToList();
					
					foreach (string folder in dirs)
					{
						_list.Add(new FileData(folder, true));
					}

					string[] stringFiles = Directory.GetFiles(fullPath);
					List<string> files = stringFiles.OrderBy(s => s).ToList();

					int pageSize = VisibleItemsCount();

					//loop throught all files
					for (int i = 0; i < files.Count; i++)
					{
						_list.Add(new FileData(files[i], false));

						if (i == pageSize) //after first page
						{
							//m_listFiles.SuspendLayout();
							m_listFiles.VirtualListSize = _list.Count;
							//m_listFiles.ResumeLayout();
							Application.DoEvents();
						}

						//Icon ico = Icon.ExtractAssociatedIcon(stringFile);
						//int iconIndex = m_imageListTreeView.Images.Count;
						//m_imageListTreeView.Images.Add(ico);
					}

					_fileSystemWatcherHelper.StartWatching(fullPath);

					m_listFiles.VirtualListSize = _list.Count;
					m_txtPath.Text = fullPath;
					OpenFolderAction(fullPath);
				}
				catch (IOException e)
				{
					MessageBox.Show("Error: Drive not ready or directory does not exist."+e.Message);
				}
				catch (UnauthorizedAccessException e)
				{
					MessageBox.Show("Error: Drive or directory access denided." + e.Message);
				}
				catch (Exception e)
				{
					MessageBox.Show("Error: " + e);
				}
			}
		}

		private int VisibleItemsCount()
		{
			return m_listFiles.Height / 18;
		}

		protected string formatDate(DateTime dtDate)
		{
			//Get date and time in short format
			string stringDate = "";

			stringDate = dtDate.ToShortDateString().ToString() + " " + dtDate.ToShortTimeString().ToString();

			return stringDate;
		}

		protected string formatSize(Int64 lSize)
		{
			//Format number to KB
			string stringSize = "";
			NumberFormatInfo myNfi = new NumberFormatInfo();

			Int64 lKBSize = 0;

			if (lSize < 1024)
			{
				if (lSize == 0)
				{
					//zero byte
					stringSize = "0";
				}
				else
				{
					//less than 1K but not zero byte
					stringSize = "1";
				}
			}
			else
			{
				//convert to KB
				lKBSize = lSize / 1024;
				//format number with default format
				stringSize = lKBSize.ToString("n", myNfi);
				//remove decimal
				stringSize = stringSize.Replace(".00", "");
			}

			return stringSize + " KB";

		}

		private void m_listFiles_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (m_listFiles.SelectedIndices.Count == 0)
				return;

			FileData data = _list[m_listFiles.SelectedIndices[0]];
			if (!data.IsDirectory)
				return;

			if(Directory.Exists(data.FullPath))
				PopulateFiles(data.FullPath);
		}

		private void m_listFiles_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			string[] lvData = new string[4];
			FileData data = _list[e.ItemIndex];

			lvData[0] = data.FileName;
			if(!data.IsDirectory)
				lvData[1] = formatSize(data.FileSize);
			lvData[2] = formatDate(data.CreatedTime);
			lvData[3] = formatDate(data.ModifiedTime);

			int iconIndex = data.IsDirectory ? 2 : 4;
			if (lvData[0] == "..") iconIndex = 10;

			ListViewItem lvItem = new ListViewItem(lvData, iconIndex);
			e.Item = lvItem;
		}
	}
}
