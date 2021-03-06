using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;
using ClipboardManager.Properties;
using MZ.Tools;
using Utils;

namespace ClipboardManager
{
	static class Program
	{
		//static Mutex m_SingleInstance = null;
		static int m_iFailCount = 0;

//#if (DEBUG)
//		static string AppName = "ClipboardManager(Debug)";
//#else
//		static string AppName = "ClipboardManager";
//#endif

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            UpdateDependencies();

RunAgain:
			try
			{
                //single instance
                using (var mutex = new System.Threading.Mutex(true, FormClipboard.TITLE, out bool result))
                {
                    if (!result)
                    {
                        CenteredMessageBox.MsgBoxErr("Another instance of "+ FormClipboard.TITLE + " is already running.", FormClipboard.TITLE);
                        return;
                    }
                    Application.Run(new FormClipboard());
                }
			}//end try
			catch ( Exception err )
			{
                Utils.Log.WriteLineF("[Main] Exeption: " + err.ToString());

				m_iFailCount++;
                Utils.Log.LogEventErr("Exception(No:" + m_iFailCount + ") in main: " + err.Message);
				if ( m_iFailCount < 4 )
					goto RunAgain;
			}//end catch

            Utils.Log.CloseLog();
		}//end Main

        private static void UpdateDependencies()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileName = Path.Combine(dir, "MZ.WPF.MessageBox.dll");
            if (!File.Exists(fileName))
                File.WriteAllBytes(fileName, Properties.Resources.MZ_WPF_MessageBox);
        }

        public static string GetUserPath()
        {
            string path = null; // Settings.Default.UserPath;
            if (string.IsNullOrEmpty(path))
                path = Application.UserAppDataPath;
            return path;
        }
	}//end class Program
}//end namespace ClipboardListener