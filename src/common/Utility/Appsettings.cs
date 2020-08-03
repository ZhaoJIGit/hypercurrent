using System;
using System.Configuration;

namespace JieNor.Megi.Common.Utility
{
	public class Appsettings
	{
		public static string Versions => Guid.NewGuid().ToString();

		public static string InitConfig => FileUtility.GetPhysicalPath(ConfigurationManager.AppSettings["InitConfig"]);
	}
}
