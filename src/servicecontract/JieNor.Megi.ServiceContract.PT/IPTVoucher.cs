using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.PT;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.PT
{
	[ServiceContract]
	public interface IPTVoucher
	{
		[OperationContract]
		MActionResult<List<PTVoucherModel>> GetList(string accessToken = null);

		[OperationContract]
		MActionResult<PTVoucherModel> GetModel(string itemID, bool isFromPrint = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Sort(string ids, string accessToken = null);

		[OperationContract]
		MActionResult<PTVoucherModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertOrUpdate(PTVoucherModel modelData, string fields = null, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null);
	}
}
