using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace JieNor.Megi.Common.Utility
{
	public class ValidateCodeUtility
	{
		private int length = 4;

		private int fontSize = 40;

		private int padding = 2;

		private bool chaos = true;

		private Color chaosColor = Color.LightGray;

		private Color backgroundColor = Color.White;

		private Color[] colors = new Color[1]
		{
			Color.Blue
		};

		private string[] fonts = new string[2]
		{
			"Arial",
			"Georgia"
		};

		private string codeSerial = "2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";

		private const double PI = 3.1415926535897931;

		private const double PI2 = 6.2831853071795862;

		public int Length
		{
			get
			{
				return length;
			}
			set
			{
				length = value;
			}
		}

		public int FontSize
		{
			get
			{
				return fontSize;
			}
			set
			{
				fontSize = value;
			}
		}

		public int Padding
		{
			get
			{
				return padding;
			}
			set
			{
				padding = value;
			}
		}

		public bool Chaos
		{
			get
			{
				return chaos;
			}
			set
			{
				chaos = value;
			}
		}

		public Color ChaosColor
		{
			get
			{
				return chaosColor;
			}
			set
			{
				chaosColor = value;
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return backgroundColor;
			}
			set
			{
				backgroundColor = value;
			}
		}

		public Color[] Colors
		{
			get
			{
				return colors;
			}
			set
			{
				colors = value;
			}
		}

		public string[] Fonts
		{
			get
			{
				return fonts;
			}
			set
			{
				fonts = value;
			}
		}

		public string CodeSerial
		{
			get
			{
				return codeSerial;
			}
			set
			{
				codeSerial = value;
			}
		}

		public Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
		{
			Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
			Graphics graph = Graphics.FromImage(destBmp);
			graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
			graph.Dispose();
			double dBaseAxisLen = bXDir ? ((double)destBmp.Height) : ((double)destBmp.Width);
			for (int j = 0; j < destBmp.Width; j++)
			{
				for (int i = 0; i < destBmp.Height; i++)
				{
					double dx3 = 0.0;
					dx3 = (bXDir ? (6.2831853071795862 * (double)i / dBaseAxisLen) : (6.2831853071795862 * (double)j / dBaseAxisLen));
					dx3 += dPhase;
					double dy = Math.Sin(dx3);
					int nOldX2 = 0;
					int nOldY2 = 0;
					nOldX2 = (bXDir ? (j + (int)(dy * dMultValue)) : j);
					nOldY2 = (bXDir ? i : (i + (int)(dy * dMultValue)));
					Color color = srcBmp.GetPixel(j, i);
					if (nOldX2 >= 0 && nOldX2 < destBmp.Width && nOldY2 >= 0 && nOldY2 < destBmp.Height)
					{
						destBmp.SetPixel(nOldX2, nOldY2, color);
					}
				}
			}
			return destBmp;
		}

		public Bitmap CreateImageCode(string code)
		{
			int fSize = FontSize;
			int fWidth = fSize + Padding;
			int imageWidth = code.Length * fWidth + 4 + Padding * 2;
			int imageHeight = fSize * 2 + Padding;
			Bitmap image = new Bitmap(imageWidth, imageHeight);
			Graphics g = Graphics.FromImage(image);
			g.Clear(BackgroundColor);
			Random rand = new Random();
			if (Chaos)
			{
				Pen pen = new Pen(ChaosColor, 0f);
				int c = Length * 10;
				for (int j = 0; j < c; j++)
				{
					int x = rand.Next(image.Width);
					int y = rand.Next(image.Height);
					g.DrawRectangle(pen, x, y, 1, 1);
				}
			}
			int left2 = 0;
			int top6 = 0;
			int top5 = 1;
			int top4 = 1;
			int n3 = imageHeight - FontSize - Padding * 2;
			int n2 = n3 / 4;
			top5 = n2;
			top4 = n2 * 2;
			for (int i = 0; i < code.Length; i++)
			{
				int cindex = rand.Next(Colors.Length - 1);
				int findex = rand.Next(Fonts.Length - 1);
				Font f = new Font(Fonts[findex], (float)fSize, FontStyle.Bold);
				Brush b = new SolidBrush(Colors[cindex]);
				top6 = ((i % 2 != 1) ? top5 : top4);
				left2 = i * fWidth;
				g.DrawString(code.Substring(i, 1), f, b, (float)left2, (float)top6);
			}
			g.DrawRectangle(new Pen(Color.Gainsboro, 0f), 0, 0, image.Width - 1, image.Height - 1);
			g.Dispose();
			return TwistImage(image, true, 8.0, 4.0);
		}

		public byte[] CreateImageCodeByByte(string code)
		{
			Bitmap bitmap = CreateImageCode(code);
			MemoryStream ms = new MemoryStream();
			bitmap.Save(ms, ImageFormat.Png);
			return ms.ToArray();
		}

		public string CreateVerifyCode(int codeLen = 0)
		{
			if (codeLen == 0)
			{
				codeLen = Length;
			}
			string[] arr = CodeSerial.Split(',');
			string code = "";
			int randValue2 = -1;
			Random rand = new Random((int)DateTime.Now.Ticks);
			for (int i = 0; i < codeLen; i++)
			{
				randValue2 = rand.Next(0, arr.Length - 1);
				code += arr[randValue2];
			}
			return code;
		}
	}
}
