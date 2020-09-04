using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Identity.HtmlHelper
{
    public static class HtmlLang
    {
        public static string LangID => LangIndentity.CurrentLangID;

        private static string DefaultLangs => "0x0009,0x7804;English,中文(简体)";

        public static string GetText(LangKey key)
        {
            return LangHelper.GetText("", LangModule.Common, Convert.ToInt32(key), "");
        }

        public static string GetText(LangModule langModule, LangKey key)
        {
            return LangHelper.GetText("", langModule, Convert.ToInt32(key), "");
        }

        public static string GetText(LangModule langModule, LangKey key, string defaultValue)
        {
            return LangHelper.GetText("", langModule, Convert.ToInt32(key), defaultValue);
        }

        public static string GetText(LangModule langModule, string key)
        {
            return LangHelper.GetText(LangIndentity.CurrentLangID, langModule, key, "");
        }

        public static string GetText(LangModule langModule, string key, string defaultValue)
        {
            return LangHelper.GetText(LangIndentity.CurrentLangID, langModule, key, defaultValue);
        }

        public static MvcHtmlString Write(LangKey key)
        {
            //IL_001c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0021: Expected O, but got Unknown
            return new MvcHtmlString(LangHelper.GetText("", LangModule.Common, Convert.ToInt32(key), ""));
        }

        public static MvcHtmlString Write(LangModule module, string key, string defaultValue = null)
        {
            
            //IL_0009: Unknown result type (might be due to invalid IL or missing references)
            //IL_000e: Expected O, but got Unknown
            return new MvcHtmlString(GetText(module, key, defaultValue));
        }

        public static MvcHtmlString Write(LangModule module, int key, string defaultValue)
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0013: Expected O, but got Unknown
            return new MvcHtmlString(LangHelper.GetText(LangIndentity.CurrentLangID, module, key, defaultValue));
        }

        public static MvcHtmlString WriteFormat(LangModule module, string key, string defaultValue, params object[] parameters)
        {
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_0019: Expected O, but got Unknown
            return new MvcHtmlString(string.Format(LangHelper.GetText(LangIndentity.CurrentLangID, module, key, defaultValue), parameters));
        }

        public static MvcHtmlString WriteFormat(LangModule module, int key, string defaultValue, params object[] parameters)
        {
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_0019: Expected O, but got Unknown
            return new MvcHtmlString(string.Format(LangHelper.GetText(LangIndentity.CurrentLangID, module, key, defaultValue), parameters));
        }

        public static MvcHtmlString WriteScript(LangModule module)
        {
            //IL_003b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0040: Expected O, but got Unknown
            string text = $"<script type=\"text/javascript\" src=\"{ServerHelper.StaticServer}/Lang/JieNor.Megi.{module.ToString()}.{LangIndentity.CurrentLangID}.js?{ServerHelper.JSVersion}\"></script>";
            return new MvcHtmlString(text);
        }

        public static MvcHtmlString WriteEasyuiLocalJs(string langId)
        {
            //IL_0058: Unknown result type (might be due to invalid IL or missing references)
            //IL_005d: Expected O, but got Unknown
            string text = "";
            if (langId == LangCodeEnum.ZH_TW)
            {
                text = "easyui-lang-zh_TW.js";
            }
            else if (langId == LangCodeEnum.ZH_CN)
            {
                text = "easyui-lang-zh_CN.js";
            }
            string text2 = string.IsNullOrEmpty(text) ? "" : $"<script type='text/javascript' src='{ServerHelper.StaticServer}/FW/include/easyui/locale/{text}'></script>";
            return new MvcHtmlString(text2);
        }

        private static string GetLangOptionsHtml(List<BASLangModel> list)
        {
            string currentLangID = LangIndentity.CurrentLangID;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (BASLangModel item in list)
            {
                if (item.LangID == currentLangID)
                {
                    stringBuilder.Append($"<option value='{item.LangID}'  selected='selected'>{item.LangName}</option>");
                }
                else
                {
                    stringBuilder.Append($"<option value='{item.LangID}'>{item.LangName}</option>");
                }
            }
            return stringBuilder.ToString();
        }

        public static string ToLangDate(this DateTime dt)
        {
            return dt.ToDateString();
        }

        public static string DateNowString()
        {
            MContext mContext = ContextHelper.MContext;
            return mContext.DateNow.ToDateString();
        }

        public static string GetCurrentMonthDate(bool isMonthFirst)
        {
            DateTime date = DateTime.Now;
            if (isMonthFirst)
            {
                date = date.AddDays((double)(1 - date.Day));
            }
            else
            {
                DateTime dateTime = date.AddDays((double)(1 - date.Day));
                dateTime = dateTime.AddMonths(1);
                date = dateTime.AddDays(-1.0);
            }
            return date.ToDateString();
        }

        public static MvcHtmlString LangBar(ServerType type)
        {
            //IL_01b8: Unknown result type (might be due to invalid IL or missing references)
            //IL_01bd: Expected O, but got Unknown
            string text = ConfigurationManager.AppSettings["langTypes"];
            StringBuilder stringBuilder = new StringBuilder();
            if (text != "-1")
            {
                text = ((string.IsNullOrEmpty(text) || text.Split(';').Length != 2 || text.Split(';')[0].Split(',').Length == 0) ? DefaultLangs : text);
                string langID = LangID;
                string[] array = text.Split(';')[0].Split(',');
                string[] array2 = text.Split(';')[1].Split(',');
                string[] source = (type == ServerType.GoServer) ? (from x in LangIndentity.GetOrgLangList()
                                                                   select x.LangID).ToArray() : array;
                bool value = type == ServerType.LoginServer || type == ServerType.MainServer;
                for (int i = 0; i < array.Length; i++)
                {
                    if (source.Contains(array[i]) && LangID != array[i])
                    {
                        stringBuilder.AppendFormat("<a href='javascript:void(0)' id='" + array[i] + "' onclick=\"mUtil.ChangeLang('" + array[i] + "',{0} ,'' , {1})\" class=''>" + array2[i] + "</a>", (int)type, Convert.ToString(value).ToLower());
                    }
                }
            }
            return new MvcHtmlString(stringBuilder.ToString());
        }

        public static string GlobalInfo()
        {
            return LangIndentity.GetClientGlobalInfo();
        }

        public static string OrgLang()
        {
            string langId = LangIndentity.CurrentLangID;
            List<BASLangModel> orgLangList = LangIndentity.GetOrgLangList();
            List<BASLangModel> list = new List<BASLangModel>();
            list.Add((from t in orgLangList
                      where t.LangID == langId
                      select t).FirstOrDefault());
            list.AddRange((from t in orgLangList
                           where t.LangID != langId
                           select t).ToList());
            return new JavaScriptSerializer().Serialize(from f in list
                                                        where f != null
                                                        select f);
        }

        public static MvcHtmlString InputCheckBoxs()
        {
            //IL_0072: Unknown result type (might be due to invalid IL or missing references)
            //IL_0077: Expected O, but got Unknown
            List<BASLangModel> sysLangList = LangIndentity.GetSysLangList();
            StringBuilder stringBuilder = new StringBuilder();
            if (sysLangList != null)
            {
                string langID = LangID;
                foreach (BASLangModel item in sysLangList)
                {
                    stringBuilder.AppendFormat("<div class='lang-item'><input type='checkbox' name='language' value='{0}' {2}/>{1}</div>", item.LangID, item.LangName, "disabled='disabled'");
                }
            }
            return new MvcHtmlString(stringBuilder.ToString());
        }

        public static MvcHtmlString OrgLangOptions()
        {
            //IL_000d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0012: Expected O, but got Unknown
            List<BASLangModel> orgLangList = LangIndentity.GetOrgLangList();
            return new MvcHtmlString(GetLangOptionsHtml(orgLangList));
        }

        public static MvcHtmlString OrgLangSelect()
        {
            //IL_0020: Unknown result type (might be due to invalid IL or missing references)
            //IL_0025: Expected O, but got Unknown
            //IL_0060: Unknown result type (might be due to invalid IL or missing references)
            //IL_0065: Expected O, but got Unknown
            List<BASLangModel> orgLangList = LangIndentity.GetOrgLangList();
            if (orgLangList == null || orgLangList.Count < 2)
            {
                return new MvcHtmlString("");
            }
            string currentLangID = LangIndentity.CurrentLangID;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<select class=\"easyui-combobox\" data-options=\"onSelect:function(record){Megi.changeLang(record,'/Framework/ChangeLang')}\">");
            stringBuilder.Append(GetLangOptionsHtml(orgLangList));
            stringBuilder.Append("</select>");
            return new MvcHtmlString(stringBuilder.ToString());
        }

        public static MvcHtmlString GetFeedBackLink(bool isMy = true)
        {
            //IL_0046: Unknown result type (might be due to invalid IL or missing references)
            //IL_004b: Expected O, but got Unknown
            string text = GetText(LangModule.My, "IWantToFeedback", "I want to feedback");
            string text2 = GetText(LangModule.My, "Feedback", "Feedback");
            string text3 = string.Format("<a class=\"m-feedback-link\" onclick=\"$.mDialog.show({{title:'{0}', width:650, height:450, href:'{1}/Feed/FeedBack/FeedIndex' }});\">{2}</a>", text, isMy ? string.Empty : ServerHelper.MyServer, "&nbsp;");
            return new MvcHtmlString(text3);
        }
    }
}
