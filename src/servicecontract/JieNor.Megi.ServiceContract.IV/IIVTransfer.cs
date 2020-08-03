using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVTransfer
	{
		[OperationContract]
		MActionResult<List<IVTransferListModel>> GetTransferList(string filterString, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateTransfer(IVTransferModel model, string accessToken = null);

		[OperationContract]
		MActionResult<IVTransferModel> GetTransferEditModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteTransfer(IVTransferModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReconcileStatu(string transferId, IVReconcileStatus statu, string accessToken = null);
	}
}
