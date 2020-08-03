using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLSettlementBusiness : IDataContract<GLSettlementModel>
	{
		GLSettlementModel GetSettlementModel(MContext ctx, GLSettlementModel model);

		OperationResult Settle(MContext ctx, GLSettlementModel model);

		OperationResult PreSettle(MContext ctx, DateTime date, bool isCalculate = false);

		bool IsPeriodValid(MContext ctx, DateTime date);

		string GetLastFinishedPeriod(MContext ctx);

		List<DateTime> GetSettledPeriodFromBeginDate(MContext ctx, bool includeCurrentPeriod = false);

		List<string> GetSettledPeriod(MContext ctx);

		List<DateTime> GetFullPeriod(MContext ctx);
	}
}
