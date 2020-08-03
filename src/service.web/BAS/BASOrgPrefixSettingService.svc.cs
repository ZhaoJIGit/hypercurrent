using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASOrgPrefixSettingService : ServiceT<BASOrgPrefixSettingModel>, IBASOrgPrefixSetting
	{
		private readonly IBASOrgPrefixSettingBusiness biz = new BASOrgPrefixSettingBusiness();

		public MActionResult<OperationResult> UpdateOrgPrefixSettingModel(BASOrgPrefixSettingModel model, string accessToken = null)
		{
			IBASOrgPrefixSettingBusiness iBASOrgPrefixSettingBusiness = biz;
			return base.RunFunc(iBASOrgPrefixSettingBusiness.UpdateOrgPrefixSettingModel, model, accessToken);
		}

		public MActionResult<BASOrgPrefixSettingModel> GetOrgPrefixSettingModel(string module, string accessToken = null)
		{
			IBASOrgPrefixSettingBusiness iBASOrgPrefixSettingBusiness = biz;
			return base.RunFunc(iBASOrgPrefixSettingBusiness.GetOrgPrefixSettingModel, module, accessToken);
		}
	}
}
