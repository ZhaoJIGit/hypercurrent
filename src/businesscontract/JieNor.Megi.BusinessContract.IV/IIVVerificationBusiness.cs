using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Verification;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVVerificationBusiness
	{
		List<IVVerificationListModel> GetVerificationList(MContext ctx, IVVerificationListFilterModel filter);

		void UpdateVerification(MContext ctx, IVVerificationModel model);

		OperationResult UpdateVerificationList(MContext ctx, List<IVVerificationModel> list);

		OperationResult DeleteVerification(MContext ctx, IVVerificationModel model);

		OperationResult DeleteVerificationByPKID(MContext ctx, string pkID, bool isMergePay = false);

		IVVerificationModel GetVerificationEditModel(MContext ctx, string pkID);

		List<IVVerificationInforModel> GetCustomerWaitForVerificationInfor(MContext ctx, string contactID, string currencyID, string bizBillType, string bizType);

		List<IVVerificationInforModel> GetCustomerWaitForVerificationInfor(MContext ctx, string invoiceId, string bizType);

		bool CheckIsCanEditOrVoidOrDelete(MContext ctx, string billType, string pkID);

		List<IVBatchPaymentModel> GetBatchPaymentList(MContext ctx, ParamBase para, string selectObj, bool isMergePay);

		OperationResult BatchPaymentUpdate(MContext ctx, IVBatchPayHeadModel headModel);

		OperationResult IsSuccessBatch(MContext ctx, ParamBase para, string selectObj);

		List<IVVerifiactionDocMapModel> GetVerificationList(MContext ctx, IVVerificationFilterModel filter);
	}
}
