using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDBankAccountBusiness
	{
		BDBankChartModel GetBankTotalChartModel(MContext ctx);

		OperationResult DeleteBankbill(MContext ctx, string[] mids);

		List<BDBankAccountEditModel> GetBDBankAccountEditList(MContext ctx, DateTime startDate, DateTime endDate, string[] accountIds, bool useBase = false, bool needSum = false, bool needChart = false);

		List<BDBankAccountEditModel> GetBankAccountList(MContext ctx);

		BDBankAccountEditModel GetBDBankAccountEditModel(MContext ctx, string pkID);

		List<BDBankAccountEditModel> GetBDBankDashboardData(MContext ctx, DateTime? startDate, DateTime? endDate);

		OperationResult UpdateBankAccount(MContext ctx, BDBankAccountEditModel model);

		List<BDBankAccountListModel> GetBDBankAccountList(MContext ctx);

		DataGridJson<BDBankInitBalanceModel> GetInitBalanceListByPage(MContext ctx, SqlWhere filter);

		BDBankInitBalanceModel GetBDBankInitBalance(MContext ctx, string bankId);

		OperationResult UpdateBankInitBalance(MContext ctx, BDBankInitBalanceModel model);

		List<NameValueModel> GetSimpleBankAccountList(MContext ctx, string orgIds);
	}
}
