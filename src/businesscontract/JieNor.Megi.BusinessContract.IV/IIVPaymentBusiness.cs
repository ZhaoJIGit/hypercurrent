using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVPaymentBusiness
	{
		List<IVPaymentListModel> GetPaymentList(MContext ctx, string filterString);

		OperationResult UpdatePayment(MContext ctx, IVPaymentModel model);

		IVPaymentModel GetPaymentEditModel(MContext ctx, string pkID);

		IVPaymentViewModel GetPaymentViewModel(MContext ctx, string pkID);

		OperationResult DeletePayment(MContext ctx, IVPaymentModel model);

		List<IVPaymentModel> GetInitList(MContext ctx);

		DataGridJson<IVPaymentModel> GetInitPaymentListByPage(MContext ctx, IVPaymentListFilterModel filter);

		OperationResult DeletePaymentList(MContext ctx, ParamBase param);

		OperationResult UpdateReconcileStatu(MContext ctx, string paymentId, IVReconcileStatus statu);

		OperationResult UpdateReconcileStatu(MContext ctx, ParamBase param, IVReconcileStatus statu);

		OperationResult ImportPaymentList(MContext ctx, List<IVPaymentModel> list);

		List<IVPaymentModel> GetPaymentListIncludeEntry(MContext ctx, IVPaymentListFilterModel filter);
	}
}
