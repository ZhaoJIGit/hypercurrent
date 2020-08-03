using System;
using System.Text;

namespace JieNor.Megi.Core
{
	public static class RandomService
	{
		public static string GetRndNum()
		{
			string text = string.Empty;
			Random random = new Random();
			for (int i = 0; i < 4; i++)
			{
				text += random.Next(9);
			}
			return text;
		}

		public static string GetRndStr()
		{
			string text = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
			string[] array = text.Split(',');
			string text2 = string.Empty;
			Random random = new Random();
			for (int i = 0; i < 4; i++)
			{
				int num = random.Next(array.Length);
				text2 += array[num];
			}
			return text2;
		}

		public static string GetRndCh()
		{
			Encoding @default = Encoding.Default;
			object[] array = CreateRegionCode(4);
			string[] array2 = new string[4];
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				array2[i] = @default.GetString((byte[])Convert.ChangeType(array[i], typeof(byte[])));
				stringBuilder.Append(array2[i].ToString());
			}
			return stringBuilder.ToString();
		}

		private static object[] CreateRegionCode(int strlength)
		{
			string[] array = new string[16]
			{
				"0",
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
				"a",
				"b",
				"c",
				"d",
				"e",
				"f"
			};
			Random random = new Random();
			object[] array2 = new object[strlength];
			for (int i = 0; i < strlength; i++)
			{
				int num = random.Next(11, 14);
				string str = array[num].Trim();
				int num2 = num;
				DateTime now = DateTime.Now;
				random = new Random(num2 * (int)now.Ticks + i);
				int num3 = (num != 13) ? random.Next(0, 16) : random.Next(0, 7);
				string str2 = array[num3].Trim();
				int num4 = num3;
				now = DateTime.Now;
				random = new Random(num4 * (int)now.Ticks + i);
				int num5 = random.Next(10, 16);
				string str3 = array[num5].Trim();
				int num6 = num5;
				now = DateTime.Now;
				random = new Random(num6 * (int)now.Ticks + i);
				int num7;
				switch (num5)
				{
				case 10:
					num7 = random.Next(1, 16);
					break;
				case 15:
					num7 = random.Next(0, 15);
					break;
				default:
					num7 = random.Next(0, 16);
					break;
				}
				string str4 = array[num7].Trim();
				byte b = Convert.ToByte(str + str2, 16);
				byte b2 = Convert.ToByte(str3 + str4, 16);
				byte[] value = new byte[2]
				{
					b,
					b2
				};
				array2.SetValue(value, i);
			}
			return array2;
		}
	}
}
