using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BD;
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
	public static class HtmlBDEmployee
	{
		public static MvcHtmlString SelectOptions(string id)
		{
			List<BDEmployeesModel> list = new List<BDEmployeesModel>();
			list.Add(BDEmployeeManager.GetEmployeeViewData(id));
			StringBuilder stringBuilder = new StringBuilder();
			if (list != null)
			{
				foreach (BDEmployeesModel item in list)
				{
					stringBuilder.Append($"<option value=\"{item.MItemID}\">{item.Name}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString EmployeeViewHtml(string id, object data, bool isPersonInfo = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			BDEmployeesModel bDEmployeesModel = new BDEmployeesModel();
			bDEmployeesModel = ((data != null) ? ((BDEmployeesModel)data) : BDEmployeeManager.GetEmployeeViewData(id));
			if (bDEmployeesModel != null)
			{
				List<PropertyInfo> list = new List<PropertyInfo>(typeof(BDEmployeesModel).GetProperties());
				List<BDEmployeeViewModel> modelProperty = GetModelProperty(isPersonInfo);
				foreach (string item in (from s in modelProperty
				select s.ColumnName).ToList())
				{
					PropertyInfo propertyInfo = list.Find((PropertyInfo p) => p.Name == item);
					if (!(propertyInfo == (PropertyInfo)null))
					{
						stringBuilder.Append(formatHtmlString(list, propertyInfo, bDEmployeesModel, modelProperty));
					}
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		private static List<BDEmployeeViewModel> GetModelProperty(bool isPersonInfo)
		{
			List<BDEmployeeViewModel> list = new List<BDEmployeeViewModel>();
			if (isPersonInfo)
			{
				list.Add(retModel("LastName", "Contact Person"));
				list.Add(retModel("MEmail", "Email"));
				list.Add(retModel("MPhone", "Phone"));
				list.Add(retModel("MFax", "Fax "));
				list.Add(retModel("MMobile", "Mobile"));
				list.Add(retModel("MDirectPhone", "Direct Dial"));
				list.Add(retModel("MPCountryID", "Postal Address"));
				list.Add(retModel("MRealCountryID", "Street Address"));
				list.Add(retModel("MSkypeName", "Skype"));
				list.Add(retModel("MWebsite", "Website"));
			}
			else
			{
				list.Add(retModel("MTaxNo", "Tax ID Number"));
				list.Add(retModel("MSalTaxTypeID", "Sales Tax"));
				list.Add(retModel("MPurTaxTypeID", "Purchases Tax"));
				list.Add(retModel("MRecAcctID", "Sales Account"));
				list.Add(retModel("MPayAcctID", "Purchases Account"));
				list.Add(retModel("MDiscount", "Sales Discount"));
				list.Add(retModel("MDefaultCyID", "Default Currency"));
				list.Add(retModel("MBankName", "Details"));
				list.Add(retModel("MPurDueCondition", "Bills Terms"));
				list.Add(retModel("MSalDueCondition", "Sales Terms"));
			}
			return list;
		}

		private static BDEmployeeViewModel retModel(string ColumnName, string ColumnTitle)
		{
			return new BDEmployeeViewModel
			{
				ColumnName = ColumnName,
				ColumnTitle = ColumnTitle
			};
		}

		private static string formatHtmlString(List<PropertyInfo> plist, PropertyInfo info, BDEmployeesModel model, List<BDEmployeeViewModel> viewModel)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetByColumnName(plist, info, model, viewModel));
			return stringBuilder.ToString();
		}

		private static string GetByColumnName(List<PropertyInfo> plist, PropertyInfo info, BDEmployeesModel model, List<BDEmployeeViewModel> viewModel)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (info == (PropertyInfo)null)
			{
				return null;
			}
			string text = Convert.ToString(info.GetValue(model));
			string columnTitle = (from w in viewModel
			where w.ColumnName == info.Name
			select w).FirstOrDefault().ColumnTitle;
			switch (info.Name)
			{
			case "MEmail":
				stringBuilder.Append(string.Format("<div class=\"field\"><label>{0}</label><span><a href=\"mailto:{1}\">{1}</a></span></div>", columnTitle, text));
				break;
			case "LastName":
				stringBuilder.Append(GetColumnUnionStr(plist, model, columnTitle, info, "MFirstName,MLastName"));
				break;
			case "MPCountryID":
				stringBuilder.Append(GetColumnUnionStr(plist, model, columnTitle, info, "MPAttention,MPStreet,MPCityID,MPRegion,MPPostalNo,MPCountryID"));
				break;
			case "MRealCountryID":
				stringBuilder.Append(GetColumnUnionStr(plist, model, columnTitle, info, "MRealAttention,MRealStreet,MRealCityID,MRealRegion,MRealPostalNo,MRealCountryID"));
				break;
			case "MPurDueCondition":
				stringBuilder.Append(GetDueDateStr(plist, model, columnTitle, "MPurDueDate", text));
				break;
			case "MSalDueCondition":
				stringBuilder.Append(GetDueDateStr(plist, model, columnTitle, "MSalDueDate", text));
				break;
			case "MDiscount":
				if (Convert.ToDecimal(info.GetValue(model)) > decimal.Zero)
				{
					stringBuilder.Append(string.Format("<div class=\"field\"><label>{0}</label><span>{1}</span></div>", columnTitle, Convert.ToString(Math.Round(Convert.ToDecimal(info.GetValue(model)), 2)) + "%"));
				}
				break;
			case "MWebsite":
				if (!string.IsNullOrWhiteSpace(text))
				{
					stringBuilder.Append(string.Format("<div class=\"field\"><label>{0}</label><span><a href=\"http://{1}\">{1}</a></span></div>", columnTitle, text));
				}
				break;
			case "MPhone":
			case "MFax":
			case "MMobile":
			case "MDirectPhone":
				stringBuilder.Append(GetTeleInfoStr(columnTitle, text));
				break;
			default:
				if (!string.IsNullOrWhiteSpace(text))
				{
					stringBuilder.Append($"<div class=\"field\"><label>{columnTitle}</label><span>{text}</span></div>");
				}
				break;
			}
			return stringBuilder.ToString();
		}

		private static string GetColumnUnionStr(List<PropertyInfo> plist, BDEmployeesModel model, string title, PropertyInfo info, string UnionColumn)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (plist.FindAll((PropertyInfo p) => UnionColumn.Contains(p.Name)).Exists((PropertyInfo u) => !string.IsNullOrWhiteSpace(Convert.ToString(u.GetValue(model)))))
			{
				string str = string.Empty;
				string empty = string.Empty;
				switch (info.Name)
				{
				case "LastName":
					stringBuilder.Append($"<div class=\"field\"><label>{title}</label><span>{model.Name}</span></div>");
					break;
				case "MPCountryID":
					if (GetMultielementColumnValue("MPAttention", model, out empty))
					{
						str = HtmlLang.Write(LangModule.Contact, "Attention", "Attention") + ":" + empty + "<br />";
					}
					if (GetMultielementColumnValue("MPStreet", model, out empty))
					{
						str = str + empty + "<br />";
					}
					str = str + model.MPCityID + "<br />";
					if (GetMultielementColumnValue("MPRegion", model, out empty))
					{
						str = str + empty + " ";
					}
					str += model.MPPostalNo;
					if (GetMultielementColumnValue("MPRegion", model, out empty) || model.MPPostalNo != null)
					{
						str += "<br />";
					}
					str += model.MPCountryID;
					stringBuilder.Append($"<div class=\"field\"><label>{title}</label><span>{str}</span></div>");
					break;
				case "MRealCountryID":
					if (GetMultielementColumnValue("MRealAttention", model, out empty))
					{
						str = HtmlLang.Write(LangModule.Contact, "Attention", "Attention") + ":" + empty + "<br />";
					}
					if (GetMultielementColumnValue("MRealStreet", model, out empty))
					{
						str = str + empty + "<br />";
					}
					str = str + model.MRealCityID + "<br />";
					if (GetMultielementColumnValue("MRealRegion", model, out empty))
					{
						str = str + empty + " ";
					}
					str += model.MRealPostalNo;
					if (GetMultielementColumnValue("MRealRegion", model, out empty) || model.MRealPostalNo != null)
					{
						str += "<br />";
					}
					str += model.MRealCountryID;
					stringBuilder.Append($"<div class=\"field\"><label>{title}</label><span>{str}</span></div>");
					break;
				}
			}
			return stringBuilder.ToString();
		}

		private static bool GetMultielementColumnValue(string columnName, BDEmployeesModel model, out string outValue)
		{
			outValue = string.Empty;
			if (model.MultiLanguage != null)
			{
				outValue = model.MultiLanguage.FirstOrDefault((MultiLanguageFieldList w) => w.MFieldName.EqualsIgnoreCase(columnName)).MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == ContextHelper.MContext.MLCID).MValue;
			}
			if (!string.IsNullOrWhiteSpace(outValue))
			{
				return true;
			}
			return false;
		}

		private static bool isStringEmpty(List<PropertyInfo> plist, BDEmployeesModel model, string key, out string outValue)
		{
			outValue = Convert.ToString(plist.Find((PropertyInfo f) => f.Name == key).GetValue(model));
			if (!string.IsNullOrWhiteSpace(outValue))
			{
				return true;
			}
			return false;
		}

		private static string GetDueDateStr(List<PropertyInfo> plist, BDEmployeesModel model, string title, string key, string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = Convert.ToInt32(plist.Find((PropertyInfo f) => f.Name == key).GetValue(model));
			if (num > 0)
			{
				stringBuilder.Append(GetUnionString(num, value, title));
			}
			return stringBuilder.ToString();
		}

		private static string GetUnionString(int DueDate, string DueCondition, string title)
		{
			string text = DueDate.ToString();
			if (DueCondition == "item0" || DueCondition == "item3")
			{
				switch (DueDate)
				{
				case 1:
					text = "1st";
					break;
				case 2:
					text = "2nd";
					break;
				case 3:
					text = "3rd";
					break;
				default:
					text = DueDate.ToString() + "th";
					break;
				}
			}
			if (!(DueCondition == "item0"))
			{
				if (!(DueCondition == "item1"))
				{
					if (!(DueCondition == "item2"))
					{
						if (DueCondition == "item3")
						{
							text += HtmlLang.Write(LangModule.Contact, "OfTheCurrentMonth", "of the current month");
						}
					}
					else
					{
						text += HtmlLang.Write(LangModule.Contact, "DaysAfterEndBillMonth", "days after the end of the bill month");
					}
				}
				else
				{
					text += HtmlLang.Write(LangModule.Contact, "DaysAfterTheBillDate", " days after the bill date");
				}
			}
			else
			{
				text += HtmlLang.Write(LangModule.Contact, "OfTheFollowingMonth", "of the following month");
			}
			return $"<div class=\"field\"><label>{title}</label><span>{text}</span></div>";
		}

		private static string GetTeleInfoStr(string title, string value)
		{
			string text = string.Empty;
			string[] array = value.Split('-');
			if (array.Length > 2 && !string.IsNullOrWhiteSpace(array[2]))
			{
				text += (string.IsNullOrWhiteSpace(array[0]) ? "" : array[0]);
				text += (string.IsNullOrWhiteSpace(array[1]) ? "" : (string.IsNullOrWhiteSpace(text) ? array[1] : ("-" + array[1])));
				text += (string.IsNullOrWhiteSpace(array[2]) ? "" : (string.IsNullOrWhiteSpace(text) ? array[2] : ("-" + array[2])));
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				text = $"<div class=\"field\"><label>{title}</label><span>{text}</span></div>";
			}
			return text;
		}
	}
}
