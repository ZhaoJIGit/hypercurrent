using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.Identity.Go.AutoManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlBDCurrency
	{
		public static MvcHtmlString SelectOptions()
		{
			new REGCurrencyModel();
			List<REGCurrencyViewModel> currencyViewList = BDCurrencyManager.GetCurrencyViewList(null);
			StringBuilder stringBuilder = new StringBuilder();
			if (currencyViewList != null)
			{
				foreach (REGCurrencyViewModel item in currencyViewList)
				{
					stringBuilder.Append(string.Format("<option value=\"{0}\">{0} {1}</option>", item.MCurrencyID, item.MName));
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static string GetDataOptionsString(DateTime? endDate, bool hasRate = false)
		{
			List<REGCurrencyViewModel> billCurrencyViewList = BDCurrencyManager.GetBillCurrencyViewList(endDate);
			BASCurrencyViewModel baseCurrency = BDCurrencyManager.GetBaseCurrency();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("valueField: 'CurrencyID',textField: 'CurrencyName',data:[");
			string text = (baseCurrency == null) ? string.Empty : baseCurrency.MCurrencyID;
			stringBuilder.Append("{").Append(string.Format("CurrencyID:'{0}',CurrencyName:'{0} {1}',IsLocalCy:{2},MLToORate:'{3}',MOToLRate:'{4}',MLCurrencyID:'{5}',MExchangeRateID:'{6}',MCyItemID:'{7}'", text, (baseCurrency == null) ? string.Empty : baseCurrency.MLocalName, "true", 1, 1, text, "", baseCurrency.MItemID)).Append("},");
			if (billCurrencyViewList != null && billCurrencyViewList.Count > 0)
			{
				foreach (REGCurrencyViewModel item in billCurrencyViewList)
				{
					string mUserRate = item.MUserRate;
					decimal d = default(decimal);
					if (item.MUserRate != null && item.MRate.Length > 0 && decimal.TryParse(item.MRate, out d) && d > decimal.Parse("0.000001") && item.MRate != null && item.MRate.Length > 0 && decimal.TryParse(item.MRate, out d) && d > decimal.Parse("0.000001"))
					{
						stringBuilder.Append("{");
						stringBuilder.Append(string.Format("CurrencyID:'{0}',CurrencyName:'{0} {1}',IsLocalCy:{2},MLToORate:'{3}',MOToLRate:'{4}',MLCurrencyID:'{5}',MExchangeRateID:'{6}',MCyItemID:'{7}'", item.MCurrencyID, item.MName, "false", item.MUserRate, item.MRate, baseCurrency.MCurrencyID, item.MExchangeRateID, item.MItemID));
						stringBuilder.Append("},");
					}
					else if (!hasRate)
					{
						stringBuilder.Append("{");
						stringBuilder.Append(string.Format("CurrencyID:'{0}',CurrencyName:'{0} {1}',IsLocalCy:{2},MLToORate:'{3}',MOToLRate:'{4}',MLCurrencyID:'{5}',MExchangeRateID:'{6}',MCyItemID:'{7}'", item.MCurrencyID, item.MName, "false", item.MUserRate, mUserRate, baseCurrency.MCurrencyID, item.MExchangeRateID, item.MItemID));
						stringBuilder.Append("},");
					}
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append("]");
			stringBuilder.Append($",defaultValue:'{text}'");
			return stringBuilder.ToString();
		}

		public static MvcHtmlString DataOptions(DateTime? endDate = default(DateTime?), bool hasRate = false)
		{
			return new MvcHtmlString(GetDataOptionsString(endDate, hasRate));
		}

		public static MvcHtmlString GetBaseCurrency()
		{
			BASCurrencyViewModel baseCurrency = BDCurrencyManager.GetBaseCurrency();
			return new MvcHtmlString((baseCurrency == null) ? string.Empty : baseCurrency.MCurrencyID);
		}
	}
}
