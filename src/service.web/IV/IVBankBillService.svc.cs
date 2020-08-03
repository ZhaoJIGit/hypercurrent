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
	public class IVBankBillService : ServiceT<IVBankBillModel>, IIVBankBill
	{
		private readonly IVBankBillBusiness biz = new IVBankBillBusiness();

		public MActionResult<OperationResult> UpdateBankBillEntryList(List<IVBankBillEntryModel> entryList, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.UpdateBankBillEntryList, entryList, accessToken);
		}

		public MActionResult<OperationResult> UpdateBankBillRec(List<IVBankBillReconcileModel> model, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.UpdateBankBillRec, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateReconcileMatch(string entryID, string matchBillID, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.UpdateReconcileMatch, entryID, matchBillID, accessToken);
		}

		public MActionResult<OperationResult> UpdateCashCoding(IVCashCodingEditModel model, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.UpdateCashCoding, model, accessToken);
		}

		public MActionResult<List<IVReconcileTranstionListModel>> GetRecTranstionList(IVReconcileTranstionListFilterModel filter, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.GetRecTranstionList, filter, accessToken);
		}

		public MActionResult<OperationResult> DeleteBankBillEntry(ParamBase param, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.DeleteBankBillEntry, param, accessToken);
		}

		public MActionResult<OperationResult> UpdateBankBillVoucherStatus(List<string> entryIdList, IVBankBillVoucherStatus status, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.UpdateBankBillVoucherStatus, entryIdList, status, accessToken);
		}

		public MActionResult<DataGridJson<IVBankBillEntryModel>> GetCashCodingList(IVBankBillEntryListFilterModel filter, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.GetCashCodingList, filter, accessToken);
		}

		public MActionResult<IVBankBillEntryModel> GetBankBillEntryEditModel(string bankBillEntryId, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.GetBankBillEntryEditModel, bankBillEntryId, accessToken);
		}

		public MActionResult<IVBankBillRecListModel> GetBankBillRecByID(string id, string bankId, string accessToken = null)
		{
			return base.RunFunc(biz.GetBankBillRecByID, id, bankId, accessToken);
		}

		public MActionResult<DataGridJson<IVBankBillRecListModel>> GetBankBillRecList(IVBankBillRecListFilterModel filter, string accessToken = null)
		{
			return base.RunFunc(biz.GetBankBillRecList, filter, accessToken);
		}

		public MActionResult<List<IVBankBillReconcileEntryModel>> GetIVBankBillReconcileEntryModelList(string billType, string billId, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.GetIVBankBillReconcileEntryModelList, billType, billId, accessToken);
		}

		public MActionResult<bool> CheckIsExistsBankBillReconcile(string billType, string billId, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.CheckIsExistsBankBillReconcile, billType, billId, accessToken);
		}

		public MActionResult<OperationResult> DeleteBankBillReconcile(string billType, string billId, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.DeleteBankBillReconcile, billType, billId, accessToken);
		}

		public MActionResult<IVBankBillEntryModel> GetIVBankBillEntryModelByBankBillReconcileMID(string bankBillReconcileMID, string accessToken = null)
		{
			IVBankBillBusiness iVBankBillBusiness = biz;
			return base.RunFunc(iVBankBillBusiness.GetIVBankBillEntryModelByBankBillReconcileMID, bankBillReconcileMID, accessToken);
		}
	}
}
