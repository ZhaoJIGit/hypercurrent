using JieNor.Megi.BusinessContract.REG;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.ServiceContract.REG;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.REG
{
	public class REGCurrencyService : ServiceT<REGCurrencyModel>, IREGCurrency
	{
		private readonly IREGCurrencyBusiness biz = new REGCurrencyBusiness();

		public MActionResult<List<REGCurrencyModel>> GetCurrencyList(string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.GetCurrencyList, accessToken);
		}

		public MActionResult<List<REGCurrencyViewModel>> GetCurrencyViewList(DateTime? endTime, bool isIncludeBase = false, string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.GetCurrencyViewList, endTime, isIncludeBase, accessToken);
		}

		public MActionResult<OperationResult> InsertCurrency(REGCurrencyModel model, string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.InsertCurrency, model, accessToken);
		}

		public MActionResult<OperationResult> RemoveModel(REGCurrencyModel model, string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.RemoveCurrency, model, accessToken);
		}

		public MActionResult<BASCurrencyViewModel> GetBaseCurrency(string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.GetBaseCurrency, accessToken);
		}

		public MActionResult<OperationResult> RemoveCurrency(REGCurrencyModel model, string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.RemoveCurrency, model, accessToken);
		}

		public MActionResult<List<GlobalCurrencyModel>> GetOrgCurrencyModel(string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.GetOrgCurrencyModel, accessToken);
		}

		public MActionResult<List<REGCurrencyViewModel>> GetAllCurrencyList(bool isIncludeBase = false, bool ignoreLocale = false, string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.GetAllCurrencyList, isIncludeBase, ignoreLocale, accessToken);
		}

		public MActionResult<List<REGCurrencyViewModel>> GetBillCurrencyViewList(DateTime? endTime, bool isIncludeBase = false, string accessToken = null)
		{
			IREGCurrencyBusiness iREGCurrencyBusiness = biz;
			return base.RunFunc(iREGCurrencyBusiness.GetBillCurrencyViewList, endTime, isIncludeBase, accessToken);
		}
	}
}
