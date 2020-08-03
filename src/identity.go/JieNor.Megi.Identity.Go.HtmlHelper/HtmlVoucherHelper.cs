using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.HtmlHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public class HtmlVoucherHelper
	{
		public string APPROVED = HtmlLang.GetText(LangModule.Common, "Approved", "Approved");

		public string SAVED = HtmlLang.GetText(LangModule.Common, "Saved", "Saved");

		public string INVALIDCLASS = "v-c-i-l";

		public string VOUCHER_HEADER_ROW_HTML = "\r\n                <tr class='vl-v-h'>\r\n                    <td class='m-v-s'>&nbsp;</td>\r\n                    <td class='m-v-c' colspan='6'>\r\n                        <div class='m-v-c-d'>\r\n                            <span class='m-v-d-t'>" + HtmlLang.GetText(LangKey.Date) + ":</span>\r\n                            <span class='v-v-d'>{0}</span>\r\n                            <span class='v-v-n-t'>" + HtmlLang.GetText(LangModule.Common, "Number", "Number") + ":</span>\r\n                            <span class='v-v-n'>{1}</span>\r\n                            <span class='v-v-s-t'>" + HtmlLang.GetText(LangModule.Common, "Status", "Status") + ":</span>\r\n                            <span class='v-v-s'>{2}</span>\r\n                            <span class='v-v-t-t' style='display:{3};'>" + HtmlLang.GetText(LangModule.Common, "TransferType", "计提类型") + ":</span>\r\n                            <span class='v-v-t'>{4}</span>\r\n                        </div>\r\n                    </td>\r\n                </tr>";

		public string VOUCHER_BODY_ROW_HTMLOW_BODY_HTML = "\r\n                <tr class='v-v-e' voucherid='{6}' status='{7}' year='{9}' period='{10}'>\r\n                    <td class='m-v-s' width='1%'>\r\n                        <input type = 'checkbox' class='m-c-it' style='display:{8};'/>\r\n                    </td><td class='m-v-c m-v-r vl-bleft vl-b vl-t' width='20%'>{0}</td><td class='m-v-c m-e-a vl-b vl-t' width='30%'>{1}</td><td class='m-v-c m-e-c vl-b vl-t' width='10%'>{2}</td><td class='m-v-c m-e-ck vl-b vl-t' width='23%'>{3}</td><td class='m-v-c m-e-d vl-b vl-t td-money' width='8%'>{4}</td><td class='m-v-c m-e-cd vl-b vl-t vl-r td-money' width='8%'>{5}</td></tr>";

		public string VOUCHER_END_ROW_HTMLOW_BODY_HTML = "\r\n                <tr class='vl-v-t'  voucherid='{1}' status='{2}' year='{3}' period='{4}'>\r\n                    <td class='m-v-s'>&nbsp;</td>\r\n                    <td class='m-v-c m-v-t-a vl-bleft vl-r vl-b' colspan='6'>\r\n                       " + HtmlLang.GetText(LangKey.Total) + ":\r\n                        <span>{0}</span>\r\n                    </td>\r\n                </tr>";

		public string GenerateVoucherListHtml(List<GLVoucherViewModel> voucherList)
		{
			MContext mContext = ContextHelper.MContext;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < voucherList.Count; i++)
			{
				new StringBuilder();
				GLVoucherViewModel gLVoucherViewModel = voucherList[i];
				stringBuilder.AppendFormat(VOUCHER_HEADER_ROW_HTML, gLVoucherViewModel.MDate.ToString(mContext.MDateFormat), gLVoucherViewModel.MNumber, (gLVoucherViewModel.MStatus == 1) ? APPROVED : SAVED, (gLVoucherViewModel.MTransferTypeID >= 0) ? "" : "none", gLVoucherViewModel.MTransferTypeName).AppendLine();
				int num = (gLVoucherViewModel.MVoucherEntrys.Count + 1) / 2 - 1;
				for (int j = 0; j < gLVoucherViewModel.MVoucherEntrys.Count; j++)
				{
					GLVoucherEntryViewModel gLVoucherEntryViewModel = gLVoucherViewModel.MVoucherEntrys[j];
					stringBuilder.AppendFormat(VOUCHER_BODY_ROW_HTMLOW_BODY_HTML, MText.Encode(gLVoucherEntryViewModel.MExplanation.Replace('\n', ' ').Replace('\r', ' ')), MText.Encode(gLVoucherEntryViewModel.MAccountName), GetCurrencyHtml(mContext, gLVoucherEntryViewModel.MAccountModel.MCurrencyDataModel), GetCheckGroupValueHtml(gLVoucherEntryViewModel.MAccountModel.MCheckGroupValueModel, gLVoucherEntryViewModel.MAccountModel.MCheckGroupModel), (!(gLVoucherEntryViewModel.MDebit == decimal.Zero) || !(gLVoucherEntryViewModel.MCredit == decimal.Zero)) ? ((gLVoucherEntryViewModel.MDebit != decimal.Zero) ? gLVoucherEntryViewModel.MDebit.ToMoneyFormat() : "") : ((gLVoucherEntryViewModel.MDC == 1) ? "0.00" : ""), (!(gLVoucherEntryViewModel.MDebit == decimal.Zero) || !(gLVoucherEntryViewModel.MCredit == decimal.Zero)) ? ((gLVoucherEntryViewModel.MCredit != decimal.Zero) ? gLVoucherEntryViewModel.MCredit.ToMoneyFormat() : "") : ((gLVoucherEntryViewModel.MDC == 1) ? "" : "0.00"), gLVoucherViewModel.MItemID, gLVoucherViewModel.MStatus, (j == num) ? "" : "none", gLVoucherViewModel.MYear, gLVoucherViewModel.MPeriod);
				}
				stringBuilder.AppendFormat(VOUCHER_END_ROW_HTMLOW_BODY_HTML, gLVoucherViewModel.MDebitTotal.ToMoneyFormat(), gLVoucherViewModel.MItemID, gLVoucherViewModel.MStatus, gLVoucherViewModel.MYear, gLVoucherViewModel.MPeriod);
				if (i != voucherList.Count - 1)
				{
					stringBuilder.Append("<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
				}
			}
			return stringBuilder.ToString();
		}

		public StringBuilder GetCurrencyHtml(MContext ctx, BDCurrrencyDataViewModel currency)
		{
			StringBuilder stringBuilder = new StringBuilder("");
			if (currency != null && !string.IsNullOrWhiteSpace(currency.MCurrencyID) && currency.MCurrencyID != ctx.MBasCurrencyID)
			{
				stringBuilder.AppendFormat("\r\n                        <ul class='vc-currency-ul'>\r\n                            <li >{0}[{1}]</li>\r\n                            <li >1 : {2}[{3}]</li>\r\n                        </ul>", currency.MAmountFor.ToMoneyFormat(), currency.MCurrencyID, Math.Round(currency.MExchangeRate, 6).ToString(), ctx.MBasCurrencyID);
			}
			return stringBuilder;
		}

		public bool IsCheckTypeEnable(int value)
		{
			if (value != 1)
			{
				return value == 2;
			}
			return true;
		}

		public string GetCheckTypeName(CheckTypeEnum checkType)
		{
			switch (checkType)
			{
			case CheckTypeEnum.MContactID:
				return HtmlLang.GetText(LangModule.BD, "Contact", "联系人");
			case CheckTypeEnum.MEmployeeID:
				return HtmlLang.GetText(LangModule.BD, "Employee", "员工");
			case CheckTypeEnum.MMerItemID:
				return HtmlLang.GetText(LangModule.BD, "MerItem", "商品项目");
			case CheckTypeEnum.MExpItemID:
				return HtmlLang.GetText(LangModule.BD, "ExpenseItem", "费用项目");
			case CheckTypeEnum.MPaItemID:
				return HtmlLang.GetText(LangModule.BD, "PayItem", "工资项目");
			default:
				return "";
			}
		}

		public StringBuilder GetCheckGroupValueHtml(GLCheckGroupValueViewModel value, GLCheckGroupViewModel group)
		{
			StringBuilder stringBuilder = new StringBuilder("<ul class='vc-checkgroup-ul'>");
			if (value != null && group != null)
			{
				if (!string.IsNullOrWhiteSpace(value.MContactID) && !string.IsNullOrWhiteSpace(value.MContactName))
				{
					string text = (!IsCheckTypeEnable(group.MContactID)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text + "'>" + GetCheckTypeName(CheckTypeEnum.MContactID) + ":" + MText.Encode(value.MContactName) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MEmployeeID) && !string.IsNullOrWhiteSpace(value.MEmployeeName))
				{
					string text2 = (!IsCheckTypeEnable(group.MEmployeeID)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text2 + "'>" + GetCheckTypeName(CheckTypeEnum.MEmployeeID) + ":" + MText.Encode(value.MEmployeeName) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MMerItemID) && !string.IsNullOrWhiteSpace(value.MMerItemName))
				{
					string text3 = (!IsCheckTypeEnable(group.MMerItemID)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text3 + "'>" + GetCheckTypeName(CheckTypeEnum.MMerItemID) + ":" + MText.Encode(value.MMerItemName) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MExpItemID) && !string.IsNullOrWhiteSpace(value.MExpItemName))
				{
					string text4 = (!IsCheckTypeEnable(group.MExpItemID)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text4 + "'>" + GetCheckTypeName(CheckTypeEnum.MExpItemID) + ":" + MText.Encode(value.MExpItemName) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MPaItemID) && (!string.IsNullOrWhiteSpace(value.MPaItemName) || !string.IsNullOrWhiteSpace(value.MPaItemGroupName)))
				{
					string text5 = (!IsCheckTypeEnable(group.MPaItemID)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text5 + "'>" + GetCheckTypeName(CheckTypeEnum.MPaItemID) + ":" + MText.Encode((!string.IsNullOrWhiteSpace(value.MPaItemName)) ? value.MPaItemName : value.MPaItemGroupName) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MTrackItem1) && !string.IsNullOrWhiteSpace(value.MTrackItem1Name))
				{
					string text6 = (!IsCheckTypeEnable(group.MTrackItem1)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text6 + "'>" + MText.Encode(value.MTrackItem1GroupName + ":" + value.MTrackItem1Name) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MTrackItem2) && !string.IsNullOrWhiteSpace(value.MTrackItem2Name))
				{
					string text7 = (!IsCheckTypeEnable(group.MTrackItem2)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text7 + "'>" + MText.Encode(value.MTrackItem2GroupName + ":" + value.MTrackItem2Name) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MTrackItem3) && !string.IsNullOrWhiteSpace(value.MTrackItem3Name))
				{
					string text8 = (!IsCheckTypeEnable(group.MTrackItem3)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text8 + "'>" + MText.Encode(value.MTrackItem3GroupName + ":" + value.MTrackItem3Name) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MTrackItem4) && !string.IsNullOrWhiteSpace(value.MTrackItem4Name))
				{
					string text9 = (!IsCheckTypeEnable(group.MTrackItem4)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text9 + "'>" + MText.Encode(value.MTrackItem4GroupName + ":" + value.MTrackItem4Name) + "</li>");
				}
				if (!string.IsNullOrWhiteSpace(value.MTrackItem5) && !string.IsNullOrWhiteSpace(value.MTrackItem5Name))
				{
					string text10 = (!IsCheckTypeEnable(group.MTrackItem5)) ? INVALIDCLASS : " ";
					stringBuilder.Append("<li  class='vc-checkgroup-li " + text10 + "'>" + MText.Encode(value.MTrackItem5GroupName + ":" + value.MTrackItem5Name) + "</li>");
				}
				stringBuilder.Append("</ul>");
				return stringBuilder;
			}
			return new StringBuilder();
		}
	}
}
