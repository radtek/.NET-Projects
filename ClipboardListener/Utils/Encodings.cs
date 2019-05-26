using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ClipboardManager
{
    //https://stackoverflow.com/questions/1212742/xml-serialize-generic-list-of-serializable-objects
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlInclude(typeof(EncodingItemData))]
    public class EncodingsData
    {
        [XmlArray("EncodingsList")]
        [XmlArrayItem("EncodingItemData")]
        public List<EncodingItemData> Encodings { get; set; } = new List<EncodingItemData>();

        public override string ToString()
        {
            int sel = CountSelected();
            return string.Format("Selected Encodings {0} of {1}", sel, Encodings.Count);
        }

        private int CountSelected()
        {
            int count = 0;
            foreach (EncodingItemData e in Encodings)
            {
                if (e.bEnable) count++;
            }
            return count;
        }

        public void UpdateEncodingsAfterLoadFromXml()
        {
            List<EncodingItemData> loaded = new List<EncodingItemData>(Encodings);
            Encodings.Clear();
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                bool show = GetShow(ei.CodePage, loaded);
                Encodings.Add(new EncodingItemData() { e = ei.GetEncoding(), bEnable = show });
            }//end foreach
        }

        private bool GetShow(int codePage, List<EncodingItemData> loaded)
        {
            foreach (EncodingItemData data in loaded)
            {
                if (data.CodePage == codePage)
                    return data.bEnable;
            }
            return false;
        }
    }

    [Serializable]
    [XmlType("EncodingItemData")]
    [TypeConverter(typeof(PropertySorter))]
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public class EncodingItemData
    {
        [XmlAttribute(AttributeName = "Show")]
        [DisplayName("Show in Menu")]
        [PropertyOrder(1)]
        public bool bEnable { get; set; } = false;
        [PropertyOrder(2)]
        public string sName { get { if (e != null) return e.EncodingName; else return "N/A"; } }
        [ReadOnly(true)]
        [PropertyOrder(3)]
        public int CodePage { get; set; } = Encoding.UTF8.CodePage;
        [XmlIgnore]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PropertyOrder(4)]
        public Encoding e
        {
            get
            {
                try { return Encoding.GetEncoding(CodePage); }
                catch (Exception err) { return Encoding.UTF8; }
            }
            set
            {
                CodePage = value.CodePage;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}) {1}", bEnable?"+":"-", sName);
        }
    }

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
            byte[] currentBytes = Encoding.Unicode.GetBytes(s);

			byte [] convertedBytes = Encoding.Convert(e, e, currentBytes);
			char[] convertedChars = new char[e.GetCharCount(convertedBytes, 0, convertedBytes.Length)];
			e.GetChars(convertedBytes, 0, convertedBytes.Length, convertedChars, 0);
			return new string(convertedChars);

			//string targetString = Decode(currentBytes, e);
			//if (targetString.IndexOf("????") >= 0) //conversion did not succceed, try another way
			//{
			//    char[] origChars = s.ToCharArray();
			//    currentBytes = new byte[origChars.Length];
			//    for (int i = 0; i < origChars.Length; i++)
			//        currentBytes[i] = (byte)origChars[i];

			//    targetString = Decode(currentBytes, e);
			//}//end if

            //return new string(convertedBytes);
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
