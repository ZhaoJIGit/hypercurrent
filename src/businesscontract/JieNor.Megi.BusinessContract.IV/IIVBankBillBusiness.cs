using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVBankBillBusiness
	{
		OperationResult UpdateBankBillEntryList(MContext ctx, List<IVBankBillEntryModel> entryList);

		OperationResult UpdateBankBillRec(MContext ctx, List<IVBankBillReconcileModel> model);

		OperationResult UpdateReconcileMatch(MContext ctx, string entryID, string matchBillID);

		OperationResult UpdateCashCoding(MContext ctx, IVCashCodingEditModel model);

		OperationResult DeleteBankBillEntry(MContext ctx, ParamBase param);

		OperationResult UpdateBankBillVoucherStatus(MContext ctx, List<string> entryIdList, IVBankBillVoucherStatus status);

		DataGridJson<IVBankBillEntryModel> GetCashCodingList(MContext ctx, IVBankBillEntryListFilterModel filter);

		List<IVReconcileTranstionListModel> GetRecTranstionList(MContext ctx, IVReconcileTranstionListFilterModel filter);

		IVBankBillEntryModel GetBankBillEntryEditModel(MContext ctx, string bankBillEntryId);

		List<IVBankBillReconcileEntryModel> GetIVBankBillReconcileEntryModelList(MContext ctx, string billType, string billId);

		bool CheckIsExistsBankBillReconcile(MContext ctx, string billType, string billId);

		OperationResult DeleteBankBillReconcile(MContext ctx, string billType, string billId);

		IVBankBillEntryModel GetIVBankBillEntryModelByBankBillReconcileMID(MContext ctx, string bankBillReconcileMID);
	}
}
