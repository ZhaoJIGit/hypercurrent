using JieNor.Megi.Core;
using System.Linq;
using System.Web;

namespace JieNor.Megi.Tools.Resubmit
{
	public class SessionPageTokenView : PageTokenViewBase
	{
		public override string GetLastPageToken
		{
			get
			{
				if (!HttpContext.Current.Request.Headers.AllKeys.Contains(PageTokenViewBase.HiddenTokenName))
				{
					return null;
				}
				return HttpContext.Current.Request.Headers[PageTokenViewBase.HiddenTokenName];
			}
		}

		public override string GetLastPageID
		{
			get
			{
				if (!HttpContext.Current.Request.Headers.AllKeys.Contains(PageTokenViewBase.HiddenPageIDName))
				{
					return null;
				}
				return HttpContext.Current.Request.Headers[PageTokenViewBase.HiddenPageIDName];
			}
		}

		public override bool TokensMatch
		{
			get
			{
				string getLastPageToken = GetLastPageToken;
				string getLastPageID = GetLastPageID;
				if (!string.IsNullOrWhiteSpace(getLastPageToken))
				{
					if (string.IsNullOrWhiteSpace(getLastPageID))
					{
						return true;
					}
					if (HttpContext.Current.Session[getLastPageID] != null && !string.IsNullOrWhiteSpace(HttpContext.Current.Session[getLastPageID].ToString()) && !getLastPageToken.Equals(GeneratePageToken(getLastPageID)))
					{
						return false;
					}
					HttpContext.Current.Session[getLastPageID] = UUIDHelper.GetGuid();
					return true;
				}
				return true;
			}
		}

		public override string GeneratePageToken(string pageId)
		{
			if (HttpContext.Current.Session[pageId] != null)
			{
				return HttpContext.Current.Session[pageId].ToString();
			}
			string guid = UUIDHelper.GetGuid();
			HttpContext.Current.Session[pageId] = guid;
			return guid;
		}
	}
}
