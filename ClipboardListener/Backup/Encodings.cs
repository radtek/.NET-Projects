using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace ClipboardManager
{
	public class Encodings
	{
		public class Item : IComparable
		{
			public bool bEnable = false;
			public string sName = "";
			public Encoding e = null;

			public Item(Encoding e)
			{
				this.e = e;
				this.sName = e.EncodingName;
			}//end constructor

			public int CompareTo(object obj)
			{
				Item i = (Item)obj;
				if ( i == null )
					return -1;

				return sName.CompareTo(i.sName);
			}//end CompareTo
		}//end class Item

		public ArrayList m_vItems = new ArrayList();
		RichTextBox m_RichTextBoxSrc, m_RichTextBoxDst = null;

		public Encodings()
		{
			foreach ( EncodingInfo ei in Encoding.GetEncodings() )
			{
				Encoding e = ei.GetEncoding();
				m_vItems.Add(new Item(e));
			}//end foreach
			m_vItems.Sort();
		}//end constructor

		public int CreateEncodingsMenuItems(ToolStripItemCollection items, 
			RichTextBox richSrc, RichTextBox richDst)
		{
			m_RichTextBoxSrc = richSrc;
			m_RichTextBoxDst = richDst;

			ArrayList vEncodingMenus = new ArrayList();
			for ( int i = 0; i < m_vItems.Count; i++ )
			{
				Item itm = (Item)m_vItems[i];
				if ( !itm.bEnable )
					continue;

				ToolStripItem x = new ToolStripMenuItem(itm.sName);
				x.Tag = itm;
				x.Click += new EventHandler(MenuItem_Encoding_Click);

				vEncodingMenus.Add(x);
			}//end for
			
			//leave first 2 items
			while ( items.Count > 2 )
				items.RemoveAt(items.Count - 1);

			if ( vEncodingMenus.Count == 0 )
				return 0;

			items.AddRange((ToolStripItem[])vEncodingMenus.ToArray(typeof(ToolStripItem)));

			return items.Count;
		}//end CreateEncodingsMenuItems

		private string Decode(string s, Encoding e)
		{
            Encoding eDef = Encoding.Default;
            byte[] currentBytes = eDef.GetBytes(s);

            string targetString = Decode(currentBytes, e);
            if (targetString.IndexOf("????") >= 0) //conversion did not succceed, try another way
            {
                char[] origChars = s.ToCharArray();
                currentBytes = new byte[origChars.Length];
                for (int i = 0; i < origChars.Length; i++)
                    currentBytes[i] = (byte)origChars[i];

                targetString = Decode(currentBytes, e);
            }//end if
            return targetString;
        }//end Decode

        private string Decode(byte[] currentBytes, Encoding e)
        {
            Decoder decoder = e.GetDecoder();
            int count = decoder.GetCharCount(currentBytes, 0, currentBytes.Length);
            char[] targetChars = new char[count];
            count = decoder.GetChars(currentBytes, 0, currentBytes.Length, targetChars, 0);
            return new string(targetChars);
        }

		void MenuItem_Encoding_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem itm = (ToolStripMenuItem)sender;
			Item i = (Item)itm.Tag;

			System.Diagnostics.Trace.WriteLine("MenuItem_Encoding_Click: "+itm.Text);

			m_RichTextBoxDst.Text = Decode(m_RichTextBoxSrc.Text, i.e);
		}//end MenuItem_Encoding_Click

		//
		public void Save(XmlNode ndParent)
		{
			XmlNode ndEnc = XmlUtil.AddNewNode(ndParent, "Encodings");
			foreach ( Item i in m_vItems )
			{
				if ( !i.bEnable )
					continue;

				XmlUtil.AddNewNode(ndEnc, "Encoding", i.sName);
			}//end foreach
		}//end Save

		public void Load(XmlNode ndParent)
		{
			int countEnabled = 0;
			XmlNodeList list = ndParent.SelectNodes("Encodings/Encoding");
			foreach ( XmlNode n in list )
			{
				if ( EnableEncoding(n.InnerText) )
					countEnabled++;
			}//end foreach

			if ( countEnabled > 0 )
				return;

			//add some default settings
			EnableEncoding("Hebrew (Windows)");
			EnableEncoding("Cyrillic (Windows)");
		}//end Load

		private int Find(string sEncodingName)
		{
			for ( int i = 0; i < m_vItems.Count; i++ )
			{
				Item itm = (Item)m_vItems[i];
				if ( itm.sName == sEncodingName )
					return i;
			}//end for
			return -1;
		}//end Find

		private bool EnableEncoding(string sEncodingName)
		{
			int idx = Find(sEncodingName);
			if ( idx < 0 ) return false;
			Item itm = (Item)m_vItems[idx];
			itm.bEnable = true;
			return true;
		}//end EnableEncoding
	}//end class Encodings
}//end namespace ClipboardListener
