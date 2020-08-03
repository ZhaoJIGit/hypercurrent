using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.GL;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.GL
{
	public class GLInitBalanceService : ServiceT<GLInitBalanceModel>, IGLInitBalance
	{
		private readonly IGLInitBalanceBusiness biz = new GLInitBalanceBusiness();

		public MActionResult<GLInitBalanceModel> GetBankInitBalance(string accountId, string bankName, string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.GetBankInitBalance, accountId, bankName, accessToken);
		}

		public MActionResult<OperationResult> ClearInitBalance(string initBalanceId = null, string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.ClearInitBalance, initBalanceId, accessToken);
		}

		public MActionResult<List<GLInitBalanceModel>> GetInitBalanceList(GLInitBalanceListFilterModel filter, string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.GetInitBalanceList, filter, accessToken);
		}

		public MActionResult<OperationResult> Save(List<GLInitBalanceModel> initBalanceList, string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.SaveInitBalance, initBalanceList, accessToken);
		}

		public MActionResult<List<ImportTemplateModel>> GetImportTemplateModel(string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.GetImportTemplateModel, accessToken);
		}

		public MActionResult<List<GLInitBalanceModel>> GetCompleteInitBalanceList(GLInitBalanceListFilterModel filter, string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.GetCompleteInitBalanceList, filter, accessToken);
		}

		public MActionResult<OperationResult> ImportInitBalanceList(List<GLInitBalanceModel> list, string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.ImportInitBalanceList, list, accessToken);
		}

		public MActionResult<OperationResult> CheckAutoCreateBillHadVerifiyRecord(string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.CheckAutoCreateBillHadVerifiyRecord, accessToken);
		}

		public MActionResult<OperationResult> ValidateData(List<GLInitBalanceModel> list, string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.RunFunc(iGLInitBalanceBusiness.ValidateData, list, accessToken);
		}

		public MActionResult<List<GLInitBalanceModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null)
		{
			IGLInitBalanceBusiness iGLInitBalanceBusiness = biz;
			return base.GetModelList(iGLInitBalanceBusiness.GetModelList, filter, includeDelete, accessToken);
		}
	}
}
