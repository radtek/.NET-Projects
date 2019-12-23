﻿using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using System.Diagnostics;
using DashCamGPSView.Tools;

namespace DashCamGPSView.Controls
{
    /// <summary>
    /// Interaction logic for ThumbnailsUserControl.xaml
    /// </summary>
    public partial class ThumbnailsUserControl : UserControl
    {
        public class ThumbnailData
        {
            public BitmapSource bmp { get; set; }
            public string txt { get; set; }
            public double seconds { get; set; }
            public double width { get; set; } = 160;
            public double height { get; set; } = 120;

            public ThumbnailData(BitmapSource image, double sec)
            {
                bmp = image;
                txt = TimeSpan.FromSeconds(sec).ToString();
                seconds = sec;
            }
        }

        public Action<ThumbnailData> OnItemSelectedAction = (item) => { };

        public ObservableCollection<ThumbnailData> Images { get; set; } = new ObservableCollection<ThumbnailData>();

        public ThumbnailsUserControl()
        {
            InitializeComponent();

            DataContext = this;

            player.MediaOpened += Player_MediaOpened;
            player.MediaFailed += Player_MediaFailed;
        }

        private bool _selectFromThis = false;
        public void SelectItem(double second)
        {
            if (_selectFromThis)
                return;
            _selectFromThis = true;

            int index = IndexFromSecond(second);
            if (index >= 0 && index < Images.Count)
            {
                Thumbnails.SelectedItem = Images[index];
                Thumbnails.ScrollToCenterOfView(Thumbnails.SelectedItem);
            }
            else
            {
                Thumbnails.SelectedItem = 0;
            }
            _selectFromThis = false;
        }

        private int IndexFromSecond(double second)
        {
            for (int i = 0; i < Images.Count; i++)
            {
                if (Images[i].seconds >= second)
                    return i;
            }
            return -1;
        }

        private void Thumbnails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectFromThis)
                return;
            _selectFromThis = true;
            OnItemSelectedAction(Thumbnails.SelectedItem as ThumbnailData);
            _selectFromThis = false;
        }

        private Size CalculateThumbSize()
        {
            double h = Thumbnails.ActualHeight - 36;
            double w = h * 16 / 9;
            if (h < 32 || w < 64)
                return new Size(64, 32); //min size
            if (h > 320 || w > 640)
                return new Size(640, 320); //max size
            return new Size(w, h);
        }

        private void Thumbnails_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size size = CalculateThumbSize();

            DataContext = null;
            foreach (ThumbnailData item in Images)
            {
                item.width = size.Width;
                item.height = size.Height;
            }
            DataContext = this;
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine(e.ErrorException.ToString());

            player.Position = TimeSpan.Zero;
            player.Stop();
            player.Source = null;
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                Images.Clear();
                player.Pause();
                if (player.NaturalDuration > TimeSpan.FromSeconds(8))
                {
                    player.UpdateLayout();
                    Size size = CalculateThumbSize();
                    double seconds = player.NaturalDuration.TimeSpan.TotalSeconds;
                    const int COUNT = 16; // thumbnails count
                    double interval = seconds / COUNT;

                    for (int i = 0; i < COUNT; i++)
                    {
                        double position = i * interval;
                        player.Position = TimeSpan.FromSeconds(position);
                        System.Threading.Thread.Sleep(33);
                        player.UpdateLayout();
                        Tools.Tools.ForceUIToUpdate();

                        BitmapSource bmp = Tools.Tools.UIElementToBitmap(player);
                        Images.Add(new ThumbnailData(bmp, (int)position) { width = size.Width, height = size.Height });
                    }
                    //Thumbnails_SizeChanged(sender, null);
                    this.UpdateLayout();
                }
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.ToString());
            }
            finally
            {
                player.Position = TimeSpan.Zero;
                player.Stop();
                player.Source = null;
            }
        }

        public void StartCreateThumbnailsFromVideoFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            try
            {
                Images.Clear();
                player.Source = new System.Uri(fileName);
                player.Play();
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
    }
}
