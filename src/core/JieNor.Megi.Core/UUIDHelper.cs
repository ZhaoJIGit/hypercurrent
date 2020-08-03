using System;
using System.Net;

namespace JieNor.Megi.Core
{
	public class UUIDHelper
	{
		private static int curSeq = 1;

		private static object objLock = 1;

		public static string GetGuid()
		{
			return Guid.NewGuid().ToString().Replace("-", "")
				.Replace("{", "")
				.Replace("}", "");
		}

		public static long IP2Long(string strIP)
		{
			long[] array = new long[4];
			string[] array2 = strIP.Split('.');
			array[0] = long.Parse(array2[0]);
			array[1] = long.Parse(array2[1]);
			array[2] = long.Parse(array2[2]);
			array[3] = long.Parse(array2[3]);
			return (array[0] << 24) + (array[1] << 16) + (array[2] << 8) + array[3];
		}

		private static int GetSequence()
		{
			lock (objLock)
			{
				if (curSeq > 999)
				{
					curSeq = 1;
				}
				return curSeq++;
			}
		}

		public static string GetLocalIPv4()
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
			for (int i = 0; i != hostEntry.AddressList.Length; i++)
			{
				if (!hostEntry.AddressList[i].IsIPv6LinkLocal)
				{
					return hostEntry.AddressList[i].ToString();
				}
			}
			return "127.0.0.1";
		}
	}
}
