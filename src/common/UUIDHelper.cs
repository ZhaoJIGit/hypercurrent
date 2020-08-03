using System;
using System.Net;

namespace JieNor.Megi.Common
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
			long[] ip = new long[4];
			string[] s = strIP.Split('.');
			ip[0] = long.Parse(s[0]);
			ip[1] = long.Parse(s[1]);
			ip[2] = long.Parse(s[2]);
			ip[3] = long.Parse(s[3]);
			return (ip[0] << 24) + (ip[1] << 16) + (ip[2] << 8) + ip[3];
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
			IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
			for (int i = 0; i != IpEntry.AddressList.Length; i++)
			{
				if (!IpEntry.AddressList[i].IsIPv6LinkLocal)
				{
					return IpEntry.AddressList[i].ToString();
				}
			}
			return "127.0.0.1";
		}
	}
}
