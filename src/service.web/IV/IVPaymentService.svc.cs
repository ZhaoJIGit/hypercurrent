using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.IV;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.IV
{
	public class IVPaymentService : ServiceT<IVPaymentModel>, IIVPayment
	{
		private readonly IIVPaymentBusiness biz = new IVPaymentBusiness();

		public MActionResult<List<IVPaymentListModel>> GetPaymentList(string filterString, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.GetPaymentList, filterString, accessToken);
		}

		public MActionResult<OperationResult> UpdatePayment(IVPaymentModel model, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.UpdatePayment, model, accessToken);
		}

		public MActionResult<IVPaymentModel> GetPaymentEditModel(string pkID, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.GetPaymentEditModel, pkID, accessToken);
		}

		public MActionResult<IVPaymentViewModel> GetPaymentViewModel(string pkID, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.GetPaymentViewModel, pkID, accessToken);
		}

		public MActionResult<OperationResult> DeletePayment(IVPaymentModel model, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.DeletePayment, model, accessToken);
		}

		public MActionResult<List<IVPaymentModel>> GetInitList(string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.GetInitList, accessToken);
		}

		public MActionResult<DataGridJson<IVPaymentModel>> GetInitPaymentListByPage(IVPaymentListFilterModel filter, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.GetInitPaymentListByPage, filter, accessToken);
		}

		public MActionResult<OperationResult> DeletePaymentList(ParamBase param, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.DeletePaymentList, param, accessToken);
		}

		public MActionResult<OperationResult> UpdateReconcileStatu(string paymentId, IVReconcileStatus statu, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.UpdateReconcileStatu, paymentId, statu, accessToken);
		}

		public MActionResult<OperationResult> BatchUpdateReconcileStatu(ParamBase param, IVReconcileStatus statu, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.UpdateReconcileStatu, param, statu, accessToken);
		}

		public MActionResult<OperationResult> ImportPaymentList(List<IVPaymentModel> list, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.ImportPaymentList, list, accessToken);
		}

		public MActionResult<List<IVPaymentModel>> GetPaymentListIncludeEntry(IVPaymentListFilterModel filter, string accessToken = null)
		{
			IIVPaymentBusiness iIVPaymentBusiness = biz;
			return base.RunFunc(iIVPaymentBusiness.GetPaymentListIncludeEntry, filter, accessToken);
		}
	}
}
