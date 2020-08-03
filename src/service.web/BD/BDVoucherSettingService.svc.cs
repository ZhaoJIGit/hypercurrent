using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDVoucherSettingService : ServiceT<BDVoucherSettingModel>, IBDVoucherSetting
	{
		private readonly IBDVoucherSettingBusiness biz = new BDVoucherSettingBusiness();

		public MActionResult<List<BDVoucherSettingCategoryModel>> GetVoucherSettingCategoryList(string accessToken = null)
		{
			IBDVoucherSettingBusiness iBDVoucherSettingBusiness = biz;
			return base.RunFunc(iBDVoucherSettingBusiness.GetVoucherSettingCategoryList, accessToken);
		}

		public MActionResult<OperationResult> SaveVoucherSetting(List<BDVoucherSettingModel> list, string accessToken = null)
		{
			IBDVoucherSettingBusiness iBDVoucherSettingBusiness = biz;
			return base.RunFunc(iBDVoucherSettingBusiness.SaveVoucherSetting, list, accessToken);
		}
	}
}
