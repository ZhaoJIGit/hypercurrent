using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Verification;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.IV;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.IV
{
	public class IVVerificationService : ServiceT<IVBankBillModel>, IIVVerification
	{
		private readonly IIVVerificationBusiness biz = new IVVerificationBusiness();

		public MActionResult<List<IVVerificationListModel>> GetVerificationList(IVVerificationListFilterModel filter, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.GetVerificationList, filter, accessToken);
		}

		public MActionResult<string> UpdateVerification(IVVerificationModel model, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunAction<string, IVVerificationModel>(iIVVerificationBusiness.UpdateVerification, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateVerificationList(List<IVVerificationModel> list, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.UpdateVerificationList, list, accessToken);
		}

		public MActionResult<OperationResult> DeleteVerification(IVVerificationModel model, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.DeleteVerification, model, accessToken);
		}

		public MActionResult<OperationResult> DeleteVerificationByPKID(string pkID, bool isMergePay = false, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.DeleteVerificationByPKID, pkID, isMergePay, accessToken);
		}

		public MActionResult<IVVerificationModel> GetVerificationEditModel(string pkID, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.GetVerificationEditModel, pkID, accessToken);
		}

		public MActionResult<List<IVVerificationInforModel>> GetCustomerWaitForVerificationInfor(string contactID, string currencyID, string bizBillType, string bizType, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.GetCustomerWaitForVerificationInfor, contactID, currencyID, bizBillType, bizType, accessToken);
		}

		public MActionResult<List<IVVerificationInforModel>> GetCustomerWaitForVerificationInforByBillId(string billId, string bizType, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.GetCustomerWaitForVerificationInfor, billId, bizType, accessToken);
		}

		public MActionResult<bool> CheckIsCanEditOrVoidOrDelete(string billType, string pkID, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.CheckIsCanEditOrVoidOrDelete, billType, pkID, accessToken);
		}

		public MActionResult<List<IVBatchPaymentModel>> GetBatchPaymentList(ParamBase para, string selectObj, bool isMergePay, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.GetBatchPaymentList, para, selectObj, isMergePay, null);
		}

		public MActionResult<OperationResult> BatchPaymentUpdate(IVBatchPayHeadModel headModel, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.BatchPaymentUpdate, headModel, null);
		}

		public MActionResult<OperationResult> IsSuccessBatch(ParamBase para, string selectObj, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.IsSuccessBatch, para, selectObj, null);
		}

		public MActionResult<List<IVVerifiactionDocMapModel>> GetVerificationIncludeBillList(IVVerificationFilterModel filter, string accessToken = null)
		{
			IIVVerificationBusiness iIVVerificationBusiness = biz;
			return base.RunFunc(iIVVerificationBusiness.GetVerificationList, filter, accessToken);
		}
	}
}
