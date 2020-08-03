using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDBankType
	{
		[OperationContract]
		MActionResult<List<BDBankTypeViewModel>> GetBDBankTypeList(string accessToken = null);

		[OperationContract]
		MActionResult<BDBankTypeEditModel> GetBDBankTypeEditModel(BDBankTypeEditModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveBankType(BDBankTypeEditModel banktype, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteBankType(BDBankTypeModel banktype, string accessToken = null);
	}
}
