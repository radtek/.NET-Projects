﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DashCamGPSView.Tools
{
    public static class Tools
    {
        public static BitmapSource ScreenshotWindow(Window wnd)
        {
            System.Windows.Point pos = wnd.GetAbsolutePosition();
            int width = (int)wnd.ActualWidth;
            var height = (int)wnd.ActualHeight;

            using (var screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen((int)pos.X, (int)pos.Y, 0, 0, new System.Drawing.Size(width, height));
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
        public static void UIElementToPng(UIElement element, string filename)
        {
            //var rect = new Rect(element.RenderSize);
            //var visual = new DrawingVisual();

            //using (var dc = visual.RenderOpen())
            //{
            //    dc.DrawRectangle(new VisualBrush(element), null, rect);
            //}

            //var bitmap = new RenderTargetBitmap(
            //    (int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
            //bitmap.Render(visual);

            var bitmap = UIElementToBitmap(element);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var file = File.OpenWrite(filename))
            {
                encoder.Save(file);
            }
        }

        public static BitmapSource UIElementToBitmap(UIElement element)
        {
            var rect = new Rect(element.RenderSize);
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(element), null, rect);
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
            bitmap.Render(visual);

            return bitmap;
        }

        public static void Snapshot(GpsFileFormat format, string videoFileName, TimeSpan position, UIElement element)
        {
            string fileName = @"C:\Temp\Screenshot.png";
            if (File.Exists(videoFileName))
                fileName = DashCamFileInfo.GetScreenshotFileName(format, videoFileName);

            fileName = string.Format("{0}_at{1}.png", fileName, position.ToString("hh\\.mm\\.ss"));
            UIElementToPng(element, fileName);
            Process.Start(fileName);
        }

        public static void Screenshot(GpsFileFormat format, string videoFileName, TimeSpan position, Window mainWindow)
        {
            string fileName = @"C:\Temp\Screenshot.png";
            if (File.Exists(videoFileName))
                fileName = DashCamFileInfo.GetScreenshotFileName(format, videoFileName);

            fileName = string.Format("{0}_at{1}.png", fileName, position.ToString("hh\\.mm\\.ss"));
            SaveWindowScreenshotToFile(mainWindow, fileName);
            Process.Start(fileName);
        }

        public static void SaveWindowScreenshotToFile(Window wnd, string fileName)
        {
            BitmapSource bmp = ScreenshotWindow(wnd);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream stm = File.Create(fileName))
            {
                png.Save(stm);
            }
        }

        public static void ForceUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
        }
    }

    static class OSInterop
    {
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int smIndex);
        public const int SM_CMONITORS = 80;

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(int nAction, int nParam, ref RECT rc, int nUpdate);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public int width { get { return right - left; } }
            public int height { get { return bottom - top; } }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
        public class MONITORINFOEX
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
            public int dwFlags;
        }
    }

    static class WPFExtensionMethods
    {
        public static System.Windows.Point GetAbsolutePosition(this Window w)
        {
            if (w.WindowState != WindowState.Maximized)
                return new System.Windows.Point(w.Left, w.Top);

            Int32Rect r;
            bool multimonSupported = OSInterop.GetSystemMetrics(OSInterop.SM_CMONITORS) != 0;
            if (!multimonSupported)
            {
                OSInterop.RECT rc = new OSInterop.RECT();
                OSInterop.SystemParametersInfo(48, 0, ref rc, 0);
                r = new Int32Rect(rc.left, rc.top, rc.width, rc.height);
            }
            else
            {
                WindowInteropHelper helper = new WindowInteropHelper(w);
                IntPtr hmonitor = OSInterop.MonitorFromWindow(new HandleRef((object)null, helper.EnsureHandle()), 2);
                OSInterop.MONITORINFOEX info = new OSInterop.MONITORINFOEX();
                OSInterop.GetMonitorInfo(new HandleRef((object)null, hmonitor), info);
                r = new Int32Rect(info.rcWork.left, info.rcWork.top, info.rcWork.width, info.rcWork.height);
            }
            return new System.Windows.Point(r.X, r.Y);
        }
    }
}
