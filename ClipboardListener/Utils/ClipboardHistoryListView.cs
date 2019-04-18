﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipboardManager.Utils
{
    public class ClipboardHistoryListView : ListView
    {
        private List<ClipboardList.ClipboardEntry> _history = new List<ClipboardList.ClipboardEntry>();

        public ClipboardHistoryListView()
        {
            this.DoubleBuffered = true;

            this.VirtualMode = true;
            this.View = View.Details;
            this.GridLines = true;
            this.FullRowSelect = true;
            this.LabelEdit = false;
            this.ShowItemToolTips = true;

            ColumnHeader columnHeader1 = new ColumnHeader();
            columnHeader1.Text = @"Size";
            columnHeader1.Width = 60;
            columnHeader1.TextAlign = HorizontalAlignment.Right;
            this.Columns.Add(columnHeader1);

            ColumnHeader columnHeader2 = new ColumnHeader();
            columnHeader2.Text = @"Clipboard History";
            columnHeader2.Width = 400;
            this.Columns.Add(columnHeader2);

            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            this.SizeChanged += ClipboardHistoryListView_SizeChanged;
            this.RetrieveVirtualItem += ClipboardHistoryListView_RetrieveVirtualItem;
            this.DoubleClick += ClipboardHistoryListView_DoubleClick;
        }

        private void ClipboardHistoryListView_DoubleClick(object sender, EventArgs e)
        {
            if (this.SelectedIndices.Count == 0)
                return;
            _history[SelectedIndices[0]].Put();
        }

        private void ClipboardHistoryListView_SizeChanged(object sender, EventArgs e)
        {
            this.Columns[1].Width = -1; //this.Width - 30 - this.Columns[0].Width;
        }

        public void UpdateHistoryList(ClipboardList log, bool update = true)
        {
            if (update)
            {
                _history = new List<ClipboardList.ClipboardEntry>(log.Count);
                for (int i = 0; i < log.Count; i++)
                {
                    _history.Add(log.GetEntry(i));
                }

                this.VirtualListSize = log.Count;
                this.SelectedIndices.Clear();
                if (_history.Count > 0)
                    this.EnsureVisible(0);

                this.Columns[1].Width = -1;
                this.Refresh();
            }
        }

        private void ClipboardHistoryListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < _history.Count)
            {
                string text = _history[e.ItemIndex].ToString();
                e.Item = new ListViewItem(text.Length.ToString(), _history[e.ItemIndex]._icoItemType);
                e.Item.SubItems.Add(text);
            }
        }
    }
}
