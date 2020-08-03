using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.EntityModel.Context;
using System.Configuration;
using System.Text;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlUser
	{
		private static readonly string _trackEvent = ConfigurationManager.AppSettings["TrackEvent"];

		public static string UserID
		{
			get
			{
				MContext mContext = ContextHelper.MContext;
				return mContext.MUserID;
			}
		}

		public static string UserEmail
		{
			get
			{
				MContext mContext = ContextHelper.MContext;
				return mContext.MEmail;
			}
		}

		public static string UserName()
		{
			MContext mContext = ContextHelper.MContext;
			return GlobalFormat.GetUserName(mContext.MFirstName, mContext.MLastName, mContext);
		}

		public static string VersionScript()
		{
			MContext mContext = ContextHelper.MContext;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<script type='text/javascript'>");
			stringBuilder.AppendFormat("window.VersionNumber='{0}';", ServerHelper.VersionNumber);
			stringBuilder.Append("</script>");
			return stringBuilder.ToString();
		}
	}
}
