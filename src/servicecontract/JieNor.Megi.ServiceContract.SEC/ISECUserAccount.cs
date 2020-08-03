using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.SEC;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.SEC
{
	[ServiceContract]
	public interface ISECUserAccount
	{
		[OperationContract]
		MActionResult<SECLoginResultModel> Login(SECLoginModel model, string accessToken = null);
	}
}
