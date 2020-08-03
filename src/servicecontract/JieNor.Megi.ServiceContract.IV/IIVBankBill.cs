using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVBankBill
	{
		[OperationContract]
		MActionResult<OperationResult> UpdateBankBillEntryList(List<IVBankBillEntryModel> entryList, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateBankBillRec(List<IVBankBillReconcileModel> model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReconcileMatch(string entryID, string matchBillID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateCashCoding(IVCashCodingEditModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteBankBillEntry(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateBankBillVoucherStatus(List<string> entryIdList, IVBankBillVoucherStatus status, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<IVBankBillEntryModel>> GetCashCodingList(IVBankBillEntryListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVReconcileTranstionListModel>> GetRecTranstionList(IVReconcileTranstionListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<IVBankBillEntryModel> GetBankBillEntryEditModel(string bankBillEntryId, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<IVBankBillRecListModel>> GetBankBillRecList(IVBankBillRecListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<IVBankBillRecListModel> GetBankBillRecByID(string id, string bankId, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVBankBillReconcileEntryModel>> GetIVBankBillReconcileEntryModelList(string billType, string billId, string accessToken = null);

		[OperationContract]
		MActionResult<bool> CheckIsExistsBankBillReconcile(string billType, string billId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteBankBillReconcile(string billType, string billId, string accessToken = null);

		[OperationContract]
		MActionResult<IVBankBillEntryModel> GetIVBankBillEntryModelByBankBillReconcileMID(string bankBillReconcileMID, string accessToken = null);
	}
}
