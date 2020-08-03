using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVPayment
	{
		[OperationContract]
		MActionResult<List<IVPaymentListModel>> GetPaymentList(string filterString, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdatePayment(IVPaymentModel model, string accessToken = null);

		[OperationContract]
		MActionResult<IVPaymentModel> GetPaymentEditModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<IVPaymentViewModel> GetPaymentViewModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeletePayment(IVPaymentModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReconcileStatu(string paymentId, IVReconcileStatus statu, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> BatchUpdateReconcileStatu(ParamBase param, IVReconcileStatus statu, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVPaymentModel>> GetInitList(string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<IVPaymentModel>> GetInitPaymentListByPage(IVPaymentListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeletePaymentList(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportPaymentList(List<IVPaymentModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVPaymentModel>> GetPaymentListIncludeEntry(IVPaymentListFilterModel filter, string accessToken = null);
	}
}
