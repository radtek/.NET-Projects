using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClipboardManager
{
	public partial class FormSettings : Form
	{
		private FormClipboard.Settings m_Settings = null;
		private HotKeyTranslator m_HotKey = null;

		public FormSettings(FormClipboard.Settings sett)
		{
			this.m_Settings = sett;
			
			//remember old settings - to reset on cancel
			m_HotKey = m_Settings.m_HotKey.Clone();
			
			InitializeComponent();
		}//end constructor

		private void m_txtHotKey_TextChanged(object sender, EventArgs e)
		{
			m_txtHotKey.Text = m_HotKey.ToString();
		}//end m_txtHotKey_TextChanged

		private void m_txtHotKey_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode < Keys.A || e.KeyCode > Keys.Z )
				return;

			m_HotKey.SetHotKey(e);
			e.Handled = true;
			m_txtHotKey.Text = m_HotKey.ToString();
		}//end m_txtHotKey_KeyUp

		private void FormSettings_Load(object sender, EventArgs e)
		{
			m_txtHotKey.Text				= m_HotKey.ToString();
			m_chkUseHotKey.Checked			= m_HotKey.m_bUseHotKey;

			m_numHistoryLen.Text			= m_Settings.m_iHistoryLen.ToString();
			m_numHistoryLen.Value			= m_Settings.m_iHistoryLen;
			m_chkStartWithWindows.Checked	= LoadWithWindows;
			
			m_HotKey.UnregisterHotKey(); //to allow change or reset

            m_chkReconnect.Checked			= m_Settings.m_AutoReconnect;
            m_chkLog.Checked    			= m_Settings.WriteLogFile;
        }//end FormSettings_Load

		private void m_btnOK_Click(object sender, EventArgs e)
		{
			LoadWithWindows = m_chkStartWithWindows.Checked;

			m_Settings.m_iHistoryLen = (int)m_numHistoryLen.Value;

			m_Settings.m_HotKey = m_HotKey; //set new values
			m_Settings.m_HotKey.RegisterHotKey(); //register if needed

			m_Settings.m_AutoReconnect = m_chkReconnect.Checked;
            m_Settings.WriteLogFile  = m_chkLog.Checked;
        }//end m_btnOK_Click

		private void m_btnCancel_Click(object sender, EventArgs e)
		{
			m_Settings.m_HotKey.RegisterHotKey(); //register old key if needed
		}//end m_btnCancel_Click

        private const string _strAppKey = @"ClipboardHistoryMZ";
        private Microsoft.Win32.RegistryKey OpenRegKey(bool writable)
        {
            //const string REG_KEY = @"Software\Microsoft\Windows\CurrentVersion\Run";
            const string REG_KEY = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run";
            try
            {
                return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(REG_KEY, writable);
            }
            catch (Exception err)
            {
                FormClipboard.MsgBoxErr(err.Message);
                return null;
            }
        }

		[Category("Windows Options")]
		[Description("Load application when windows starts")]
		[DisplayName("Load with Windows")]
		public bool LoadWithWindows
		{
			get
			{
                var key = OpenRegKey(writable: false);
                if (key == null)
                    return false;
                bool run = (key.GetValue(_strAppKey) != null);
                key.Close();
                return run;
			}//end get
			
            set
			{
                var key = OpenRegKey(writable: true);
                if (key == null)
                    return;

                if (value)
					key.SetValue(_strAppKey, "\""+Application.ExecutablePath+"\"");
				else
					key.DeleteValue(_strAppKey, false);
                key.Close();
			}//end set
		}//end LoadWithWindows

		private void m_chkUseHotKey_CheckedChanged(object sender, EventArgs e)
		{
			m_txtHotKey.Enabled = m_chkUseHotKey.Checked;
			m_HotKey.m_bUseHotKey = m_chkUseHotKey.Checked;
		}
	}//end class FormSettings
}//end namespace ClipboardListener