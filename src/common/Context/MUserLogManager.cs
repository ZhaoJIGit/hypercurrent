using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Mongo.BusinessService;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.EntityModel.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Common.Context
{
	public class MUserLogManager
	{
		public static void SaveUserLog(ActionExecutingContext actionContext)
		{
			try
			{
				if (ConfigurationManager.AppSettings["LogUser"] != null && !(ConfigurationManager.AppSettings["LogUser"].ToString() != "1"))
				{
					HttpContextBase httpContext = actionContext.HttpContext;
					HttpRequestBase request = httpContext.Request;
					string requestUri = request.Url.AbsoluteUri;
					string requestHost = request.Url.DnsSafeHost;
					string urlSchema = request.Url.Scheme.ToLower();
					if (!request.Path.ToLower().Equals(ContextHelper.MCheckTokenUrl))
					{
						string tabTitle = GetTabTitle(request);
						MUserLog log = new MUserLog
						{
							Id = UUIDHelper.GetGuid(),
							MDate = DateTime.Now,
							MPath = GetPurePathAndQuery(request.Url.PathAndQuery),
							MType = request.RequestType,
							MUserAddress = request.UserHostAddress,
							MParameters = JsonConvert.SerializeObject(actionContext.ActionParameters),
							MAccessToken = request.Params["MAccessToken"],
							MEmail = DESEncrypt.Encrypt(request.Params["MUserEmail"]),
							MLocaleID = request.Params["MLocaleID"],
							MOrgID = request.Params["MOrgID"],
							MNavigator = request.UserAgent,
							MTabTitle = tabTitle
						};
						Thread thread = new Thread((ThreadStart)delegate
						{
							new MongoMUserLogBusiness().SaveMUserLog(new List<MUserLog>
							{
								log
							});
						});
						thread.Start();
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private static string GetTabTitle(HttpRequestBase request)
		{
			string query = request.IsAjaxRequest() ? request.UrlReferrer.Query : request.Url.Query;
			Regex reg = new Regex("(?<=_tabTitle_=)(\\S*)");
			string tabTitle = reg.IsMatch(query) ? reg.Matches(query)[0].ToString() : null;
			return JavaScriptDES.DecryptDES(tabTitle, null);
		}

		private static string GetPurePathAndQuery(string pathAndQuery)
		{
			if (string.IsNullOrWhiteSpace(pathAndQuery))
			{
				return pathAndQuery;
			}
			return new Regex("\\?*(ver=[0-9]+&)*_tabTitle_=(.*)&*").Replace(pathAndQuery, "");
		}
	}
}
