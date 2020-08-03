using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Verification;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVVerification
	{
		[OperationContract]
		MActionResult<List<IVVerificationListModel>> GetVerificationList(IVVerificationListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<string> UpdateVerification(IVVerificationModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateVerificationList(List<IVVerificationModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteVerification(IVVerificationModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteVerificationByPKID(string pkID, bool isMergePay = false, string accessToken = null);

		[OperationContract]
		MActionResult<IVVerificationModel> GetVerificationEditModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVVerificationInforModel>> GetCustomerWaitForVerificationInfor(string contactID, string currencyID, string bizBillType, string bizType, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVVerificationInforModel>> GetCustomerWaitForVerificationInforByBillId(string billId, string bizType, string accessToken = null);

		[OperationContract]
		MActionResult<bool> CheckIsCanEditOrVoidOrDelete(string billType, string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVBatchPaymentModel>> GetBatchPaymentList(ParamBase para, string selectObj, bool isMergePay, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> BatchPaymentUpdate(IVBatchPayHeadModel headModel, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> IsSuccessBatch(ParamBase para, string selectObj, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVVerifiactionDocMapModel>> GetVerificationIncludeBillList(IVVerificationFilterModel filter, string accessToken = null);
	}
}
