using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Identity.Go.AutoManager;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlREGTaxRate
	{
		public static MvcHtmlString SelectOptions()
		{
			MContext mContext = ContextHelper.MContext;
			new REGTaxRateModel();
			List<REGTaxRateModel> taxRateList = REGTaxRateManager.GetTaxRateList();
			StringBuilder stringBuilder = new StringBuilder();
			if (taxRateList != null)
			{
				foreach (REGTaxRateModel item in taxRateList)
				{
					stringBuilder.Append(string.Format("<option value=\"{0}\">{1}</option>", item.MItemID, MText.Encode(item.MName) + "(" + item.MTaxRate.To2Decimal() + "%)"));
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString OrgTaxRate()
		{
			MContext mContext = ContextHelper.MContext;
			new REGTaxRateModel();
			List<REGTaxRateModel> taxRateList = REGTaxRateManager.GetTaxRateList();
			StringBuilder stringBuilder = new StringBuilder();
			if (taxRateList != null && taxRateList.Count > 0)
			{
				foreach (REGTaxRateModel item in taxRateList)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(string.Format("MItemID:\"{0}\",MName:\"{1}({2}%)\",MTaxRate:\"{3}\",MEffectiveTaxRate:\"{4}\"", item.MItemID, item.MName, item.MTaxRate.ToString("#0.00"), item.MTaxRate / 100m, item.MEffectiveTaxRate / 100m));
					stringBuilder.Append("},");
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return new MvcHtmlString($"<input type=\"hidden\" id='hidOrgTaxRateList' value=\"[{stringBuilder.ToString()}]\" />");
		}

		public static List<object> GetOrgTaxRateList()
		{
			MContext mContext = ContextHelper.MContext;
			new REGTaxRateModel();
			List<REGTaxRateModel> taxRateList = REGTaxRateManager.GetTaxRateList();
			new StringBuilder();
			List<object> list = new List<object>();
			if (taxRateList != null && taxRateList.Count > 0)
			{
				{
					foreach (REGTaxRateModel item2 in taxRateList)
					{
						var item = new
						{
							MItemID = item2.MItemID,
							MName = string.Format("{0}({1}%)", item2.MName, item2.MTaxRate.ToString("#0.00")),
							MTaxRate = item2.MTaxRate / 100m,
							MEffectiveTaxRate = item2.MEffectiveTaxRate / 100m
						};
						list.Add(item);
					}
					return list;
				}
			}
			return list;
		}
	}
}
