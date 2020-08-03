using JieNor.Megi.Common.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Configuration;
using System.Linq;

namespace JieNor.Megi.Common.Utility
{
	public static class ServerHelper
	{
		private static readonly string _versionNumber = ConfigurationManager.AppSettings["JavaScriptCSSVersion"];

		private static readonly string _serviceEmail = ConfigurationManager.AppSettings["ServiceEmail"];

		public static string[] DomainString => ConfigurationManager.AppSettings["DomainString"].Split(';');

		public static string[] BetaDomainString => ConfigurationManager.AppSettings["BetaDomainString"].Split(';');

		public static bool IsBetaSite => Convert.ToBoolean(ConfigurationManager.AppSettings["IsBetaSite"]);

		public static string ServiceEmail => _serviceEmail;

		public static string WebServerPrefix
		{
			get
			{
				string ssl = ConfigurationManager.AppSettings["WebSSLEnable"];
				return (!string.IsNullOrWhiteSpace(ssl) && ssl.ToString().Equals("1")) ? "Https" : "Http";
			}
		}

		public static string AccessWhiteList
		{
			get
			{
				string format = ConfigurationManager.AppSettings["AccessWhiteList"];
				object[] domainString = DomainString;
				return string.Format(format, domainString);
			}
		}

		public static string ServiceServerPrefix
		{
			get
			{
				string ssl = ConfigurationManager.AppSettings["ServiceSSLEnable"];
				return (!string.IsNullOrWhiteSpace(ssl) && ssl.ToString().Equals("1")) ? "Https" : "Http";
			}
		}

		public static string ScriptServer => GetWebServer(ServerType.ScriptServer);

		public static string StaticServer => GetWebServer(ServerType.StaticServer);

		public static string LoginServer => GetWebServer(ServerType.LoginServer);

		public static string GoServer => GetWebServer(ServerType.GoServer);

		public static string BetaGoServer
		{
			get
			{
				string url2 = GetWebServer(ServerType.GoServer, BetaDomainString);
				return url2 = (string.IsNullOrWhiteSpace(url2) ? GoServer : url2);
			}
		}

		public static string FinalMyServer
		{
			get
			{
				string url2 = GetWebServer(ServerType.MyServer, DomainString);
				return url2 = (string.IsNullOrWhiteSpace(url2) ? MyServer : url2);
			}
		}

		public static string FinalLoginServer
		{
			get
			{
				string url = GetWebServer(ServerType.LoginServer, DomainString);
				return string.IsNullOrWhiteSpace(url) ? LoginServer : url;
			}
		}

		public static string FinalGoServer
		{
			get
			{
				string url = GetWebServer(ServerType.GoServer, DomainString);
				return string.IsNullOrWhiteSpace(url) ? GoServer : url;
			}
		}

		public static string MyServer => GetWebServer(ServerType.MyServer);

		public static string AppServer => GetWebServer(ServerType.AppServer);

		public static string ProfileServer => GetWebServer(ServerType.ProfileServer);

		public static string MainServer => GetWebServer(ServerType.MainServer);

		public static string HubServer => GetWebServer(ServerType.HubServer);

		public static string SysServiceUrl => GetServiceServer(ServerType.SysServiceUrl);

		public static string MongoServiceUrl => GetServiceServer(ServerType.MongoServiceUrl);

		public static string WebApiServiceUrl => GetServiceServer(ServerType.WebApiServiceUrl);

		public static string[] MyServerSetupPages
		{
			get
			{
				string[] setupPageArray = new string[0];
				string setupPageConfig = ConfigurationManager.AppSettings["SetupPage"];
				if (!string.IsNullOrWhiteSpace(setupPageConfig))
				{
					setupPageArray = setupPageConfig.Split(',');
				}
				return setupPageArray;
			}
		}

		public static string Domain => ConfigurationManager.AppSettings["Domain"];

		public static string JSVersion => $"ver={_versionNumber}";

		public static string VersionNumber => _versionNumber;

		public static string[] MegiLangTypes
		{
			get
			{
				string config = ConfigurationManager.AppSettings["MegiLangTypes"];
				if (string.IsNullOrEmpty(config))
				{
					return new string[3]
					{
						"0x0009",
						"0x7804",
						"0x7C04"
					};
				}
				return config.Split(',').ToArray();
			}
			set
			{
			}
		}

		public static string DefaultUserEmail
		{
			get
			{
				return CookieHelper.GetCookieValue(ContextHelper.MUserEmailCookie, null);
			}
			set
			{
				CookieHelper.ClearCookie(ContextHelper.MUserEmailCookie, ContextHelper.Domain, null);
				CookieHelper.SetCookie(ContextHelper.MUserEmailCookie, value, DateTime.Now.AddYears(1), ContextHelper.Domain, null);
			}
		}

		public static string NotFoundPage => ConfigurationManager.AppSettings["NotFoundPage"];

		public static string ErrorPage => ConfigurationManager.AppSettings["ErrorPage"];

		public static bool IsEnableHttpsJump => Convert.ToBoolean(ConfigurationManager.AppSettings["EnableHttpsJump"]);

		public static string GetWebServer(ServerType type)
		{
			string[] tempDomainString = IsBetaSite ? BetaDomainString : DomainString;
			string webServerPrefix = WebServerPrefix;
			string format = ConfigurationManager.AppSettings[type.ToString()];
			object[] args = tempDomainString;
			return webServerPrefix + "://" + string.Format(format, args);
		}

		public static string GetWebServer(ServerType type, string[] domainString)
		{
			return WebServerPrefix + "://" + string.Format(ConfigurationManager.AppSettings[type.ToString()], domainString);
		}

		public static string GetServiceServer(ServerType type)
		{
			string[] tempDomainString = IsBetaSite ? BetaDomainString : DomainString;
			string serviceServerPrefix = ServiceServerPrefix;
			string format = ConfigurationManager.AppSettings[type.ToString()];
			object[] args = tempDomainString;
			return serviceServerPrefix + "://" + string.Format(format, args);
		}
	}
}
