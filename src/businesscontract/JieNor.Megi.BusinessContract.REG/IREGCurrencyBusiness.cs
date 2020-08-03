using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.REG
{
	public interface IREGCurrencyBusiness : IDataContract<REGCurrencyModel>
	{
		List<REGCurrencyModel> GetCurrencyList(MContext context);

		List<REGCurrencyViewModel> GetCurrencyViewList(MContext context, DateTime? endTime, bool inIncludeBase = false);

		OperationResult InsertCurrency(MContext ctx, REGCurrencyModel model);

		OperationResult RemoveCurrency(MContext ctx, REGCurrencyModel model);

		REGCurrencyModel GetSingleCurrency(MContext ctx, REGCurrencyModel model);

		BASCurrencyViewModel GetBaseCurrency(MContext context);

		List<GlobalCurrencyModel> GetOrgCurrencyModel(MContext ctx);

		List<REGCurrencyViewModel> GetAllCurrencyList(MContext ctx, bool isIncludeBase = false, bool ignoreLocale = false);

		List<REGCurrencyViewModel> GetBillCurrencyViewList(MContext ctx, DateTime? endTime, bool isIncludeBase = false);
	}
}
