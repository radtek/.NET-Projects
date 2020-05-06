﻿using SmartBackup.Settings;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartBackup
{
    internal enum BackupStatus : int
    {
        None = 0,
        InProgress = 1,
        Done = 2,
        Error = 3
    }

    [Flags]
    internal enum BackupOption
    {
        OverwriteAll = 0,
        OverwriteAllOlder = 2,
        OverwriteNone = 4,
        OverwriteIfSizeDifferent = 8
    }

    internal class BackupFile
    {
        public string Src;
        public string Err;
        public BackupStatus Status;

        private string _dst = null;
        public string Dst 
        {
            get
            {
                if (_dst == null)
                {
                    string fileName = Path.GetFileName(Src);
                    _dst = Path.Combine(DstFolder, fileName);
                }
                return _dst;
            }
        }

        private string _dstFolder = null;
        public string DstFolder 
        {
            get
            {
                if(_dstFolder == null)
                {
                    string subFolders = FindSubFolders(_entry.FolderSrc, Src);
                    _dstFolder = Path.Combine(_entry.FolderDst, subFolders);
                }
                return _dstFolder;
            }
        }

        private FileInfo _srcIfo = null;
        public FileInfo SrcIfo 
        {
            get 
            {
                if (_srcIfo == null)
                    _srcIfo = new FileInfo(Src);
                return _srcIfo;
            } 
        }

        private FileInfo _dstIfo = null;
        public FileInfo DstIfo
        {
            get
            {
                if (_dstIfo == null)
                    _dstIfo = new FileInfo(Dst);
                return _dstIfo;
            }
            private set
            {
                _dstIfo = value;
            }
        }

        private readonly BackupEntry _entry;

        public BackupFile(string file, BackupEntry entry)
        {
            _entry = entry;

            Status = BackupStatus.None;
            Src = file;

            //string fileName = Path.GetFileName(file);
            //string subFolders = FindSubFolders(entry.FolderSrc, file);
            //DstFolder = Path.Combine(entry.FolderDst, subFolders);
            //Dst = Path.Combine(DstFolder, fileName);

            //SrcIfo = new FileInfo(Src);
            //DstIfo = new FileInfo(Dst);
        }

        public void PerformBackup(ProgressBar progress, BackupOption option = BackupOption.OverwriteAllOlder)
        {
            try
            {
                if (!DstIfo.Exists)
                {
                    Directory.CreateDirectory(DstFolder);
                }
                else //check overwrite options
                {
                    if (option.HasFlag(BackupOption.OverwriteAllOlder) && SrcIfo.CreationTimeUtc <= DstIfo.CreationTimeUtc)
                    { Status = BackupStatus.Done; return; }

                    if (option.HasFlag(BackupOption.OverwriteIfSizeDifferent) && SrcIfo.Length == DstIfo.Length)
                    { Status = BackupStatus.Done; return; }
                }

                Status = BackupStatus.InProgress;

                if (SrcIfo.Length < 1024 * 1024) //less than 1M
                    File.Copy(Src, Dst, true);
                else
                    CopyWithProgress(progress);

                DstIfo = new FileInfo(Dst);
                DstIfo.CreationTime = SrcIfo.CreationTime;
                DstIfo.LastWriteTime = SrcIfo.LastWriteTime;
                DstIfo.LastAccessTime = SrcIfo.LastAccessTime;

                Status = BackupStatus.Done;
            }
            catch (Exception err)
            {
                Err = err.Message;
                Status = BackupStatus.Error;
            }        
        }

        private void CopyWithProgress(ProgressBar progress)
        {
            int bufferSize = 1024 * 64;
            byte[] bytes = new byte[bufferSize];

            using (FileStream srcFile = new FileStream(Src, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan))
            {
                using (FileStream fileStream = new FileStream(Dst, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    int bytesRead = -1;
                    long totalWrite = 0;
                    while ((bytesRead = srcFile.Read(bytes, 0, bufferSize)) > 0)
                    {
                        fileStream.Write(bytes, 0, bytesRead);

                        totalWrite += bytesRead;
                        progress.Value = (int)(totalWrite * 100 / SrcIfo.Length);
                        Application.DoEvents();
                    }
                }
            }
        }

        private string FindSubFolders(string folderSrc, string file)
        {
            List<string> newFolders = new List<string>();

            string folder = Path.GetDirectoryName(file);
            while (folderSrc.CompareTo(folder) != 0)
            {
                newFolders.Add(Path.GetFileName(folder));
                folder = Path.GetDirectoryName(folder);
            }
            newFolders.Add(Path.GetFileName(folder));

            newFolders.Reverse();
            return Path.Combine(newFolders.ToArray());
        }
    }

    internal class BackupLogic
    {
        public List<BackupFile> FileList = new List<BackupFile>();

        public BackupLogic(BackupGroup group)
        {
            foreach (BackupEntry entry in group.BackupList)
            {
                CollectFiles(entry);
            }
        }

        private void CollectFiles(BackupEntry entry)
        {
            if (!Directory.Exists(entry.FolderSrc))
                return;

            string [] files = Directory.GetFiles(entry.FolderSrc, entry.FolderIncludeTypes, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileList.Add(new BackupFile(file, entry));
            }
        }
    }
}
