﻿using System.Windows.Controls;

namespace DashCamGPSView.CustomMarkers
{
   /// <summary>
   /// Interaction logic for Test.xaml
   /// </summary>
   public partial class Test : UserControl
   {
      public Test(string txt)
      {
         InitializeComponent();

         text.Text = txt;
      }
   }
}
