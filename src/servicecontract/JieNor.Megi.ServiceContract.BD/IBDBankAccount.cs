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
	public interface IBDBankAccount
	{
		[OperationContract]
		MActionResult<BDBankChartModel> GetBankTotalChartModel(string accessToken = null);

		[OperationContract]
		MActionResult<List<BDBankAccountEditModel>> GetBDBankAccountEditList(DateTime startDate, DateTime endDate, string[] accountIds, bool useBase = false, bool needSum = false, bool needChart = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDBankAccountEditModel>> GetBankAccountList(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteBankbill(string[] mids, string accessToken = null);

		[OperationContract]
		MActionResult<BDBankAccountEditModel> GetBDBankAccountEditModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDBankAccountEditModel>> GetBDBankDashboardData(DateTime? startDate, DateTime? endDate, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateBankAccount(BDBankAccountEditModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDBankAccountListModel>> GetBDBankAccountList(string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BDBankInitBalanceModel>> GetInitBalanceListByPage(SqlWhere filter, string accessToken = null);

		[OperationContract]
		MActionResult<BDBankInitBalanceModel> GetBDBankInitBalance(string bankId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateBankInitBalance(BDBankInitBalanceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<NameValueModel>> GetSimpleBankAccountList(string orgIds, string accessToken = null);
	}
}
