using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.GL;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.GL
{
	public class GLSettlementService : ServiceT<GLSettlementModel>, IGLSettlement
	{
		private readonly IGLSettlementBusiness biz = new GLSettlementBusiness();

		public MActionResult<string> GetLastFinishedPeriod(string accessToken = null)
		{
			IGLSettlementBusiness iGLSettlementBusiness = biz;
			return base.RunFunc(iGLSettlementBusiness.GetLastFinishedPeriod, accessToken);
		}

		public MActionResult<GLSettlementModel> GetSettlementModel(GLSettlementModel model, string accessToken = null)
		{
			IGLSettlementBusiness iGLSettlementBusiness = biz;
			return base.RunFunc(iGLSettlementBusiness.GetSettlementModel, model, accessToken);
		}

		public MActionResult<OperationResult> Settle(GLSettlementModel model, string accessToken = null)
		{
			IGLSettlementBusiness iGLSettlementBusiness = biz;
			return base.RunFunc(iGLSettlementBusiness.Settle, model, accessToken);
		}

		public MActionResult<OperationResult> PreSettle(DateTime date, bool isCalculate = false, string accessToken = null)
		{
			IGLSettlementBusiness iGLSettlementBusiness = biz;
			return base.RunFunc(iGLSettlementBusiness.PreSettle, date, isCalculate, accessToken);
		}

		public MActionResult<List<DateTime>> GetSettledPeriodFromBeginDate(bool includeCurrentPeriod = false, string accessToken = null)
		{
			IGLSettlementBusiness iGLSettlementBusiness = biz;
			return base.RunFunc(iGLSettlementBusiness.GetSettledPeriodFromBeginDate, includeCurrentPeriod, accessToken);
		}

		public MActionResult<bool> IsPeriodValid(DateTime date, string accessToken = null)
		{
			IGLSettlementBusiness iGLSettlementBusiness = biz;
			return base.RunFunc(iGLSettlementBusiness.IsPeriodValid, date, accessToken);
		}

		public MActionResult<List<string>> GetSettledPeriod(string accessToken = null)
		{
			IGLSettlementBusiness iGLSettlementBusiness = biz;
			return base.RunFunc(iGLSettlementBusiness.GetSettledPeriod, accessToken);
		}

		public MActionResult<List<DateTime>> GetFullPeriod(string accessToken = null)
		{
			IGLSettlementBusiness iGLSettlementBusiness = biz;
			return base.RunFunc(iGLSettlementBusiness.GetFullPeriod, accessToken);
		}
	}
}
