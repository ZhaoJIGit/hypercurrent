using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.BusinessService.SEC;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.SEC;

namespace JieNor.Megi.Service.Web.SEC
{
	public class SECUserService : ServiceT<SECUserModel>, ISECUser
	{
		private readonly ISECUserBusiness biz = new SECUserBusiness();

		public MActionResult<OperationResult> SureRegister(SECAccountModel model, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.SureRegister, model, accessToken);
		}

		public MActionResult<OperationResult> PutNewPwd(SECUserModel model, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.PutNewPwd, model, accessToken);
		}

		public MActionResult<bool> IsExistEmail(string email, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.IsExistEmail, email, accessToken);
		}

		public MActionResult<OperationResult> Register(SECUserModel model, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.Register, model, accessToken);
		}

		public MActionResult<SECUserModel> GetModelByEmail(string email, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.GetModelByEmail, email, accessToken);
		}

		public MActionResult<SECUserModel> GetUserModel(string email, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.GetUserModel, email, accessToken);
		}

		public MActionResult<SECUserlModel> GetMulitLangModel(SqlWhere filter)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.GetMulitLangModel, filter, null);
		}

		public MActionResult<OperationResult> UpdateUserMulitLangModel(SECUserModel model, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.UpdateUserMulitLangModel, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateOrgListShowType(int type, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.UpdateOrgListShowType, type, accessToken);
		}

		public MActionResult<OperationResult> ValidateCreateOrgAuth(int type, string accessToken = null)
		{
			ISECUserBusiness iSECUserBusiness = biz;
			return base.RunFunc(iSECUserBusiness.ValidateCreateOrgAuth, type, null);
		}
	}
}
