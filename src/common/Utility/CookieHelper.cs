using JieNor.Megi.Common.Context;
using System;
using System.Configuration;
using System.Web;

namespace JieNor.Megi.Common.Utility
{
	public class CookieHelper
	{
		private static readonly string _domain = ConfigurationManager.AppSettings["Domain"];

		public static void ClearCookie(string cookieName, string domain = null, HttpContextBase context = null)
		{
			HttpCookie cookie = (context != null) ? context.Request.Cookies[cookieName] : HttpContext.Current.Request.Cookies[cookieName];
			if (cookie != null)
			{
				cookie.Expires = DateTime.Now.AddYears(-1);
				cookie.Domain = (domain ?? _domain);
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
		}

		public static string GetCookieValue(string cookieName, HttpContextBase context = null)
		{
			string str = string.Empty;
			if (context == null && HttpContext.Current == null)
			{
				str = ContextHelper.MAccessTokenApp;
			}
			else
			{
				HttpCookie cookie = (context != null) ? context.Request.Cookies[cookieName] : HttpContext.Current.Request.Cookies[cookieName];
				if (cookie != null)
				{
					str = cookie.Value;
				}
			}
			return str;
		}

		public static void SetCookie(string cookieName, string cookieValue, DateTime expires, HttpContextBase context = null)
		{
			HttpCookie cookie = new HttpCookie(cookieName, cookieValue);
			cookie.Expires = expires;
			cookie.HttpOnly = true;
			if (context == null)
			{
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
			else
			{
				context.Response.Cookies.Add(cookie);
			}
		}

		public static void SetTempCookie(string cookieName, string cookieValue, HttpContextBase context = null)
		{
			HttpCookie cookie = new HttpCookie(cookieName, cookieValue);
			cookie.HttpOnly = true;
			if (context == null)
			{
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
			else
			{
				context.Response.Cookies.Add(cookie);
			}
		}

		public static void SetCookie(string cookieName, string cookieValue, DateTime expires, string domain, HttpContextBase context = null)
		{
			HttpCookie cookie = new HttpCookie(cookieName, cookieValue);
			cookie.Expires = expires;
			cookie.Domain = domain;
			cookie.HttpOnly = true;
			if (context == null)
			{
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
			else
			{
				context.Response.Cookies.Add(cookie);
			}
		}

		public static void SetCookie(string cookieName, string cookieValue, DateTime expires)
		{
			SetCookie(cookieName, cookieValue, expires, _domain, null);
		}

		public static void SetTempCookie(string cookieName, string cookieValue, string domain, HttpContextBase context = null)
		{
			HttpCookie cookie = new HttpCookie(cookieName, cookieValue);
			cookie.Domain = domain;
			cookie.HttpOnly = true;
			if (context == null)
			{
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
			else
			{
				context.Response.Cookies.Add(cookie);
			}
		}
	}
}
