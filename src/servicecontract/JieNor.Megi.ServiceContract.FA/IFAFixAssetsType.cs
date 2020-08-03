using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FA
{
	[ServiceContract]
	public interface IFAFixAssetsType
	{
		[OperationContract]
		MActionResult<List<FAFixAssetsTypeModel>> GetFixAssetsTypeList(string itemID = null, string accessToken = null);

		[OperationContract]
		MActionResult<FAFixAssetsTypeModel> GetFixAssetsTypeModel(string itemID = null, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteFixAssetsType(List<string> itemIDs, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveFixAssetsType(FAFixAssetsTypeModel model, string accessToken = null);
	}
}
