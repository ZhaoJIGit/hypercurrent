using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVTransferBusiness
	{
		List<IVTransferListModel> GetTransferList(MContext ctx, string filterString);

		OperationResult UpdateReconcileStatu(MContext ctx, string transferId, IVReconcileStatus statu);

		OperationResult UpdateTransfer(MContext ctx, IVTransferModel model);

		IVTransferModel GetTransferEditModel(MContext ctx, string pkID);

		OperationResult DeleteTransfer(MContext ctx, IVTransferModel model);
	}
}
