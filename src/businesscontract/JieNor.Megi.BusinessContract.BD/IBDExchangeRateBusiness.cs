using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDExchangeRateBusiness : IDataContract<BDExchangeRateModel>
	{
		List<BDExchangeRateModel> GetExchangeRateList(MContext ctx, BDExchangeRateModel model);

		DataGridJson<BDExchangeRateViewModel> GetExchangeRateViewList(MContext context, BDExchangeRateFilterModel filter);

		OperationResult InsertExchangeRate(MContext ctx, BDExchangeRateModel model);

		OperationResult UpdateExchangeRate(MContext ctx, BDExchangeRateModel model);

		OperationResult RemoveExchangeRate(MContext ctx, BDExchangeRateModel model);

		OperationResult UpdateExchangeRateList(MContext ctx, List<BDExchangeRateModel> list);

		List<BDExchangeRateModel> GetMonthlyExchangeRateList(MContext ctx, DateTime date);

		decimal GetExchangeRate(MContext ctx, string from, DateTime date, string to = null);
	}
}
