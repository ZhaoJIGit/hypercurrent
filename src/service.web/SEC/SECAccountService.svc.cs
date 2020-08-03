using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.BusinessService.SEC;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.SEC;

namespace JieNor.Megi.Service.Web.SEC
{
	public class SECAccountService : ServiceT<SECAccountModel>, ISECAccount
	{
		private readonly ISECAccountBusiness biz = new SECAccountBusiness();

		public MActionResult<OperationResult> UpdateAccountData(SECUserModel model, int type, string accessToken = null)
		{
			ISECAccountBusiness iSECAccountBusiness = biz;
			return base.RunFunc(iSECAccountBusiness.UpdateAccountData, model, type, accessToken);
		}

		public MActionResult<SECUserModel> GetUserModelByKey(string MItemID, string accessToken = null)
		{
			ISECAccountBusiness iSECAccountBusiness = biz;
			return base.RunFunc(iSECAccountBusiness.GetUserModelByKey, MItemID, accessToken);
		}

		public MActionResult<OperationResult> LoginForUpdateEmail(string[] token, string accessToken = null)
		{
			ISECAccountBusiness iSECAccountBusiness = biz;
			return base.RunFunc(iSECAccountBusiness.LoginForUpdateEmail, token, accessToken);
		}
	}
}
