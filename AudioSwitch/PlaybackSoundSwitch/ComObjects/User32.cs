﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PlaybackSoundSwitch.ComObjects
{
    public static class User32
    {
        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowThreadProcessId([In] IntPtr hWnd, [Out] out uint ProcessId);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            internal delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

            [DllImport("user32.dll")]
            public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

            internal const uint WINEVENT_OUTOFCONTEXT = 0;
            internal const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
            internal const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;
        }

        public static uint ForegroundProcessId
        {
            get
            {
                var activeWindowHandle = NativeMethods.GetForegroundWindow();
                uint processId;
                NativeMethods.GetWindowThreadProcessId(activeWindowHandle, out processId);
                return processId;
            }
        }
    }
}