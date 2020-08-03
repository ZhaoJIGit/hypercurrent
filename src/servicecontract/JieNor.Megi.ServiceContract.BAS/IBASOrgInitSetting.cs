using JieNor.Megi.Core.Context;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASOrgInitSetting
	{
		[OperationContract]
		MActionResult<OperationResult> GLSetupSuccess(string accessToken = null);
	}
}
