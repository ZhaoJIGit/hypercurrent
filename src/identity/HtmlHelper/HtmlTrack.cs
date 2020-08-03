using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.AutoManager;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlTrack
	{
		public static MvcHtmlString ReportFilter()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Expected O, but got Unknown
			List<NameValueModel> trackList = BDTrackManager.GetTrackList();
			if (trackList == null || trackList.Count == 0)
			{
				return new MvcHtmlString("");
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			foreach (NameValueModel item in trackList)
			{
				stringBuilder.Append("<div class='m-form-item'>");
				stringBuilder.Append("<ul>");
				stringBuilder.AppendFormat("<li class='m-bold'>{0}{1}</li>", HtmlLang.Write(LangModule.Report, "TrackBy", "Filter by track:"), item.MName);
				stringBuilder.Append("<li>");
				stringBuilder.AppendFormat("<select id='selTrack{0}' class='easyui-combobox mg-data' style='width:140px; height:22px;' name='MTrackItem{0}'>", num);
				stringBuilder.AppendFormat("<option value='0'>{0}</option>", HtmlLang.Write(LangModule.Report, "DoNotFilter", "Do not filter"));
				stringBuilder.AppendFormat("<option value='1'>{0}</option>", HtmlLang.Write(LangModule.Report, "Unassigned", "Unassigned"));
				if (item.MChildren != null && item.MChildren.Count > 0)
				{
					foreach (NameValueModel mChild in item.MChildren)
					{
						stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", mChild.MValue, mChild.MName);
					}
				}
				stringBuilder.Append("</select>");
				stringBuilder.Append("</li>");
				stringBuilder.Append("</ul>");
				stringBuilder.Append("</div>");
				num++;
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static string AdvanceSearchFilter()
		{
			List<NameValueModel> trackList = BDTrackManager.GetTrackList();
			if (trackList == null || trackList.Count == 0)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			foreach (NameValueModel item in trackList)
			{
				stringBuilder.Append("<div class='item'>");
				stringBuilder.AppendFormat("<p>{0}</p>", MText.Encode(item.MName));
				stringBuilder.Append("<p>");
				stringBuilder.AppendFormat("<select class='easyui-combobox mg-data' data-options='' name='MTrackItem{0}' style='width:120px; height:22px;'>", num);
				stringBuilder.AppendFormat("<option value=''>{0}</option>", HtmlLang.Write(LangModule.Common, "All", "All"));
				if (item.MChildren != null && item.MChildren.Count > 0)
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", "-1", HtmlLang.Write(LangModule.IV, "Blankoptions ", "空白选项"));
					foreach (NameValueModel mChild in item.MChildren)
					{
						stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", mChild.MValue, MText.Encode(mChild.MName));
					}
				}
				stringBuilder.Append("</select>");
				stringBuilder.Append("</p>");
				stringBuilder.Append("</div>");
				num++;
			}
			return stringBuilder.ToString();
		}
	}
}
