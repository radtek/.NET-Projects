﻿using DesktopManagerUX.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DesktopManagerUX
{
    [Serializable]
    public class Configuration
    {
        public List<DisplayConfiguration> Displays { get; set; } = new List<DisplayConfiguration>();

        public Configuration()
        {
        }

        public static string ConfigurationFileName
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                path = Path.Combine(path, "DesktopManager");
                Directory.CreateDirectory(path);
                string fileName = Path.Combine(path, "Config.xml");
                return fileName;
            }
        }

        private void UpdateDisplays()
        {
            List<DisplayInfo> info = Logic.GetDisplays();

            while (Displays.Count > info.Count)
                Displays.Remove(Displays.Last());
            while (Displays.Count < info.Count)
                Displays.Add(new DisplayConfiguration());

            Debug.Assert(info.Count == Displays.Count);

            for (int i = 0; i < info.Count; i++)
            {
                Displays[i].UpdateDisplayInfo(info[i]);
            }
        }

        public void Save()
        {
            SerializerHelper.Save<Configuration>(ConfigurationFileName, this);
        }

        public static Configuration Load()
        {
            Configuration cnf = SerializerHelper.Open<Configuration>(ConfigurationFileName);
            cnf.UpdateDisplays();
            return cnf;
        }
    }

    [Serializable]
    public class DisplayConfiguration
    {
        [XmlIgnore]
        public string Name { get { return SelectedDisplayInfo.Name; } }

        public DisplayInfo SelectedDisplayInfo { get; set; }

        private GridSizeData _gridSize = new GridSizeData() { Rows = 2, Cols = 2 };

        public GridSizeData GridSize { get { return _gridSize; } set { _gridSize = value; } }

        public System.Drawing.Size CellSize
        {
            get
            {
                int width = (int)(SelectedDisplayInfo.Bounds.Width / _gridSize.Cols);
                int height = (int)(SelectedDisplayInfo.Bounds.Height / _gridSize.Rows);
                return new System.Drawing.Size(width, height);
            }
        }

        //should be before SelectedGridSize - to avoid doubling when loading from XML
        public List<AppInfo> SelectedApps { get; set; }

        public string GetSelectedGridSizeText()
        {
            return GridSize.ToString();
        }

        public void SetSelectedgridSizeText(string txtGridSize)
        {
            UpdateApps(GridSizeData.Parse(txtGridSize));
        }

        public void UpdateApps(GridSizeData newGridSize)
        {
            if (SelectedApps.Count > 0)
            {
                if (newGridSize.Rows != _gridSize.Rows || newGridSize.Cols != _gridSize.Cols)
                {
                    //init
                    List<AppInfo> list = new List<AppInfo>();
                    for (int i = 0; i < newGridSize.CellCount; i++)
                        list.Add(AppInfo.GetEmptyAppInfo());

                    //smart copy
                    for (int row = 0; row < _gridSize.Rows; row++) //copy maximum possible settings
                    {
                        for (int col = 0; col < _gridSize.Cols; col++)
                        {
                            if (row < newGridSize.Rows && col < newGridSize.Cols)
                                list[newGridSize.Pos(row, col)] = this[row, col];
                        }
                    }
                    SelectedApps = list;
                }
            }

            _gridSize = newGridSize;
        }

        public void UpdateDisplayInfo(DisplayInfo displayInfo)
        {
            SelectedDisplayInfo = displayInfo;

            if(SelectedApps.Count == 0) //not loaded from config file
                for (int i = 0; i < _gridSize.CellCount; i++)
                    SelectedApps.Add(AppInfo.GetEmptyAppInfo());
        }

        public AppInfo this[int row, int col]
        {
            get { return SelectedApps[GridSize.Pos(row, col)]; }
            set
            {
                AppInfo info = value;
                if (info == null)
                    info = AppInfo.GetEmptyAppInfo();

                int pos = GridSize.Pos(row, col);
                SelectedApps[pos] = info;
                SelectedApps[pos].Row = row;
                SelectedApps[pos].Col = col;
            }
        }

        public DisplayConfiguration()
        {
            SelectedApps = new List<AppInfo>(GridSize.CellCount);
            //for (int i = 0; i < Rows*Columns; i++)
            //    SelectedApps.Add(AppInfo.GetEmptyAppInfo());
        }
    }
}