﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MindLamp
{
	public class ColorHSV
	{
		double Hue = 1.0;
		double Saturation = 1.0;
		double Value = 1.0;

		public double H { get { return Hue; } set { Hue = value; } }
		public double S { get { return Saturation; } set { Saturation = value; } }
		public double V { get { return Value; } set { Value = value; } }

		public Color Color 
		{
			get { return ColorFromHSV(Hue, Saturation, Value); }
			set { ColorToHSV(value, out Hue, out Saturation, out Value); }
		}

		public ColorHSV()
		{

		}

		public ColorHSV(Color c)
		{
			Color = c;
		}

		public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
		{
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));

			hue = color.GetHue();
			saturation = (max == 0) ? 0 : 1d - (1d * min / max);
			value = max / 255d;
		}

		public static Color ColorFromHSV(double hue, double saturation, double value)
		{
			int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			value = value * 255;
			int v = Convert.ToInt32(value);
			int p = Convert.ToInt32(value * (1 - saturation));
			int q = Convert.ToInt32(value * (1 - f * saturation));
			int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

			if (hi == 0)
				return Color.FromArgb(255, v, t, p);
			else if (hi == 1)
				return Color.FromArgb(255, q, v, p);
			else if (hi == 2)
				return Color.FromArgb(255, p, v, t);
			else if (hi == 3)
				return Color.FromArgb(255, p, q, v);
			else if (hi == 4)
				return Color.FromArgb(255, t, p, v);
			else
				return Color.FromArgb(255, v, p, q);
		}

		/// <summary>
		/// Convert HSV to RGB
		/// h is from 0-360
		/// s,v values are 0-1
		/// r,g,b values are 0-255
		/// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
		/// </summary>
		void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
		{
			// ######################################################################
			// T. Nathan Mundhenk
			// mundhenk@usc.edu
			// C/C++ Macro HSV to RGB

			double H = h;
			while (H < 0) { H += 360; };
			while (H >= 360) { H -= 360; };
			double R, G, B;
			if (V <= 0)
			{ R = G = B = 0; }
			else if (S <= 0)
			{
				R = G = B = V;
			}
			else
			{
				double hf = H / 60.0;
				int i = (int)Math.Floor(hf);
				double f = hf - i;
				double pv = V * (1 - S);
				double qv = V * (1 - S * f);
				double tv = V * (1 - S * (1 - f));
				switch (i)
				{

					// Red is the dominant color

					case 0:
						R = V;
						G = tv;
						B = pv;
						break;

					// Green is the dominant color

					case 1:
						R = qv;
						G = V;
						B = pv;
						break;
					case 2:
						R = pv;
						G = V;
						B = tv;
						break;

					// Blue is the dominant color

					case 3:
						R = pv;
						G = qv;
						B = V;
						break;
					case 4:
						R = tv;
						G = pv;
						B = V;
						break;

					// Red is the dominant color

					case 5:
						R = V;
						G = pv;
						B = qv;
						break;

					// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

					case 6:
						R = V;
						G = tv;
						B = pv;
						break;
					case -1:
						R = V;
						G = pv;
						B = qv;
						break;

					// The color is not defined, we should throw an error.

					default:
						//LFATAL("i Value error in Pixel conversion, Value is %d", i);
						R = G = B = V; // Just pretend its black/white
						break;
				}
			}
			r = Clamp((int)(R * 255.0));
			g = Clamp((int)(G * 255.0));
			b = Clamp((int)(B * 255.0));
		}

		/// <summary>
		/// Clamp a value to 0-255
		/// </summary>
		int Clamp(int i)
		{
			if (i < 0) return 0;
			if (i > 255) return 255;
			return i;
		}
	}

	public class ColorHSL
	{
		// Private data members below are on scale 0-1
		// They are scaled for use externally based on scale
		private double hue = 1.0;
		private double saturation = 1.0;
		private double luminosity = 1.0;

		private const double scale = 240.0;

		public double Hue
		{
			get { return hue * scale; }
			set { hue = CheckRange(value / scale); }
		}
		
		public double Saturation
		{
			get { return saturation * scale; }
			set { saturation = CheckRange(value / scale); }
		}
		
		public double Luminosity
		{
			get { return luminosity * scale; }
			set { luminosity = CheckRange(value / scale); }
		}

		private double CheckRange(double value)
		{
			if (value < 0.0)
				value = 0.0;
			else if (value > 1.0)
				value = 1.0;
			return value;
		}

		public override string ToString()
		{
			return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
		}

		public string ToRGBString()
		{
			Color color = (Color)this;
			return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
		}

		#region Casts to/from System.Drawing.Color
		public static implicit operator Color(ColorHSL hslColor)
		{
			double r = 0, g = 0, b = 0;
			if (hslColor.luminosity != 0)
			{
				if (hslColor.saturation == 0)
					r = g = b = hslColor.luminosity;
				else
				{
					double temp2 = GetTemp2(hslColor);
					double temp1 = 2.0 * hslColor.luminosity - temp2;

					r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
					g = GetColorComponent(temp1, temp2, hslColor.hue);
					b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
				}
			}
			return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
		}

		private static double GetColorComponent(double temp1, double temp2, double temp3)
		{
			temp3 = MoveIntoRange(temp3);
			if (temp3 < 1.0 / 6.0)
				return temp1 + (temp2 - temp1) * 6.0 * temp3;
			else if (temp3 < 0.5)
				return temp2;
			else if (temp3 < 2.0 / 3.0)
				return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
			else
				return temp1;
		}
		private static double MoveIntoRange(double temp3)
		{
			if (temp3 < 0.0)
				temp3 += 1.0;
			else if (temp3 > 1.0)
				temp3 -= 1.0;
			return temp3;
		}
		private static double GetTemp2(ColorHSL hslColor)
		{
			double temp2;
			if (hslColor.luminosity < 0.5)  //<=??
				temp2 = hslColor.luminosity * (1.0 + hslColor.saturation);
			else
				temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity * hslColor.saturation);
			return temp2;
		}

		public static implicit operator ColorHSL(Color color)
		{
			ColorHSL hslColor = new ColorHSL();
			hslColor.hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
			hslColor.luminosity = color.GetBrightness();
			hslColor.saturation = color.GetSaturation();
			return hslColor;
		}
		#endregion

		public void SetRGB(int red, int green, int blue)
		{
			ColorHSL hslColor = (ColorHSL)Color.FromArgb(red, green, blue);
			this.hue = hslColor.hue;
			this.saturation = hslColor.saturation;
			this.luminosity = hslColor.luminosity;
		}

		public ColorHSL() { }
		public ColorHSL(Color color)
		{
			SetRGB(color.R, color.G, color.B);
		}
		public ColorHSL(int red, int green, int blue)
		{
			SetRGB(red, green, blue);
		}
		public ColorHSL(double hue, double saturation, double luminosity)
		{
			this.Hue = hue;
			this.Saturation = saturation;
			this.Luminosity = luminosity;
		}

	}
}
