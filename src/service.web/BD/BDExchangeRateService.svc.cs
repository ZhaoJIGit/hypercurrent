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
	public class BDExchangeRateService : ServiceT<BDExchangeRateModel>, IBDExchangeRate
	{
		private readonly IBDExchangeRateBusiness biz = new BDExchangeRateBusiness();

		public MActionResult<List<BDExchangeRateModel>> GetExchangeRateList(BDExchangeRateModel model, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.RunFunc(iBDExchangeRateBusiness.GetExchangeRateList, model, accessToken);
		}

		public MActionResult<DataGridJson<BDExchangeRateViewModel>> GetExchangeRateViewList(BDExchangeRateFilterModel filter, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.RunFunc(iBDExchangeRateBusiness.GetExchangeRateViewList, filter, accessToken);
		}

		public MActionResult<OperationResult> InsertExchangeRate(BDExchangeRateModel model, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.InsertOrUpdate(iBDExchangeRateBusiness.InsertOrUpdate, model, null, accessToken);
		}

		public MActionResult<OperationResult> UpdateExchangeRate(BDExchangeRateModel model, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.RunFunc(iBDExchangeRateBusiness.UpdateExchangeRate, model, accessToken);
		}

		public MActionResult<OperationResult> RemoveExchangeRate(BDExchangeRateModel model, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.RunFunc(iBDExchangeRateBusiness.RemoveExchangeRate, model, accessToken);
		}

		public MActionResult<decimal> GetExchangeRate(string from, DateTime date, string to = null, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.RunFunc(iBDExchangeRateBusiness.GetExchangeRate, from, date, to, accessToken);
		}

		public MActionResult<OperationResult> UpdateExchangeRateList(List<BDExchangeRateModel> list, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.RunFunc(iBDExchangeRateBusiness.UpdateExchangeRateList, list, accessToken);
		}

		public MActionResult<List<BDExchangeRateModel>> GetMonthlyExchangeRateList(DateTime date, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.RunFunc(iBDExchangeRateBusiness.GetMonthlyExchangeRateList, date, accessToken);
		}

		public MActionResult<BDExchangeRateModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.GetDataModel(iBDExchangeRateBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}

		public MActionResult<bool> ExistsByFilter(SqlWhere filter, string accessToken = null)
		{
			IBDExchangeRateBusiness iBDExchangeRateBusiness = biz;
			return base.ExistsByFilter(iBDExchangeRateBusiness.ExistsByFilter, filter, accessToken);
		}
	}
}
