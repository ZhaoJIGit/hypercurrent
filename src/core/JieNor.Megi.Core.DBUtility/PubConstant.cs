using JieNor.Megi.Common.Encrypt;
using System.Configuration;

namespace JieNor.Megi.Core.DBUtility
{
	public class PubConstant
	{
		public static string ConnectionString
		{
			get
			{
				string text = ConfigurationManager.AppSettings["ConnectionString"];
				string a = ConfigurationManager.AppSettings["ConStringEncrypt"];
				if (a == "true")
				{
					text = DESEncrypt.Decrypt(text);
				}
				return text;
			}
		}

		public static string GetConnectionString(string configName)
		{
			string text = ConfigurationManager.AppSettings[configName];
			string a = ConfigurationManager.AppSettings["ConStringEncrypt"];
			if (a == "true")
			{
				text = DESEncrypt.Decrypt(text);
			}
			return text;
		}
	}
}
