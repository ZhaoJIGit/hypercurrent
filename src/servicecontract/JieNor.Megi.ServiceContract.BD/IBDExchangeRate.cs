using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDExchangeRate
	{
		[OperationContract]
		MActionResult<List<BDExchangeRateModel>> GetExchangeRateList(BDExchangeRateModel model, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BDExchangeRateViewModel>> GetExchangeRateViewList(BDExchangeRateFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertExchangeRate(BDExchangeRateModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateExchangeRate(BDExchangeRateModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> RemoveExchangeRate(BDExchangeRateModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateExchangeRateList(List<BDExchangeRateModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDExchangeRateModel>> GetMonthlyExchangeRateList(DateTime date, string accessToken = null);

		[OperationContract]
		MActionResult<decimal> GetExchangeRate(string from, DateTime date, string to = null, string accessToken = null);

		[OperationContract]
		MActionResult<BDExchangeRateModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<bool> ExistsByFilter(SqlWhere filter, string accessToken = null);
	}
}
