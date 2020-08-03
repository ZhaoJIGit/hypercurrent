using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.BusinessService.SEC;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.SEC;
using JieNor.Megi.ServiceContract.SEC;

namespace JieNor.Megi.Service.Web.SEC
{
	public class SECUserAccountService : ServiceT<BDModel>, ISECUserAccount
	{
		private readonly ISECUserAccountBusiness biz = new SECUserAccountBusiness();

		public MActionResult<SECLoginResultModel> Login(SECLoginModel model, string accessToken = null)
		{
			ISECUserAccountBusiness iSECUserAccountBusiness = biz;
			return base.RunFunc(iSECUserAccountBusiness.Login, model, accessToken);
		}
	}
}
