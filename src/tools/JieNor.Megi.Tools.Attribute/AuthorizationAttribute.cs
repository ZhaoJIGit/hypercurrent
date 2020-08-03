using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.SEC;

namespace JieNor.Megi.Tools.Attribute
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class AuthorizationAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			HttpContextBase httpContext = actionContext.HttpContext;
			HttpRequestBase request = httpContext.Request;
			string absoluteUri = request.Url.AbsoluteUri;
			string dnsSafeHost = request.Url.DnsSafeHost;
			string text = request.Url.Scheme.ToLower();
			if (ServerHelper.IsEnableHttpsJump && !text.Equals(ServerHelper.WebServerPrefix.ToLower()))
			{
				actionContext.Result = new RedirectResult(ServerHelper.WebServerPrefix.ToLower() + "://" + absoluteUri.RemoveHttpLabel().RemoveHttpsLabel());
				return;
			}


			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string empty4 = string.Empty;
			string empty5 = string.Empty;
			if (request.Path.ToLower().Equals(ContextHelper.MLoginBoxSignInUrl) && AjaxRequestExtensions.IsAjaxRequest(httpContext.Request))
			{
				GetActionParameters(actionContext.ActionParameters, out empty, out empty2, out empty3, out empty4, out empty5);
				SECLoginResultModel secloginResultModel = LoginHelper.LoginBoxSignIn(new SECLoginModel
				{
					Password = empty2,
					Email = empty,
					OrgId = empty3,
					MLCID = empty5
				}, httpContext);

				if (secloginResultModel.IsSuccess)
				{
					MContext mcontextByAccessToken = MContextManager.GetMContextByAccessToken(secloginResultModel.MAccessToken, "System");
					string text2 = "";
					if (ServerHelper.MyServer.ToLower().IndexOf(dnsSafeHost) >= 0 && mcontextByAccessToken.MRegProgress > 0)
					{
						string text3 = httpContext.Request.QueryString["lang"];
						string mbrowserTabIndex = secloginResultModel.MBrowserTabIndex;
						if (mcontextByAccessToken.MRegProgress < 15)
						{
							if (secloginResultModel.MIsDeletedOrgID)
							{
								text2 = string.Concat(new string[]
								{
									ServerHelper.MyServer,
									"?bti=",
									mbrowserTabIndex,
									"&lang=",
									text3
								});
							}
							else
							{
								string text4 = Convert.ToString(typeof(InitWizardUrlEnum).GetField(Convert.ToString((BASOrgScheduleTypeEnum)mcontextByAccessToken.MRegProgress)).GetRawConstantValue());
								text2 = string.Format("{0}?RedirectUrl={1}&redirectOnload=true", ServerHelper.MyServer, HttpUtility.UrlEncode(string.Format("{0}{1}?bti={2}&lang={3}", new object[]
								{
									ServerHelper.GoServer,
									text4,
									mbrowserTabIndex,
									text3
								})));
								text2 = string.Concat(new string[]
								{
									text2,
									"&bti=",
									mbrowserTabIndex,
									"&lang=",
									text3
								});
							}
						}
						else
						{
							text2 = string.Concat(new string[]
							{
								ServerHelper.GoServer,
								"?bti=",
								secloginResultModel.MBrowserTabIndex,
								"&lang=",
								text3
							});
						}
					}
					actionContext.Result = new JsonResult
					{
						Data = new
						{
							data = (secloginResultModel.IsSuccess ? 1 : 0),
							bti = secloginResultModel.MBrowserTabIndex,
							setupProgress = mcontextByAccessToken.MRegProgress,
							setupUrl = text2,
							lang = secloginResultModel.MLocaleID,
							isDeletedOrgID = secloginResultModel.MIsDeletedOrgID
						},
						JsonRequestBehavior = 0
					};
					return;
				}
				actionContext.Result = new JsonResult
				{
					Data = new
					{
						data = (secloginResultModel.IsSuccess ? 1 : 0)
					},
					JsonRequestBehavior = 0
				};
				return;
			}
			else
			{
				if (request.Path.ToLower().Equals(ContextHelper.MCheckTokenUrl) && AjaxRequestExtensions.IsAjaxRequest(httpContext.Request))
				{
					GetActionParameters(actionContext.ActionParameters, out empty, out empty2, out empty3, out empty4, out empty5);
					int type = ContextHelper.Check(httpContext, empty4, empty, empty3, empty5);
					actionContext.Result = new JsonResult
					{
						Data = new
						{
							type
						},
						JsonRequestBehavior = 0
					};
					return;
				}
				if (!ContextHelper.CheckHostInWhiteList(dnsSafeHost))
				{
					string userHostAddress = request.UserHostAddress;
					MContext mcontext = ContextHelper.MContext;
					string key = "RedirectUrl";
					string text5 = httpContext.Request.QueryString["lang"];
					text5 = (string.IsNullOrWhiteSpace(text5) ? ContextHelper.MLocaleID : text5);
					if (mcontext != null && text5 != mcontext.MLCID)
					{
						mcontext.MLCID = text5;
						ContextHelper.MContext = mcontext;
						ContextHelper.SaveLocaleIDToCookie(text5, null);
					}
					int type2 = ContextHelper.Check(httpContext, empty4, empty, empty3, empty5);
					if (mcontext == null || mcontext.IsExpired)
					{
						if (AjaxRequestExtensions.IsAjaxRequest(httpContext.Request))
						{
							actionContext.HttpContext.Response.StatusCode = 252;
							actionContext.Result = new JsonResult
							{
								Data = new
								{
									accessDenied = 1,
									type = type2
								},
								JsonRequestBehavior = 0
							};
							return;
						}
						actionContext.Result = new RedirectResult(ServerHelper.LoginServer.AddUrlParamemter(key, HttpUtility.UrlEncode(absoluteUri)));
						return;
					}
					else
					{
						if (ConfigurationManager.AppSettings["OnlyCheckLogin"] == "1")
						{
							return;
						}
						List<string> redirectUrl = ContextHelper.GetRedirectUrl(mcontext);
						if (mcontext.MServerType == 2 && (ServerHelper.MyServerSetupPages.Any((string page) => request.Path.ToLower().Contains(page.ToLower())) || request.Path.ToLower().ToLower().Contains("/bd/") || (request.UrlReferrer != null && ServerHelper.MyServerSetupPages.Any((string page) => request.UrlReferrer.AbsolutePath.ToLower().Contains(page.ToLower())))))
						{
							return;
						}
						if (!dnsSafeHost.CheckHostInList(redirectUrl))
						{
							if (AjaxRequestExtensions.IsAjaxRequest(request))
							{
								CookieHelper.ClearCookie(ContextHelper.MAccessTokenCookie, ContextHelper.Domain, httpContext);
								CookieHelper.ClearCookie(ContextHelper.MUserEmailCookie, ContextHelper.Domain, httpContext);
								actionContext.HttpContext.Response.StatusCode = 252;
								actionContext.Result = new JsonResult
								{
									Data = new
									{
										accessDenied = 1,
										type = 2
									},
									JsonRequestBehavior = 0
								};
								return;
							}
							string text6 = redirectUrl[0];
							if (text6.ToLower().IndexOf(ServerHelper.GoServer.ToLower()) >= 0 || text6.ToLower().IndexOf(ServerHelper.MyServer.ToLower()) >= 0)
							{
								text6 = this.UrlIntercept(text6, httpContext);
							}
							text6 = text6.AddUrlParamemter(key, HttpUtility.UrlEncode(absoluteUri));
							actionContext.Result = new RedirectResult(text6);
							return;
						}
						else if (dnsSafeHost.RemoveHttpLabel().RemoveHttpsLabel() == ServerHelper.GoServer.RemoveHttpLabel().RemoveHttpsLabel() && mcontext.MRegProgress > 0 && mcontext.MRegProgress < 15)
						{
							string text7 = httpContext.Request.QueryString["bti"];
							string text8 = httpContext.Request.QueryString["lang"];
							string text9 = Convert.ToString(typeof(InitWizardUrlEnum).GetField(Convert.ToString((BASOrgScheduleTypeEnum)mcontext.MRegProgress)).GetRawConstantValue());
							string text10 = string.Format("{0}?RedirectUrl={1}&redirectOnload=true", ServerHelper.MyServer, HttpUtility.UrlEncode(string.Format("{0}{1}?bti={2}&lang={3}", new object[]
							{
								ServerHelper.GoServer,
								text9,
								text7,
								text8
							})));
							text10 = string.Concat(new string[]
							{
								text10,
								"&bti=",
								text7,
								"&lang=",
								text8
							});
							actionContext.Result = new RedirectResult(text10);
						}
					}
				}
				return;
			}
		}

		private bool IsNavToMySite(MContext mContext, string requestHost)
		{
			string text = requestHost.RemoveHttpLabel().RemoveHttpsLabel();
			string text2 = ServerHelper.GoServer.RemoveHttpLabel().RemoveHttpsLabel();
			string[] array = ServerHelper.IsBetaSite ? ServerHelper.BetaDomainString : ServerHelper.DomainString;
			string pattern = (array != null && array.Length == 2) ? array[0] : "";
			return (text == text2 || text.ToLower() == new Regex(pattern).Replace(text2.ToLower(), "", 1)) && mContext.MRegProgress > 0 && mContext.MRegProgress < 15;
		}

		private static void GetActionParameters(IDictionary<string, object> parameters, out string email, out string password, out string orgid, out string token, out string locale)
		{
			object obj = parameters["_pmp"];
			password = ((obj == null) ? string.Empty : obj.ToString());
			object obj2 = parameters["_eme"];
			email = ((obj2 == null || string.IsNullOrWhiteSpace(obj2.ToString())) ? ContextHelper.MUserEmail : obj2.ToString());
			object obj3 = parameters["_omo"];
			orgid = ((obj3 == null) ? string.Empty : obj3.ToString());
			object obj4 = parameters["_tmt"];
			token = ((obj4 != null) ? obj4.ToString() : null);
			object obj5 = parameters.ContainsKey("_lml") ? parameters["_lml"] : null;
			locale = ((obj5 == null || string.IsNullOrWhiteSpace(obj5.ToString())) ? ContextHelper.MLocaleID : obj5.ToString());
		}

		private string UrlIntercept(string url, HttpContextBase httpContext)
		{
			string arg = httpContext.Request.QueryString["bti"];
			string arg2 = httpContext.Request.QueryString["lang"];
			if (url.IndexOf('?') >= 0)
			{
				url += string.Format("&bti={0}&lang={1}", arg, arg2);
			}
			else
			{
				url += string.Format("?bti={0}&lang={1}", arg, arg2);
			}
			return url;
		}
	}
}
