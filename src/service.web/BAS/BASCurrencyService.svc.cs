using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASCurrencyService : ServiceT<BASCurrencyModel>, IBASCurrency
	{
		private readonly IBASCurrencyBusiness biz = new BASCurrencyBusiness();

		public MActionResult<List<BASCurrencyModel>> GetList(string accessToken = null)
		{
			IBASCurrencyBusiness iBASCurrencyBusiness = biz;
			return base.RunFunc(iBASCurrencyBusiness.GetList, accessToken);
		}

		public MActionResult<BASCurrencyModel> GetModel(BASCurrencyModel model, string accessToken = null)
		{
			IBASCurrencyBusiness iBASCurrencyBusiness = biz;
			return base.RunFunc(iBASCurrencyBusiness.GetModel, model, accessToken);
		}

		public MActionResult<BASCurrencyViewModel> GetViewModel(BASCurrencyModel model, string accessToken = null)
		{
			IBASCurrencyBusiness iBASCurrencyBusiness = biz;
			return base.RunFunc(iBASCurrencyBusiness.GetViewModel, model, accessToken);
		}

		public MActionResult<List<BASCurrencyViewModel>> GetViewList(bool flag = true, string accessToken = null)
		{
			IBASCurrencyBusiness iBASCurrencyBusiness = biz;
			return base.RunFunc(iBASCurrencyBusiness.GetViewList, flag, accessToken);
		}
	}
}
