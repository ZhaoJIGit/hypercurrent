using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.REG
{
	[ServiceContract]
	public interface IREGGlobalization
	{
		[OperationContract]
		MActionResult<OperationResult> GlobalizationUpdate(REGGlobalizationModel model, string accessToken = null);

		[OperationContract]
		MActionResult<REGGlobalizationModel> GetOrgGlobalizationDetail(string orgid, string accessToken = null);
	}
}
