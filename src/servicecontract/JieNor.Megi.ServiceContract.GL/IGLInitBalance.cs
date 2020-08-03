using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.GL
{
	[ServiceContract]
	public interface IGLInitBalance
	{
		[OperationContract]
		MActionResult<GLInitBalanceModel> GetBankInitBalance(string accountId, string bankName, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ClearInitBalance(string initBalanceId = null, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLInitBalanceModel>> GetInitBalanceList(GLInitBalanceListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Save(List<GLInitBalanceModel> initBalanceList, string accessToken = null);

		[OperationContract]
		MActionResult<List<ImportTemplateModel>> GetImportTemplateModel(string accessToken = null);

		[OperationContract]
		MActionResult<List<GLInitBalanceModel>> GetCompleteInitBalanceList(GLInitBalanceListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportInitBalanceList(List<GLInitBalanceModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> CheckAutoCreateBillHadVerifiyRecord(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ValidateData(List<GLInitBalanceModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLInitBalanceModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null);
	}
}
