using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

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
		DataGridJson<SECUserModel> GetUserList(MContext ctx, string email, string name, int pageIndex = 0, int pageSize = 10);
		[NoAuthorization]

		OperationResult UpdateStatus(MContext ctx, string mItemId,int status);

		SECUserModel GetModelByEmail(MContext ctx, string email);

		SECUserlModel GetMulitLangModel(MContext ctx, SqlWhere filter);

		OperationResult UpdateUserMulitLangModel(MContext ctx, SECUserModel model);

		OperationResult UpdateOrgListShowType(MContext ctx, int type);

		OperationResult ValidateCreateOrgAuth(MContext ctx, int type);
	}
}
