﻿using DashCamGPSView.Properties;
using DashCamGPSView.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DashCamGPSView
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl, INotifyPropertyChanged
    {
        private ScrollDragZoom _scrollDragger;

        public Action MaximizeAction = () => { };
        public Action VideoEnded = () => { };
        public Action VideoStarted = () => { };
        public Func<ExceptionRoutedEventArgs, MediaElement, bool> VideoFailed = (e, player) => true;
        public Action LeftButtonClick = () => { };
        public Action LeftButtonDoubleClick = () => { };

        public MediaState MediaState { get; private set; } = MediaState.Manual;

        public string Title { get { return txtTitle.Text;  } set { txtTitle.Text = value; } }

        public string FileName { get; private set; }

        MediaElement mePlayer = null;

        public VideoPlayer()
        {
            InitializeComponent();
            RecreateMediaElement(false);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        public void CopyState(VideoPlayer player, double volume, bool copySource)
        {
            if(copySource)
                mePlayer.Source = player.mePlayer.Source;

            Volume = volume;
            MediaState = player.MediaState;
            Title = player.Title;
            FileName = player.FileName;

            Play();
            
            Position = player.Position;
            if (player.MediaState == MediaState.Stop)
                Stop();
            if (player.MediaState == MediaState.Pause)
                Pause();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                LeftButtonDoubleClick();
        }

        private void UserControl_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && (e.OriginalSource is MediaElement))
                LeftButtonClick();
        }

        public bool Play_CanExecute 
        { 
            get
            {
                return (mePlayer != null) && (mePlayer.Source != null);
            } 
        }

        public double Volume
        {
            get { return mePlayer.Volume; }
            set { mePlayer.Volume = value; OnPropertyChanged(); }
        }

        public TimeSpan Position 
        { 
            get { return mePlayer.Position; }
            set { mePlayer.Position = value; OnPropertyChanged(); } 
        }

        public Size NaturalSize
        {
            get
            {
                if (mePlayer.Source != null)
                {
                    return new Size(mePlayer.NaturalVideoWidth, mePlayer.NaturalVideoHeight);
                }
                return new Size(1920, 1080);
            }
        }

        public double NaturalDuration
        {
            get
            {
                if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan))
                {
                    return mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                }
                return 0;
            }
        }

        public void ScrollToCenter()
        {
            _scrollDragger.ScrollToCenter();
        }

        public void Open(string fileName, double volume = 0)
        {
            FileName = fileName;
            mePlayer.Source = string.IsNullOrEmpty(fileName)? null : new Uri(fileName);
            Volume = volume;
            MediaState = MediaState.Manual;
        }

        internal void Close()
        {
            Stop();
            mePlayer.Source = null;
            FileName = "";
            this.Background = Brushes.Wheat;
            MediaState = MediaState.Close;
        }

        public void Play() 
        { 
            if (mePlayer.Source != null) 
            { 
                mePlayer.Play(); 
                this.Background = Brushes.Black;
                MediaState = MediaState.Play;
            }
        }

        public void Pause()
        {
            if (mePlayer.Source != null)
            { 
                mePlayer.Pause(); 
                this.Background = Brushes.Black;
                MediaState = MediaState.Pause;
            }
        }

        public void Stop() 
        { 
            if (mePlayer.Source != null) 
            { 
                mePlayer.Stop(); 
                this.Background = Brushes.Black;
                MediaState = MediaState.Stop;
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void mePlayer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        public void FitWidth()
        {
            _scrollDragger.FitWidth();  
            ScrollToCenter();
        }

        public void OriginalSize()
        {
            _scrollDragger.OriginalSize();
            ScrollToCenter();
        }

        internal void FitWindow()
        {
            _scrollDragger.FitWindow();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sometimes MediaElement crashes - black window
        /// I will replace it with the new one
        /// </summary>
        internal void RecreateMediaElement(bool flipHorizontally)
        {
            try
            {
                if (mePlayer != null)
                {
                    Settings.Default.SoundVolume = mePlayer.Volume;

                    mePlayer.Stop();
                    mePlayer.Source = null;
                    mePlayer.Volume = 0;
                }

                MediaState = MediaState.Manual;

                mePlayer = new MediaElement();
                mePlayer.Width = 1920;
                mePlayer.Height = 1080;
                mePlayer.LoadedBehavior = MediaState.Manual;
                mePlayer.Stretch = Stretch.Uniform;
                mePlayer.MouseWheel += mePlayer_MouseWheel;
                
                Volume = Settings.Default.SoundVolume; //restore

                scrollPlayer.Content = mePlayer;

                double vOff = Settings.Default.RearPlayerVerticalOffset;
                if (_scrollDragger != null)
                    vOff = _scrollDragger.VerticalOffset;

                _scrollDragger = new ScrollDragZoom(mePlayer, scrollPlayer);
                _scrollDragger.VerticalOffset = vOff;

                //refresh view when change position
                mePlayer.ScrubbingEnabled = true;

                AddFlipXRenderTransform(mePlayer, flipHorizontally);

                mePlayer.MediaOpened += (s, e) => { MediaState = GetMediaState(mePlayer); VideoStarted(); };
                mePlayer.MediaEnded += (s, e) => { VideoEnded(); };
                mePlayer.MediaFailed += (s, e) => { e.Handled = VideoFailed(e, mePlayer); };
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }        
        }

        internal void AddFlipXRenderTransform(UIElement element, bool bFlipHorizontally)
        {
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleY = 1;
            scaleTransform.ScaleX = bFlipHorizontally ? -1 : 1;

            //// Create a TransformGroup to contain the transforms 
            //// and add the transforms to it. 
            //TransformGroup myTransformGroup = new TransformGroup();
            //myTransformGroup.Children.Add(myScaleTransform);

            element.RenderTransform = scaleTransform; // myTransformGroup;
        }

        private MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }

        private void btnFitWidth_Click(object sender, RoutedEventArgs e)
        {
            FitWidth();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            MaximizeAction();
        }

        private void btnOriginalSize_Click(object sender, RoutedEventArgs e)
        {
            OriginalSize();
        }

        private void btnFitWindow_Click(object sender, RoutedEventArgs e)
        {
            FitWindow();
        }

        private void btnFlipHorizontally_Click(object sender, RoutedEventArgs e)
        {
            if (mePlayer.RenderTransform is ScaleTransform scale)
                scale.ScaleX *= -1; //Flip Horizontally
        }
    }
}
