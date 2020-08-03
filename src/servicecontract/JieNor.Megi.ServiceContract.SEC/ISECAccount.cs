using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.SEC
{
	[ServiceContract]
	public interface ISECAccount
	{
		[OperationContract]
		MActionResult<OperationResult> UpdateAccountData(SECUserModel model, int type, string accessToken = null);

		[OperationContract]
		MActionResult<SECUserModel> GetUserModelByKey(string MItemID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> LoginForUpdateEmail(string[] token, string accessToken = null);
	}
}
