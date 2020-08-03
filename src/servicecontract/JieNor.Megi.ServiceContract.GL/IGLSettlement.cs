using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.GL
{
	[ServiceContract]
	public interface IGLSettlement
	{
		[OperationContract]
		MActionResult<GLSettlementModel> GetSettlementModel(GLSettlementModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Settle(GLSettlementModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> PreSettle(DateTime date, bool isCalculate = false, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetLastFinishedPeriod(string accessToken = null);

		[OperationContract]
		MActionResult<bool> IsPeriodValid(DateTime date, string accessToken = null);

		[OperationContract]
		MActionResult<List<DateTime>> GetSettledPeriodFromBeginDate(bool includeCurrentPeriod = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<string>> GetSettledPeriod(string accessToken = null);

		[OperationContract]
		MActionResult<List<DateTime>> GetFullPeriod(string accessToken = null);
	}
}
