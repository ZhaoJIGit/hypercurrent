using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.REG;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class CurrencyController : ApiController
	{
		public HttpResponseMessage GetCurrencyList(string token, bool includeBase)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				IREGCurrency sysService = ServiceHostManager.GetSysService<IREGCurrency>();
				List<REGCurrencyViewModel> obj = new List<REGCurrencyViewModel>();
				using (sysService as IDisposable)
				{
					obj = sysService.GetCurrencyViewList(null, includeBase, token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		public HttpResponseMessage GetOrgBaseCurrency(string token)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				IREGCurrency sysService = ServiceHostManager.GetSysService<IREGCurrency>();
				BASCurrencyViewModel obj = new BASCurrencyViewModel();
				using (sysService as IDisposable)
				{
					obj = sysService.GetBaseCurrency(token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
