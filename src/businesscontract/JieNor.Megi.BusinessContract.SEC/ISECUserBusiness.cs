using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.SEC
{
	public interface ISECUserBusiness : IDataContract<SECUserModel>
	{
		[NoAuthorization]
		OperationResult Register(MContext ctx, SECUserModel model);

		OperationResult SureRegister(MContext ctx, SECAccountModel model);

		OperationResult PutNewPwd(MContext ctx, SECUserModel model);

		bool IsExistEmail(MContext ctx, string email);

		SECUserModel GetUserModel(MContext ctx, string email);

		SECUserModel GetModelByEmail(MContext ctx, string email);

		SECUserlModel GetMulitLangModel(MContext ctx, SqlWhere filter);

		OperationResult UpdateUserMulitLangModel(MContext ctx, SECUserModel model);

		OperationResult UpdateOrgListShowType(MContext ctx, int type);

		OperationResult ValidateCreateOrgAuth(MContext ctx, int type);
	}
}
