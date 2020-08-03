using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.SEC
{
	[ServiceContract]
	public interface ISECUser
	{
		[OperationContract]
		MActionResult<OperationResult> SureRegister(SECAccountModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> PutNewPwd(SECUserModel model, string accessToken = null);

		[OperationContract]
		MActionResult<bool> IsExistEmail(string email, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Register(SECUserModel model, string accessToken = null);

		[OperationContract]
		MActionResult<SECUserModel> GetModelByEmail(string email, string accessToken = null);

		[OperationContract]
		MActionResult<SECUserModel> GetUserModel(string email, string accessToken = null);

		[OperationContract]
		MActionResult<SECUserlModel> GetMulitLangModel(SqlWhere filter);

		[OperationContract]
		MActionResult<OperationResult> UpdateUserMulitLangModel(SECUserModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateOrgListShowType(int type, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ValidateCreateOrgAuth(int type, string accessToken = null);
	}
}
