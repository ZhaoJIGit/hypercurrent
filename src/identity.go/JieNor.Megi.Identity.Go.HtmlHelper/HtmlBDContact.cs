using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.Go.AutoManager;
using JieNor.Megi.Identity.HtmlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlBDContact
	{
		public static MvcHtmlString SelectOptions()
		{
			List<BDContactsInfoModel> contactsInfo = BDContactManager.GetContactsInfo();
			StringBuilder stringBuilder = new StringBuilder();
			if (contactsInfo != null)
			{
				foreach (BDContactsInfoModel item in contactsInfo)
				{
					stringBuilder.Append($"<option value=\"{item.MItemID}\">{item.MName}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static string DataOptions()
		{
			List<BDContactsInfoModel> contactsInfo = BDContactManager.GetContactsInfo();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			if (contactsInfo != null && contactsInfo.Count > 0)
			{
				foreach (BDContactsInfoModel item in contactsInfo)
				{
					stringBuilder.AppendFormat("{{ItemID:\"{0}\",ItemName:\"{1}\",MIsSupplier:{2},MIsCustomer:{3},MIsOther:{4},MCCurrentAccountID:\"{5}\",MCCurrentAccountCode:\"{6}\" }},", item.MItemID, item.MName, item.MIsSupplier ? "true" : "false", item.MIsCustomer ? "true" : "false", item.MIsOther ? "true" : "false", item.MCCurrentAccountId, item.MCCurrentAccountCode);
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public static MvcHtmlString ContactViewHtml(string contactID, bool isPersonInfo = true)
		{
			BDContactsInfoModel contactViewData = BDContactManager.GetContactViewData(contactID);
			contactViewData = (BDContactsInfoModel)MText.JsonEncode(contactViewData);
			StringBuilder stringBuilder = new StringBuilder();
			if (contactViewData != null)
			{
				if (!string.IsNullOrEmpty(contactViewData.MSalTaxTypeID))
				{
					BDContactsInfoModel bDContactsInfoModel = contactViewData;
					bDContactsInfoModel.MSalTaxTypeID += string.Format("({0})", Math.Round(contactViewData.MSalTaxRate, 2) + "%");
				}
				if (!string.IsNullOrWhiteSpace(contactViewData.MPurTaxTypeID))
				{
					BDContactsInfoModel bDContactsInfoModel2 = contactViewData;
					bDContactsInfoModel2.MPurTaxTypeID += string.Format("({0})", Math.Round(contactViewData.MPurTaxRate, 2) + "%");
				}
				List<PropertyInfo> list = new List<PropertyInfo>(typeof(BDContactsInfoModel).GetProperties());
				List<BDContactViewModel> modelProperty = GetModelProperty(isPersonInfo, contactViewData);
				foreach (string item in (from s in modelProperty
				select s.ColumnName).ToList())
				{
					PropertyInfo info = list.Find((PropertyInfo p) => p.Name == item);
					stringBuilder.Append(formatHtmlString(list, info, contactViewData, modelProperty));
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		private static List<BDContactViewModel> GetModelProperty(bool isPersonInfo, BDContactsInfoModel model)
		{
			List<BDContactViewModel> list = new List<BDContactViewModel>();
			MContext mContext = ContextHelper.MContext;
			if (isPersonInfo)
			{
				list.Add(retModel("MName", GetContactTitle("ContactName", "Contact Name", mContext.MLCID), null));
				list.Add(retModel("MEmail", GetContactTitle("Email", "Email", mContext.MLCID), null));
				list.Add(retModel("MLastName", GetContactTitle("ContactPerson", "Contact Person", mContext.MLCID), null));
				list.Add(retModel("MPhone", GetContactTitle("Phone", "Phone", mContext.MLCID), null));
				list.Add(retModel("MFax", GetContactTitle("Fax", "Fax ", mContext.MLCID), null));
				list.Add(retModel("MMobile", GetContactTitle("Mobile", "Mobile", mContext.MLCID), null));
				list.Add(retModel("MDirectPhone", GetContactTitle("DirectDial", "Direct Dial", mContext.MLCID), null));
				list.Add(retModel("MSkypeName", GetContactTitle("Skype", "Skype", mContext.MLCID), null));
				list.Add(retModel("MPCountryName", GetContactTitle("PostalAddress", "Postal Address", mContext.MLCID), null));
				list.Add(retModel("MRealCountryName", GetContactTitle("StreetAddress", "Street Address", mContext.MLCID), null));
				list.Add(retModel("MWebsite", GetContactTitle("Website", "Website", mContext.MLCID), null));
			}
			else
			{
				list.Add(retModel("MTaxNo", GetContactTitle("TaxIDNumber", "Tax ID Number", mContext.MLCID), null));
				list.Add(retModel("MSalTaxTypeID", GetContactTitle("SalesTax", "Sales Tax", mContext.MLCID), null));
				list.Add(retModel("MPurTaxTypeID", GetContactTitle("PurchasesTax", "Purchases Tax", mContext.MLCID), null));
				if (mContext.MRegProgress == 15)
				{
					list.Add(retModel("AccountFullName", GetContactTitle("CurrentMoneyAccount", "往来科目", mContext.MLCID), null));
				}
				list.Add(retModel("MDiscount", GetContactTitle("DiscountTitle", "Discount", mContext.MLCID), null));
				list.Add(retModel("MDefaultCyID", GetContactTitle("DefaultCurrency", "Default Currency", mContext.MLCID), null));
				if (model.MIsSupplier || model.MIsOther)
				{
					list.Add(retModel("MPurDueCondition", GetContactTitle("BillsTerms", "Bills Terms", mContext.MLCID), null));
				}
				if (model.MIsCustomer || model.MIsOther)
				{
					list.Add(retModel("MSalDueCondition", GetContactTitle("SalesTerms", "Sales Terms", mContext.MLCID), null));
				}
				list.Add(retModel("MBankName", GetContactTitle("BankInfo", "Bank infomation", mContext.MLCID), null));
			}
			return list;
		}

		private static string GetContactTitle(string key, string defaultValue, string localeId = "0x0009")
		{
			return LangHelper.GetText(localeId, LangModule.Contact, key, defaultValue);
		}

		private static BDContactViewModel retModel(string ColumnName, string ColumnTitle, string category = null)
		{
			return new BDContactViewModel
			{
				ColumnName = ColumnName,
				ColumnTitle = ColumnTitle
			};
		}

		private static string formatHtmlString(List<PropertyInfo> plist, PropertyInfo info, BDContactsInfoModel model, List<BDContactViewModel> viewModel)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetByColumnName(plist, info, model, viewModel));
			return stringBuilder.ToString();
		}

		private static string GetByColumnName(List<PropertyInfo> plist, PropertyInfo info, BDContactsInfoModel model, List<BDContactViewModel> viewModel)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = Convert.ToString(info.GetValue(model));
			string columnTitle = (from w in viewModel
			where w.ColumnName == info.Name
			select w).FirstOrDefault().ColumnTitle;
			MContext mContext = ContextHelper.MContext;
			string text2 = LangHelper.GetText(mContext.MLCID, LangModule.Contact, "None", "无");
			string text3 = $"<span class='m-empty-data'>{text2}</span>";
			switch (info.Name)
			{
			case "MEmail":
				text = (string.IsNullOrWhiteSpace(text) ? text3 : string.Format("<a href=\"mailto:{0}\">{0}</a>", text));
				stringBuilder.Append($"<div class=\"field\"><div class='title'>{columnTitle}</div><div>{text}</div></div>");
				break;
			case "MLastName":
				stringBuilder.Append(GetColumnUnionStr(mContext, plist, model, columnTitle, info, "MFirstName,MLastName"));
				break;
			case "MPCountryName":
				stringBuilder.Append(GetColumnUnionStr(mContext, plist, model, columnTitle, info, "MPAttention,MPStreet,MPCityID,MPRegion,MPPostalNo,MPCountryName"));
				break;
			case "MRealCountryName":
				stringBuilder.Append(GetColumnUnionStr(mContext, plist, model, columnTitle, info, "MRealAttention,MRealStreet,MRealCityID,MRealRegion,MRealPostalNo,MRealCountryName"));
				break;
			case "MPurDueCondition":
				stringBuilder.Append(GetDueDateStr(plist, model, columnTitle, "MPurDueDate", text));
				break;
			case "MSalDueCondition":
				stringBuilder.Append(GetDueDateStr(plist, model, columnTitle, "MSalDueDate", text));
				break;
			case "MDiscount":
				text = string.Empty;
				if (Convert.ToDecimal(info.GetValue(model)) > decimal.Zero)
				{
					text = Convert.ToString(Math.Round(Convert.ToDecimal(info.GetValue(model)), 2)) + "%";
				}
				text = (string.IsNullOrWhiteSpace(text) ? text3 : text);
				stringBuilder.Append($"<div class=\"field\"><div class='title'>{columnTitle}</div><div class='m-empty-data'>{text}</div></div>");
				break;
			case "MWebsite":
				if (!string.IsNullOrWhiteSpace(text))
				{
					string empty = string.Empty;
					empty = (text.ToLower().StartsWith("http") ? text : (("http://" + text) ?? ""));
					text = string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", empty);
				}
				else
				{
					text = text3;
				}
				stringBuilder.Append($"<div class=\"field\"><div class='title'>{columnTitle}</div><div>{text}</div></div>");
				break;
			case "MPhone":
			case "MFax":
			case "MMobile":
			case "MDirectPhone":
				text = (string.IsNullOrWhiteSpace(text) ? text3 : text);
				stringBuilder.Append($"<div class=\"field\"><div class='title'>{columnTitle}</div><span class='m-empty-data'>{text}</span></div>");
				break;
			case "MBankName":
				text = GetColumnUnionStr(mContext, plist, model, columnTitle, info, "MBankName,MBankAcctNo,MBankAccName");
				text = (string.IsNullOrWhiteSpace(text) ? text2 : text);
				stringBuilder.Append($"<div class=\"field\"><div class='title'>{columnTitle}</div><div class='m-empty-data'>{text}</div></div>");
				break;
			default:
				text = (string.IsNullOrWhiteSpace(text) ? text3 : text);
				stringBuilder.Append(string.Format("<div class=\"field\"><div class='title'>{0}</div><div style='text-overflow: ellipsis;white-space: nowrap;overflow: hidden;width: 170px;'>{1}</div></div>", columnTitle, text, text));
				break;
			}
			return stringBuilder.ToString();
		}

		private static string GetColumnUnionStr(MContext ctx, List<PropertyInfo> plist, BDContactsInfoModel model, string title, PropertyInfo info, string UnionColumn)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (plist.FindAll((PropertyInfo p) => UnionColumn.Contains(p.Name)).Exists((PropertyInfo u) => !string.IsNullOrWhiteSpace(Convert.ToString(u.GetValue(model)))))
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				string format = "<div class=\"field\"><div class='title'>{0}</div><div>{1}</div></div>";
				switch (info.Name)
				{
				case "MLastName":
					stringBuilder2.AppendFormat("{0} {1}", model.MFirstName, model.MLastName);
					stringBuilder.Append(string.Format(format, title, stringBuilder2.ToString().EmptyToNone(null)));
					break;
				case "MPCountryName":
				{
					string text = (!string.IsNullOrEmpty(model.MPAttention)) ? string.Format("{0}:{1}<br/>", HtmlLang.Write(LangModule.Contact, "Attention", "Attention"), model.MPAttention) : "";
					string text2 = (!string.IsNullOrEmpty(model.MPStreet)) ? (model.MPStreet + "<br/>") : "";
					string text3 = (!string.IsNullOrEmpty(model.MPCityID)) ? (model.MPCityID + "<br/>") : "";
					string text4 = (!string.IsNullOrEmpty(model.MPRegion)) ? (model.MPRegion + "<br/>") : "";
					string text5 = (!string.IsNullOrEmpty(model.MPPostalNo)) ? (model.MPPostalNo + "<br/>") : "";
					string text6 = (!string.IsNullOrEmpty(model.MPCountryName)) ? (model.MPCountryName + "<br/>") : "";
					if (ctx.MLCID == "0x0009")
					{
						stringBuilder2.AppendFormat("{0}{1}{2}{3}{4}{5}", text, text2, text3, text4, text6, text5);
					}
					else
					{
						stringBuilder2.AppendFormat("{0}{5}{3}{2}{1}{4}", text, text2, text3, text4, text5, text6);
					}
					stringBuilder.Append(string.Format(format, title, stringBuilder2.ToString().TrimEnd("<br/>".ToCharArray()).EmptyToNone(null)));
					break;
				}
				case "MRealCountryName":
				{
					string text7 = (!string.IsNullOrEmpty(model.MRealAttention)) ? string.Format("{0}:{1}<br/>", HtmlLang.Write(LangModule.Contact, "Attention", "Attention"), model.MRealAttention) : "";
					string text8 = (!string.IsNullOrEmpty(model.MRealStreet)) ? (model.MRealStreet + "<br/>") : "";
					string text9 = (!string.IsNullOrEmpty(model.MRealCityID)) ? (model.MRealCityID + "<br/>") : "";
					string text10 = (!string.IsNullOrEmpty(model.MRealRegion)) ? (model.MRealRegion + "<br/>") : "";
					string text11 = (!string.IsNullOrEmpty(model.MRealPostalNo)) ? (model.MRealPostalNo + "<br/>") : "";
					string text12 = (!string.IsNullOrEmpty(model.MRealCountryName)) ? (model.MRealCountryName + "<br/>") : "";
					if (ctx.MLCID == "0x0009")
					{
						stringBuilder2.AppendFormat("{0}{1}{2}{3}{4}{5}", text7, text8, text9, text10, text12, text11);
					}
					else
					{
						stringBuilder2.AppendFormat("{0}{5}{3}{2}{1}{4}", text7, text8, text9, text10, text11, text12);
					}
					stringBuilder.Append(string.Format(format, title, stringBuilder2.ToString().TrimEnd("<br/>".ToCharArray()).EmptyToNone(null)));
					break;
				}
				case "MBankName":
				{
					string arg = string.IsNullOrWhiteSpace(model.MBankName) ? "" : model.MBankName;
					string arg2 = string.IsNullOrWhiteSpace(model.MBankAcctNo) ? "" : model.MBankAcctNo;
					string arg3 = string.IsNullOrWhiteSpace(model.MBankAccName) ? "" : model.MBankAccName;
					stringBuilder.Append($"{arg3} {arg2} {arg}");
					break;
				}
				}
			}
			return stringBuilder.ToString();
		}

		private static string GetDueDateStr(List<PropertyInfo> plist, BDContactsInfoModel model, string title, string key, string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int dueDate = Convert.ToInt32(plist.Find((PropertyInfo f) => f.Name == key).GetValue(model));
			stringBuilder.Append(GetUnionString(dueDate, value, title));
			return stringBuilder.ToString();
		}

		private static string GetUnionString(int DueDate, string DueCondition, string title)
		{
			string arg = default(string);
			switch (DueCondition)
			{
			case "item0":
				arg = string.Format(HtmlLang.Write(LangModule.Contact, "OftheFollowingMonthA", " 至下月的{0}日 ").ToString(), DueDate);
				break;
			case "item1":
				arg = string.Format(HtmlLang.Write(LangModule.Contact, "Day(s)AftertheBillDateA", " 账单日后的第{0}天 ").ToString(), DueDate);
				break;
			case "item2":
				arg = string.Format(HtmlLang.Write(LangModule.Contact, "Day(s)AftertheEndOfThebillMonth", " 账单次月的第{0}天 ").ToString(), DueDate);
				break;
			case "item3":
				arg = string.Format(HtmlLang.Write(LangModule.Contact, "OfTheCurrentMonthA", " 账单当月的{0}日 ").ToString(), DueDate);
				break;
			}
			return $"<div class=\"field\"><div class='title'>{title}</div><span class='m-empty-data'>{arg}</span></div>";
		}

		private static string GetTeleInfoStr(string title, string value)
		{
			string text = string.Empty;
			string[] array = value.Split('-');
			if (array.Length > 2)
			{
				text += (string.IsNullOrWhiteSpace(array[0]) ? "" : array[0]);
				text += (string.IsNullOrWhiteSpace(array[1]) ? "" : (string.IsNullOrWhiteSpace(text) ? array[1] : ("-" + array[1])));
				text += (string.IsNullOrWhiteSpace(array[2]) ? "" : (string.IsNullOrWhiteSpace(text) ? array[2] : ("-" + array[2])));
			}
			return $"<div class=\"field\"><div class='title'>{title}</div><span>{text.EmptyToNone(null)}</span></div>";
		}
	}
}
