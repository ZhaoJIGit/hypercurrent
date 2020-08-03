using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASOrgInitSettingService : ServiceT<BASOrgInitSettingModel>, IBASOrgInitSetting
	{
		private readonly IBASOrgInitSettingBusiness biz = new BASOrgInitSettingBusiness();

		public MActionResult<OperationResult> GLSetupSuccess(string accessToken = null)
		{
			IBASOrgInitSettingBusiness iBASOrgInitSettingBusiness = biz;
			return base.RunFunc(iBASOrgInitSettingBusiness.GLSetupSuccess, accessToken);
		}
	}
}
