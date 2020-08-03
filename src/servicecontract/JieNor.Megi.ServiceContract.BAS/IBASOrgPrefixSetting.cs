using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASOrgPrefixSetting
	{
		[OperationContract]
		MActionResult<OperationResult> UpdateOrgPrefixSettingModel(BASOrgPrefixSettingModel model, string accessToken = null);

		[OperationContract]
		MActionResult<BASOrgPrefixSettingModel> GetOrgPrefixSettingModel(string module, string accessToken = null);
	}
}
