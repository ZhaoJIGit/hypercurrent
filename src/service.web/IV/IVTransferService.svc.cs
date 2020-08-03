using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.IV;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.IV
{
	public class IVTransferService : ServiceT<IVBankBillModel>, IIVTransfer
	{
		private readonly IIVTransferBusiness biz = new IVTransferBusiness();

		public MActionResult<List<IVTransferListModel>> GetTransferList(string filterString, string accessToken = null)
		{
			IIVTransferBusiness iIVTransferBusiness = biz;
			return base.RunFunc(iIVTransferBusiness.GetTransferList, filterString, accessToken);
		}

		public MActionResult<OperationResult> UpdateTransfer(IVTransferModel model, string accessToken = null)
		{
			IIVTransferBusiness iIVTransferBusiness = biz;
			return base.RunFunc(iIVTransferBusiness.UpdateTransfer, model, accessToken);
		}

		public MActionResult<IVTransferModel> GetTransferEditModel(string pkID, string accessToken = null)
		{
			IIVTransferBusiness iIVTransferBusiness = biz;
			return base.RunFunc(iIVTransferBusiness.GetTransferEditModel, pkID, accessToken);
		}

		public MActionResult<OperationResult> DeleteTransfer(IVTransferModel model, string accessToken = null)
		{
			IIVTransferBusiness iIVTransferBusiness = biz;
			return base.RunFunc(iIVTransferBusiness.DeleteTransfer, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateReconcileStatu(string transferId, IVReconcileStatus statu, string accessToken = null)
		{
			IIVTransferBusiness iIVTransferBusiness = biz;
			return base.RunFunc(iIVTransferBusiness.UpdateReconcileStatu, transferId, statu, accessToken);
		}
	}
}
