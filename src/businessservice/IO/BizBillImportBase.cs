using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.BusinessService.IO
{
	public class BizBillImportBase : ImportBase
	{
		private List<REGTaxRateModel> TaxRateList
		{
			get;
			set;
		}

		protected override void SetCacheData(MContext ctx)
		{
			base.SetCacheData(ctx);
			TaxRateList = new REGTaxRateBusiness().GetTaxRateList(ctx, false);
		}

		protected virtual void SetT2Info<T1, T2>(T1 model1, T2 entryModel)
		{
			string modelValue = ModelHelper.GetModelValue(model1, "MCyID");
			string modelValue2 = ModelHelper.GetModelValue(model1, "MTaxID");
			PropertyInfo[] properties = typeof(T2).GetProperties();
			decimal num = ModelHelper.GetModelValueD(entryModel, "MQty") * ModelHelper.GetModelValueD(entryModel, "MPrice");
			decimal modelValueD = ModelHelper.GetModelValueD(entryModel, "MDiscount");
			if (modelValueD > decimal.Zero)
			{
				num *= decimal.One - modelValueD / 100m;
			}
			num = Math.Abs(num);
			REGTaxRateModel rEGTaxRateModel = null;
			if (modelValue2 == "No_Tax")
			{
				ModelHelper.SetModelValue(entryModel, "MTaxID", null, properties);
			}
			else
			{
				string entryTaxId = ModelHelper.GetModelValue(entryModel, "MTaxID");
				rEGTaxRateModel = TaxRateList.SingleOrDefault((REGTaxRateModel f) => f.MItemID == entryTaxId);
			}
			decimal d = default(decimal);
			if (rEGTaxRateModel != null)
			{
				d = rEGTaxRateModel.MEffectiveTaxRateDecimal;
			}
			decimal d2 = default(decimal);
			switch (modelValue2)
			{
			case "Tax_Exclusive":
				d2 = Math.Round(num * d, 2);
				break;
			case "Tax_Inclusive":
				d2 = Math.Round(num - num / (decimal.One + d), 2);
				break;
			case "No_Tax":
				d2 = default(decimal);
				break;
			}
			ModelHelper.SetModelValue(entryModel, "MTaxAmtFor", Math.Round(d2, 2), properties);
		}

		protected virtual void SetExpenseEntryInfo(IVExpenseModel model, IVExpenseEntryModel entryModel)
		{
			model.MExchangeRate = GetExchangeRate(model.MCyID, base.BasCurrency, base.ForeignCurrencyList, model.MBizDate);
			entryModel.MTaxAmountFor = Math.Round(Math.Abs(entryModel.MQty * entryModel.MPrice), 2);
			entryModel.MTaxAmount = Math.Round(entryModel.MTaxAmountFor * model.MExchangeRate, 2);
			entryModel.MTaxAmt = Math.Round(entryModel.MTaxAmtFor * model.MExchangeRate, 2);
			entryModel.MAmountFor = entryModel.MTaxAmountFor;
			entryModel.MAmount = entryModel.MTaxAmount;
			entryModel.MApproveAmt = entryModel.MTaxAmount;
			entryModel.MApproveAmtFor = entryModel.MTaxAmountFor;
		}

		protected virtual void SetT1Info<T1, T2>(T1 model)
		{
			PropertyInfo[] properties = typeof(T1).GetProperties();
			DateTime value = Convert.ToDateTime(ModelHelper.GetModelValue(model, "MBizDate"));
			decimal exchangeRate = GetExchangeRate(ModelHelper.GetModelValue(model, "MCyID"), base.BasCurrency, base.ForeignCurrencyList, value);
			ModelHelper.SetModelValue(model, "MExchangeRate", exchangeRate, properties);
		}

		private decimal GetExchangeRate(string currencyId, BASCurrencyViewModel basCurrency, List<REGCurrencyViewModel> forCurrencyList, DateTime? bizDate = default(DateTime?))
		{
			decimal result = decimal.One;
			if (basCurrency.MCurrencyID != currencyId)
			{
				REGCurrencyViewModel rEGCurrencyViewModel = null;
				IOrderedEnumerable<REGCurrencyViewModel> source = from f in forCurrencyList
				where f.MCurrencyID == currencyId && f.MRateDate <= bizDate.Value
				orderby f.MRateDate descending
				select f;
				if (source.Any())
				{
					rEGCurrencyViewModel = source.First();
				}
				decimal d = default(decimal);
				if (rEGCurrencyViewModel != null && !string.IsNullOrWhiteSpace(rEGCurrencyViewModel.MRate) && decimal.TryParse(rEGCurrencyViewModel.MRate, out d) && d > decimal.Zero)
				{
					result = Convert.ToDecimal(rEGCurrencyViewModel.MRate);
				}
			}
			return result;
		}
	}
}
