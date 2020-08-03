using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVBankBillBusiness : IIVBankBillBusiness
	{
		private readonly IVBankBillEntryRepository _bankBillEntry = new IVBankBillEntryRepository();

		public OperationResult UpdateBankBillEntryList(MContext ctx, List<IVBankBillEntryModel> entryList)
		{
			return IVBankBillEntryRepository.UpdateBankBillEntryList(ctx, entryList);
		}

		public OperationResult UpdateBankBillRec(MContext ctx, List<IVBankBillReconcileModel> list)
		{
			return _bankBillEntry.UpdateBankBillRec(ctx, list);
		}

		public OperationResult UpdateReconcileMatch(MContext ctx, string entryID, string matchBillID)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(_bankBillEntry.GetUpdateReconcileMatchCmd(ctx, entryID, matchBillID));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0 && true);
			return operationResult;
		}

		public OperationResult UpdateCashCoding(MContext ctx, IVCashCodingEditModel model)
		{
			return IVBankBillEntryRepository.UpdateCashCoding(ctx, model);
		}

		public List<IVReconcileTranstionListModel> GetRecTranstionList(MContext ctx, IVReconcileTranstionListFilterModel filter)
		{
			List<IVReconcileTranstionListModel> recTranstionList = IVBankBillEntryRepository.GetRecTranstionList(ctx, filter);
			if (recTranstionList != null)
			{
				foreach (IVReconcileTranstionListModel item in recTranstionList)
				{
					if (item.MType == "Transfer")
					{
						if (item.MSpentAmtFor == decimal.Zero)
						{
							item.MDescription = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Bank, "BankTransferFrom", "Bank Transfer from {0}", item.MDescription);
						}
						else
						{
							item.MDescription = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Bank, "BankTransferTo", "Bank Transfer to {0}", item.MDescription);
						}
					}
				}
			}
			return recTranstionList;
		}

		public OperationResult UpdateBankBillVoucherStatus(MContext ctx, List<string> entryIdList, IVBankBillVoucherStatus status)
		{
			return IVBankBillEntryRepository.UpdateBankBillVoucherStatus(ctx, entryIdList, status);
		}

		public OperationResult DeleteBankBillEntry(MContext ctx, ParamBase param)
		{
			if (string.IsNullOrWhiteSpace(param.KeyIDs))
			{
				throw new Exception("Error Parament");
			}
			string paymentIDsByBankBillEntry = IVBankBillEntryRepository.GetPaymentIDsByBankBillEntry(ctx, param.KeyIDSWithSingleQuote);
			string receiveIDsByBankBillEntry = IVBankBillEntryRepository.GetReceiveIDsByBankBillEntry(ctx, param.KeyIDSWithSingleQuote);
			string transferIDsByBankBillEntry = IVBankBillEntryRepository.GetTransferIDsByBankBillEntry(ctx, param.KeyIDSWithSingleQuote);
			string autoCreatePaymentIDs = IVBankBillEntryRepository.GetAutoCreatePaymentIDs(ctx, param.KeyIDSWithSingleQuote);
			string autoCreateReceiveIDs = IVBankBillEntryRepository.GetAutoCreateReceiveIDs(ctx, param.KeyIDSWithSingleQuote);
			List<IVBankBillEntryModel> list = new List<IVBankBillEntryModel>();
			foreach (string mKeyID in param.MKeyIDList)
			{
				list.Add(new IVBankBillEntryModel
				{
					MEntryID = mKeyID,
					MCheckState = 2
				});
			}
			OperationResult result = _bankBillEntry.InsertOrUpdateModels(ctx, list, "MCheckState");
			IVBankBillEntryRepository.UpdateReconcileAmt(ctx, paymentIDsByBankBillEntry, receiveIDsByBankBillEntry, transferIDsByBankBillEntry, autoCreatePaymentIDs, autoCreateReceiveIDs);
			return result;
		}

		public DataGridJson<IVBankBillEntryModel> GetCashCodingList(MContext ctx, IVBankBillEntryListFilterModel filter)
		{
			BDContactsRepository bDContactsRepository = new BDContactsRepository();
			List<NameValueModel> contactNameInfoList = bDContactsRepository.GetContactNameInfoList(ctx);
			BDEmployeesRepository bDEmployeesRepository = new BDEmployeesRepository();
			List<NameValueModel> employeeNameInfoList = bDEmployeesRepository.GetEmployeeNameInfoList(ctx, false);
			DataGridJson<IVBankBillEntryModel> cashCodingList = IVBankBillEntryRepository.GetCashCodingList(ctx, filter);
			if (cashCodingList.rows != null && cashCodingList.rows.Count > 0)
			{
				List<IVBankBillEntryModel> list = new List<IVBankBillEntryModel>();
				List<string> bankBillEntryIdList = (from t in cashCodingList.rows
				select t.MEntryID).ToList();
				List<IVBankBillEntryModel> cashCodingList2 = IVBankBillEntryRepository.GetCashCodingList(ctx, bankBillEntryIdList);
				foreach (IVBankBillEntryModel row in cashCodingList.rows)
				{
					IVBankBillEntryModel item = row;
					ResetBankBillEntry(ref item, contactNameInfoList, employeeNameInfoList);
					list.Add(item);
					if (cashCodingList2 != null && cashCodingList2.Count != 0)
					{
						List<IVBankBillEntryModel> list2 = (from t in cashCodingList2
						where t.MParentID == row.MEntryID
						orderby t.MSeq
						select t).ToList();
						if (list2 != null && list2.Count != 0)
						{
							foreach (IVBankBillEntryModel item3 in list2)
							{
								IVBankBillEntryModel item2 = item3;
								ResetBankBillEntry(ref item2, contactNameInfoList, employeeNameInfoList);
								list.Add(item2);
							}
						}
					}
				}
				cashCodingList.rows = list;
			}
			return cashCodingList;
		}

		private void ResetBankBillEntry(MContext ctx, ref IVBankBillEntryModel srcModel)
		{
			BDContactsRepository bDContactsRepository = new BDContactsRepository();
			List<NameValueModel> contactNameInfoList = bDContactsRepository.GetContactNameInfoList(ctx);
			BDEmployeesRepository bDEmployeesRepository = new BDEmployeesRepository();
			List<NameValueModel> employeeNameInfoList = bDEmployeesRepository.GetEmployeeNameInfoList(ctx, false);
			ResetBankBillEntry(ref srcModel, contactNameInfoList, employeeNameInfoList);
		}

		private void ResetBankBillEntry(ref IVBankBillEntryModel srcModel, List<NameValueModel> contactNameList, List<NameValueModel> employeeNameList)
		{
			string mRef = srcModel.MRef;
			srcModel.MRef = srcModel.MUserRef;
			srcModel.MDesc = mRef;
			if (!string.IsNullOrEmpty(srcModel.MContactID))
			{
				string tempContactId = srcModel.MContactID.Split('_')[0];
				if (!contactNameList.Any((NameValueModel t) => t.MValue == tempContactId) && !employeeNameList.Any((NameValueModel t) => t.MValue == tempContactId))
				{
					goto IL_009b;
				}
				return;
			}
			goto IL_009b;
			IL_009b:
			srcModel.MContactID = "";
			NameValueModel contactNameInfo = GetContactNameInfo(contactNameList, srcModel.MTransAcctName);
			if (contactNameInfo != null)
			{
				string contactTypeByTransType = GetContactTypeByTransType(srcModel, contactNameInfo);
				srcModel.MContactID = $"{contactNameInfo.MValue}_{contactTypeByTransType}";
				srcModel.MAccountID = (string.IsNullOrEmpty(srcModel.MAccountID) ? contactNameInfo.MTag : srcModel.MAccountID);
				srcModel.MContactName = contactNameInfo.MName;
			}
			else
			{
				NameValueModel employeeNameInfo = GetEmployeeNameInfo(employeeNameList, srcModel.MTransAcctName);
				if (employeeNameInfo != null)
				{
					srcModel.MContactID = $"{employeeNameInfo.MValue}_{Convert.ToInt32(BDContactType.Employee)}";
					srcModel.MAccountID = (string.IsNullOrEmpty(srcModel.MAccountID) ? employeeNameInfo.MValue1 : srcModel.MAccountID);
					srcModel.MContactName = GlobalFormat.GetUserName(employeeNameInfo.MName, employeeNameInfo.MTag, null);
				}
			}
		}

		private string GetContactTypeByTransType(IVBankBillEntryModel srcModel, NameValueModel model)
		{
			List<string> list = (from f in model.MValue1.Split(',')
			where !string.IsNullOrWhiteSpace(f)
			select f).ToList();
			if (list.Count() == 1)
			{
				return list[0];
			}
			string empty = string.Empty;
			string text = Convert.ToString(1);
			string text2 = Convert.ToString(2);
			return (!(srcModel.MSpentAmt > decimal.Zero) || !model.MValue1.Contains(text)) ? ((!(srcModel.MReceivedAmt > decimal.Zero) || !model.MValue1.Contains(text2)) ? (from f in list
			orderby f descending
			select f).FirstOrDefault() : text2) : text;
		}

		private NameValueModel GetContactNameInfo(List<NameValueModel> contactNameList, string acctName)
		{
			if (string.IsNullOrEmpty(acctName) || contactNameList == null || contactNameList.Count == 0)
			{
				return null;
			}
			acctName = acctName.Trim().ToLower();
			return (from t in contactNameList
			where !string.IsNullOrEmpty(t.MName) && t.MName.Trim().ToLower() == acctName.Trim()
			select t).FirstOrDefault();
		}

		private NameValueModel GetEmployeeNameInfo(List<NameValueModel> employeeNameList, string acctName)
		{
			if (string.IsNullOrEmpty(acctName) || employeeNameList == null || employeeNameList.Count == 0)
			{
				return null;
			}
			acctName = acctName.Replace(" ", "").ToLower();
			return (from t in employeeNameList
			where $"{t.MName}{t.MTag}".Replace(" ", "").ToLower() == acctName || $"{t.MTag}{t.MName}".Replace(" ", "").ToLower() == acctName
			select t).FirstOrDefault();
		}

		public IVBankBillEntryModel GetBankBillEntryEditModel(MContext ctx, string bankBillEntryId)
		{
			IVBankBillEntryModel dataModel = _bankBillEntry.GetDataModel(ctx, bankBillEntryId, false);
			if (dataModel != null)
			{
				dataModel.MIsGLOpen = GLInterfaceRepository.IsPeriodUnclosed(ctx, dataModel.MDate).Success;
			}
			if (string.IsNullOrWhiteSpace(dataModel.MContactID))
			{
				ResetBankBillEntry(ctx, ref dataModel);
			}
			return dataModel;
		}

		public IVBankBillRecListModel GetBankBillRecByID(MContext ctx, string id, string bankId)
		{
			return _bankBillEntry.GetBankBillRecByID(ctx, id, bankId);
		}

		public DataGridJson<IVBankBillRecListModel> GetBankBillRecList(MContext ctx, IVBankBillRecListFilterModel filter)
		{
			return _bankBillEntry.GetBankBillRecList(ctx, filter);
		}

		public List<IVBankBillReconcileEntryModel> GetIVBankBillReconcileEntryModelList(MContext ctx, string billType, string billId)
		{
			return IVBankBillReconcileEntryRepository.GetIVBankBillReconcileEntryModelList(ctx, billType, billId);
		}

		public bool CheckIsExistsBankBillReconcile(MContext ctx, string billType, string billId)
		{
			return IVBankBillReconcileEntryRepository.CheckIsExistsBankBillReconcile(ctx, billType, billId);
		}

		public OperationResult DeleteBankBillReconcile(MContext ctx, string billType, string billId)
		{
			return IVBankBillReconcileEntryRepository.DeleteBankBillReconcile(ctx, billType, billId);
		}

		public IVBankBillEntryModel GetIVBankBillEntryModelByBankBillReconcileMID(MContext ctx, string bankBillReconcileMID)
		{
			return IVBankBillEntryRepository.GetIVBankBillEntryModelByBankBillReconcileMID(ctx, bankBillReconcileMID);
		}
	}
}
