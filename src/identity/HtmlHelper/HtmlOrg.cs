using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.AutoManager;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlOrg
	{
		private static int _regProgress = 0;

		public static int RegProgress
		{
			get
			{
				MContext mContext = ContextHelper.MContext;
				_regProgress = mContext.MRegProgress;
				return _regProgress;
			}
			set
			{
				_regProgress = value;
			}
		}

		public static string OrgChange(bool isMy)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<div id='divOrgDetails' class='mg-header-org'>");
			stringBuilder.Append($"<div><a href='{ServerHelper.MyServer}' class='mg-header-org-my'>My Hypercurrent<span class='right-arrow'></span></a></div>");
			string arg = "My Hypercurrent";
			MContext mContext = ContextHelper.MContext;
			List<BASMyHomeModel> orgInfoListByUserID = BASOrganisationManager.GetOrgInfoListByUserID();
			if (orgInfoListByUserID != null && orgInfoListByUserID.Count > 0)
			{
				stringBuilder.Append("<ul>");
				foreach (BASMyHomeModel item in orgInfoListByUserID)
				{
					if (!item.MOrgID.Equals(mContext.MOrgID))
					{
						stringBuilder.Append($"<li><a href='/Home/OrgSelect?MOrgID={item.MOrgID}&RedirectUrl={item.Url}'>{MText.Encode(item.MOrgName)}</a></li>");
					}
					else
					{
						stringBuilder.Append($"<li><a style='color:green' href='/Home/OrgSelect?MOrgID={item.MOrgID}&RedirectUrl={item.Url}'>{MText.Encode(item.MOrgName)}</a></li>");
					}
					if (!isMy && item.MOrgID.Equals(mContext.MOrgID))
					{
						arg = MText.Encode(item.MOrgName);
					}
				}
				stringBuilder.Append("</ul>");
			}
			stringBuilder.Append("</div>");
			return $"<a href='###' id='aOrgName' class='mg-header-org-name'>{arg}</a>{stringBuilder.ToString()}";
		}

		public static string GoOrgSelectList()
		{
			MContext context = ContextHelper.MContext;
			List<BASMyHomeModel> list = BASOrganisationManager.GetOrgInfoListByUserID();
			string title = "";
			string orgId = "";
			if (list != null)
			{
				BASMyHomeModel bASMyHomeModel = (from t in list
				where t.MOrgID == context.MOrgID
				select t).FirstOrDefault();
				if (bASMyHomeModel != null)
				{
					title = bASMyHomeModel.MOrgName;
					orgId = bASMyHomeModel.MOrgID;
					RegProgress = bASMyHomeModel.MRegProgress;
				}
				list = (from t in list
				where t.MOrgID != context.MOrgID
				select t).ToList();
			}
			return GetOrgSelectList(title, orgId, list);
		}

		public static string MyOrgSelectList()
		{
			string text = HtmlLang.GetText(LangModule.My, "MyMegi", "My Hypercurrent");
			List<BASMyHomeModel> orgInfoListByUserID = BASOrganisationManager.GetOrgInfoListByUserID();
			if (orgInfoListByUserID == null || orgInfoListByUserID.Count == 0)
			{
				return $"<span>{text}</span>";
			}
			return GetOrgSelectList(text, "", orgInfoListByUserID);
		}

		private static string GetOrgSelectList(string title, string orgId, List<BASMyHomeModel> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<div class=\"m-org\">");
			stringBuilder.AppendFormat("<a href=\"###\" id=\"aOrgList\" orgid='{1}'>{0}</a>", MText.Encode(title), orgId);
			stringBuilder.Append("</div>");
			stringBuilder.Append("<div id=\"divOrgList\" class=\"m-pop-box m-pop-menu\">");
			stringBuilder.Append("<b class=\"popup-arrow\"></b>");
			stringBuilder.Append("<div class=\"item-list\">");
			if (!string.IsNullOrEmpty(orgId))
			{
				stringBuilder.Append(string.Format("<p><a href='{0}/FW/FWHome/OrgSelect?MOrgID=&RedirectUrl={1}'>{2}</a></p>", ServerHelper.MyServer, ServerHelper.MyServer, HtmlLang.GetText(LangModule.My, "MyMegi", "My Hypercurrent")));
			}
			if (list != null && list.Count > 0)
			{
				foreach (BASMyHomeModel item in list)
				{
					string text = $"{ServerHelper.MyServer}/FW/FWHome/OrgSelect?MOrgID={item.MOrgID}&RedirectUrl={ServerHelper.GoServer}{item.Url}&IsBeta={item.MIsBeta}";
					string empty = string.Empty;
					empty = ((item.MRegProgress < 15) ? (string.IsNullOrEmpty(orgId) ? string.Format("$.mTab.add('{0}', '{1}', true)", HtmlLang.GetText(LangModule.My, "InitializeWizard", "Initialize Wizard"), text) : $"location.href='{$"{ServerHelper.MyServer}?RedirectUrl={HttpUtility.UrlEncode(text)}&redirectOnload=true"}'") : $"location.href='{text}'");
					stringBuilder.AppendFormat("<p><a href='###' onclick=\"{0}\">{1}</a></p>", empty, MText.Encode(item.MOrgName));
				}
			}
			stringBuilder.Append("</div>");
			stringBuilder.Append("</div>");
			return stringBuilder.ToString();
		}

		public static MvcHtmlString OrgChange()
		{
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Expected O, but got Unknown
			MContext mContext = ContextHelper.MContext;
			List<BASMyHomeModel> orgInfoListByUserID = BASOrganisationManager.GetOrgInfoListByUserID();
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			string text2 = HtmlLang.GetText(LangModule.My, "MyMegi", "My Hypercurrent");
			stringBuilder.Append("<div id=\"divOrgList\" class=\"m-pop-box m-pop-menu\">");
			stringBuilder.Append("<b class=\"popup-arrow\"></b>");
			if (orgInfoListByUserID != null && orgInfoListByUserID.Count > 0)
			{
				foreach (BASMyHomeModel item in orgInfoListByUserID)
				{
					if (!item.MOrgID.Equals(mContext.MOrgID))
					{
						stringBuilder.Append(string.Format("<p><a href='{0}/FW/FWHome/OrgSelect?MOrgID={1}&RedirectUrl={2}{3}&IsBeta={5}'>{4}</a></p>", ServerHelper.MyServer, item.MOrgID, item.Url, ServerHelper.GoServer, item.MOrgName, item.MIsBeta));
					}
					else
					{
						text = item.MOrgName;
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					text = text2;
				}
			}
			stringBuilder.Append($"<p><a href='{ServerHelper.MyServer}/FW/FWHome/OrgSelect?MOrgID=&RedirectUrl={ServerHelper.MyServer}'>{text2}</a></p>");
			stringBuilder.Append("</div>");
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("<div class=\"m-org\">");
			stringBuilder2.AppendFormat("<a href=\"###\" id=\"aOrgList\">{0}</a>", text);
			stringBuilder2.Append("</div>");
			return new MvcHtmlString($"{stringBuilder2}{stringBuilder.ToString()}");
		}
	}
}
