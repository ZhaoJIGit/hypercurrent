using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.SEC
{
	[ServiceContract]
	public interface ISECSendLinkInfo
	{
		[OperationContract]
		MActionResult<bool> IsValidLink(string itemid, string accessToken = null);

		[OperationContract]
		MActionResult<string> InsertLink(SECSendLinkInfoModel linkModel, string accessToken = null);

		[OperationContract]
		MActionResult<string> DeleteLink(string itemid, string accessToken = null);

		[OperationContract]
		MActionResult<SECSendLinkInfoModel> GetModel(string itemId, string accessToken = null);
	}
}
