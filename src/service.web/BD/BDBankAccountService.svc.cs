using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDBankAccountService : ServiceT<BDAccountModel>, IBDBankAccount
	{
		private readonly IBDBankAccountBusiness biz = new BDBankAccountBusiness();

		public MActionResult<BDBankChartModel> GetBankTotalChartModel(string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetBankTotalChartModel, accessToken);
		}

		public MActionResult<List<BDBankAccountEditModel>> GetBDBankAccountEditList(DateTime startDate, DateTime endDate, string[] accountIds, bool useBase = false, bool needSum = false, bool needChart = false, string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetBDBankAccountEditList, startDate, endDate, accountIds, useBase, needSum, needChart, accessToken);
		}

		public MActionResult<List<BDBankAccountEditModel>> GetBankAccountList(string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetBankAccountList, accessToken);
		}

		public MActionResult<OperationResult> DeleteBankbill(string[] mids, string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.DeleteBankbill, mids, accessToken);
		}

		public MActionResult<BDBankAccountEditModel> GetBDBankAccountEditModel(string pkID, string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetBDBankAccountEditModel, pkID, accessToken);
		}

		public MActionResult<List<BDBankAccountEditModel>> GetBDBankDashboardData(DateTime? startDate, DateTime? endDate, string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetBDBankDashboardData, startDate, endDate, accessToken);
		}

		public MActionResult<OperationResult> UpdateBankAccount(BDBankAccountEditModel model, string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.UpdateBankAccount, model, accessToken);
		}

		public MActionResult<List<BDBankAccountListModel>> GetBDBankAccountList(string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetBDBankAccountList, accessToken);
		}

		public MActionResult<DataGridJson<BDBankInitBalanceModel>> GetInitBalanceListByPage(SqlWhere filter, string accessToken)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetInitBalanceListByPage, filter, accessToken);
		}

		public MActionResult<BDBankInitBalanceModel> GetBDBankInitBalance(string bankId, string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetBDBankInitBalance, bankId, accessToken);
		}

		public MActionResult<OperationResult> UpdateBankInitBalance(BDBankInitBalanceModel model, string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.UpdateBankInitBalance, model, accessToken);
		}

		public MActionResult<List<NameValueModel>> GetSimpleBankAccountList(string orgIds, string accessToken = null)
		{
			IBDBankAccountBusiness iBDBankAccountBusiness = biz;
			return base.RunFunc(iBDBankAccountBusiness.GetSimpleBankAccountList, orgIds, accessToken);
		}
	}
}
