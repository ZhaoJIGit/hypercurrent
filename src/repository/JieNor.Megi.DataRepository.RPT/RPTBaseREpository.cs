using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTBaseREpository
	{
		public static string GetDateHeadTitleByMonth(MContext ctx, DateTime period)
		{
			if (ctx.MLCID == "0x0009")
			{
				return period.ToString("MMMMMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("en-US"));
			}
			return $"{period.Month}月";
		}

		public static string GetDateHeadTitleByMonth(MContext ctx, DateTime fromDate, DateTime toDate)
		{
			if (ctx.MLCID == "0x0009")
			{
				return string.Format("{0} to {1}", fromDate.ToString("MMMMMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("en-US")), toDate.ToString("MMMMMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("en-US")));
			}
			return $"{fromDate.Month}月 至 {toDate.Month}月";
		}

		public static string GetDateHeadTitle(MContext ctx, DateTime period)
		{
			if (ctx.MLCID == "0x0009")
			{
				return period.ToString("MMMMMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("en-US"));
			}
			return $"{period.Month}月";
		}

		public static string GetDateHeadTitle(MContext ctx, DateTime fromDate, DateTime toDate)
		{
			if (ctx.MLCID == "0x0009")
			{
				return $"{fromDate.ToString(ctx.MDateFormat)} to {toDate.ToString(ctx.MDateFormat)}";
			}
			return $"{fromDate.ToString(ctx.MDateFormat)} 至 {toDate.ToString(ctx.MDateFormat)}";
		}

		public static string GetCheckGroupValue(MContext ctx, RPTDepreciationBaseModel model)
		{
			string text = "";
			if (!string.IsNullOrWhiteSpace(model.MContactNameDep) || !string.IsNullOrWhiteSpace(model.MContactNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Contact", "联系人") + ":" + (string.IsNullOrWhiteSpace(model.MContactNameDep) ? model.MContactNameExp : model.MContactNameDep) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MEmployeeNameDep) || !string.IsNullOrWhiteSpace(model.MEmployeeNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Employee", "员工") + ":" + (string.IsNullOrWhiteSpace(model.MEmployeeNameDep) ? model.MEmployeeNameExp : model.MEmployeeNameDep) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MMerItemNameDep) || !string.IsNullOrWhiteSpace(model.MMerItemNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "MerItem", "商品项目") + ":" + (string.IsNullOrWhiteSpace(model.MMerItemNameDep) ? model.MMerItemNameExp : model.MMerItemNameDep) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MExpItemNameDep) || !string.IsNullOrWhiteSpace(model.MExpItemNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItem", "费用项目") + ":" + (string.IsNullOrWhiteSpace(model.MExpItemNameDep) ? model.MExpItemNameExp : model.MExpItemNameDep) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MPaItemGroupNameDep) || !string.IsNullOrWhiteSpace(model.MPaItemGroupNameExp))
			{
				text = text + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PaSalaryItem", "工资项目") + ":" + (string.IsNullOrWhiteSpace(model.MPaItemGroupNameDep) ? model.MPaItemGroupNameExp : model.MPaItemGroupNameDep) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem1NameDep) || !string.IsNullOrWhiteSpace(model.MTrackItem1NameExp))
			{
				text = text + (string.IsNullOrWhiteSpace(model.MTrackItem1NameDep) ? (model.MTrackItem1GroupNameExp + ":" + model.MTrackItem1NameExp) : (model.MTrackItem1GroupNameDep + ":" + model.MTrackItem1NameDep)) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem2NameDep) || !string.IsNullOrWhiteSpace(model.MTrackItem2NameExp))
			{
				text = text + (string.IsNullOrWhiteSpace(model.MTrackItem2NameDep) ? (model.MTrackItem2GroupNameExp + ":" + model.MTrackItem2NameExp) : (model.MTrackItem2GroupNameDep + ":" + model.MTrackItem2NameDep)) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem3NameDep) || !string.IsNullOrWhiteSpace(model.MTrackItem3NameExp))
			{
				text = text + (string.IsNullOrWhiteSpace(model.MTrackItem3NameDep) ? (model.MTrackItem3GroupNameExp + ":" + model.MTrackItem3NameExp) : (model.MTrackItem3GroupNameDep + ":" + model.MTrackItem3NameDep)) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem4NameDep) || !string.IsNullOrWhiteSpace(model.MTrackItem4NameExp))
			{
				text = text + (string.IsNullOrWhiteSpace(model.MTrackItem4NameDep) ? (model.MTrackItem4GroupNameExp + ":" + model.MTrackItem4NameExp) : (model.MTrackItem4GroupNameDep + ":" + model.MTrackItem4NameDep)) + ";";
			}
			if (!string.IsNullOrWhiteSpace(model.MTrackItem5NameDep) || !string.IsNullOrWhiteSpace(model.MTrackItem5NameExp))
			{
				text = text + (string.IsNullOrWhiteSpace(model.MTrackItem5NameDep) ? (model.MTrackItem5GroupNameExp + ":" + model.MTrackItem5NameExp) : (model.MTrackItem5GroupNameDep + ":" + model.MTrackItem5NameDep)) + ";";
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				text = text.TrimEnd(';') + ".";
			}
			return text;
		}

		public static string GetFaCheckGroupSelect(List<NameValueModel> checkTypeValueList)
		{
			string result = "";
			List<string> list = new List<string>();
			if (checkTypeValueList.Count > 0)
			{
				foreach (NameValueModel checkTypeValue in checkTypeValueList)
				{
					switch (checkTypeValue.MName)
					{
					case "0":
						list.Add(" ( t100.MContactID=@MContactID OR t101.MContactID=@MContactID ) ");
						break;
					}
				}
			}
			return result;
		}
	}
}
