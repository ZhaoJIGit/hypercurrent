using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Param;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVTransactionsBusiness : IIVTransactionsBusiness
	{
		public DataGridJson<IVAccountTransactionsModel> GetTransactionsList(MContext ctx, IVAccountTransactionsListFilterModel param)
		{
			DataGridJson<IVAccountTransactionsModel> transactionsList = IVTransactionsRepository.GetTransactionsList(ctx, param);
			if (transactionsList != null && transactionsList.rows != null && transactionsList.rows.Count > 0)
			{
				for (int i = 0; i < transactionsList.rows.Count; i++)
				{
					IVAccountTransactionsModel iVAccountTransactionsModel = transactionsList.rows[i];
					if (iVAccountTransactionsModel.MBizDate < ctx.MBeginDate)
					{
						iVAccountTransactionsModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.Marked);
					}
					if (!string.IsNullOrEmpty(iVAccountTransactionsModel.MContactName))
					{
						iVAccountTransactionsModel.MDescription = $"{iVAccountTransactionsModel.MDescription}({iVAccountTransactionsModel.MContactName})";
					}
					if (iVAccountTransactionsModel.MType == "Transfer")
					{
						if (string.IsNullOrEmpty(iVAccountTransactionsModel.MSpent))
						{
							iVAccountTransactionsModel.MDescription = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Bank, "BankTransferFrom", "Bank Transfer from {0}", iVAccountTransactionsModel.MDescription);
						}
						else
						{
							iVAccountTransactionsModel.MDescription = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Bank, "BankTransferTo", "Bank Transfer to {0}", iVAccountTransactionsModel.MDescription);
						}
					}
				}
			}
			return transactionsList;
		}

		public IVAccountTransactionsExportModel GetTransactionListForExport(MContext ctx, IVAccountTransactionsListFilterModel filter)
		{
			IVAccountTransactionsExportModel iVAccountTransactionsExportModel = new IVAccountTransactionsExportModel();
			string[] accountIds = null;
			if (!string.IsNullOrWhiteSpace(filter.MBankID))
			{
				accountIds = filter.MBankID.Split(',');
			}
			List<BDBankAccountEditModel> bankAccountList = BDBankAccountRepository.GetBankAccountList(ctx, accountIds, false, null);
			filter.MBankID = string.Join(",", from f in bankAccountList
			select f.MItemID);
			iVAccountTransactionsExportModel.AccountTransactionList = IVTransactionsRepository.GetTransactionsList(ctx, filter).rows;
			if (iVAccountTransactionsExportModel.AccountTransactionList != null && iVAccountTransactionsExportModel.AccountTransactionList.Count() > 0)
			{
				foreach (IVAccountTransactionsModel accountTransaction in iVAccountTransactionsExportModel.AccountTransactionList)
				{
					if (accountTransaction.MType == "Transfer")
					{
						if (string.IsNullOrEmpty(accountTransaction.MSpent))
						{
							accountTransaction.MDescription = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Bank, "BankTransferFrom", "Bank Transfer from {0}", accountTransaction.MDescription);
						}
						else
						{
							accountTransaction.MDescription = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Bank, "BankTransferTo", "Bank Transfer to {0}", accountTransaction.MDescription);
						}
					}
				}
			}
			iVAccountTransactionsExportModel.PaymentList = ModelInfoManager.GetDataModelList<IVPaymentModel>(ctx, new SqlWhere(), false, true);
			iVAccountTransactionsExportModel.ReceiveList = ModelInfoManager.GetDataModelList<IVReceiveModel>(ctx, new SqlWhere(), false, true);
			iVAccountTransactionsExportModel.TransferList = ModelInfoManager.GetDataModelList<IVTransferModel>(ctx, new SqlWhere(), false, true);
			iVAccountTransactionsExportModel.BankAccountList = bankAccountList;
			return iVAccountTransactionsExportModel;
		}

		public OperationResult UpdateReconcileStatu(MContext ctx, IVTransactionsReconcileParam param)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			BDBankAccountEditModel bankModel = new BDBankAccountRepository().GetBDBankAccountEditModel(ctx, param.MBankID);
			if (bankModel == null)
			{
				return operationResult;
			}
			if (!bankModel.MIsNeedReconcile)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "NotArrowReconcile", "Not arrow to reconcile!");
				return operationResult;
			}
			int num = 0;
			int num2 = 0;
			List<IVPaymentModel> paymentList = IVPaymentRepository.GetPaymentList(ctx, param);
			List<IVReceiveModel> receiveList = IVReceiveRepository.GetReceiveList(ctx, param);
			List<IVTransferModel> transferList = IVTransferRepository.GetTransferList(ctx, param);
			num = paymentList.Count + receiveList.Count + transferList.Count;
			paymentList = (from t in paymentList
			where t.MReconcileStatu == Convert.ToInt32(IVReconcileStatus.None) || t.MReconcileStatu == Convert.ToInt32(IVReconcileStatus.Marked)
			select t).ToList();
			receiveList = (from t in receiveList
			where t.MReconcileStatu == Convert.ToInt32(IVReconcileStatus.None) || t.MReconcileStatu == Convert.ToInt32(IVReconcileStatus.Marked)
			select t).ToList();
			transferList = (from t in transferList
			where (t.MFromAcctID == bankModel.MItemID && (t.MFromReconcileStatu == Convert.ToInt32(IVReconcileStatus.None) || t.MFromReconcileStatu == Convert.ToInt32(IVReconcileStatus.Marked))) || (t.MToAcctID == bankModel.MItemID && (t.MToReconcileStatu == Convert.ToInt32(IVReconcileStatus.None) || t.MToReconcileStatu == Convert.ToInt32(IVReconcileStatus.Marked)))
			select t).ToList();
			num2 = paymentList.Count + receiveList.Count + transferList.Count;
			IVTransactionsRepository.UpdateReconcileStatu(ctx, param.MStatu, paymentList, receiveList, transferList);
			int num3 = num - num2;
			if (num3 > 0)
			{
				string format = "";
				if (param.MStatu == IVReconcileStatus.None)
				{
					format = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "MarkAsUnreconcileResult", "{0} item(s) mark as unreconcile successful!<br/>{1} item(s) mark as unreconcile failure!");
				}
				else if (param.MStatu == IVReconcileStatus.Marked)
				{
					format = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "MarkAsReconcileResult", "{0} item(s) mark as reconcile successful!<br/>{1} item(s) mark as reconcile failure!");
				}
				operationResult.Message = string.Format(format, num2, num3);
				return operationResult;
			}
			operationResult.Success = true;
			return operationResult;
		}

		public OperationResult DeteleTransactions(MContext ctx, List<IVAccountTransactionsModel> list)
		{
			return IVTransactionsRepository.DeteleTransactions(ctx, list);
		}

		public List<IVBankStatementsModel> GetBankStatementsList(MContext ctx, IVTransactionQueryParamModel param)
		{
			return IVTransactionsRepository.GetBankStatementsList(ctx, param);
		}

		public IVBankStatementsModel GetBankStatementModel(MContext ctx, string MID)
		{
			return IVTransactionsRepository.GetBankStatementModel(ctx, MID);
		}

		public List<IVBankStatementViewModel> GetBankStatementView(MContext ctx, string statementID)
		{
			return IVTransactionsRepository.GetBankStatementView(ctx, statementID);
		}

		public DataGridJson<IVBankStatementDetailModel> GetBankStatementDetailList(MContext ctx, IVBankStatementDetailFilter filter)
		{
			DataGridJson<IVBankStatementDetailModel> bankStatementDetailList = IVTransactionsRepository.GetBankStatementDetailList(ctx, filter);
			if (bankStatementDetailList.rows != null && bankStatementDetailList.rows.Count > 0)
			{
				List<string> parentIdList = (from t in bankStatementDetailList.rows
				select t.MEntryID).ToList();
				List<IVBankStatementDetailModel> bankStatementDetailList2 = IVTransactionsRepository.GetBankStatementDetailList(ctx, filter.MBankID, parentIdList);
				if (bankStatementDetailList2 != null && bankStatementDetailList2.Count > 0)
				{
					List<IVBankStatementDetailModel> list = new List<IVBankStatementDetailModel>();
					foreach (IVBankStatementDetailModel row in bankStatementDetailList.rows)
					{
						List<IVBankStatementDetailModel> list2 = (from t in bankStatementDetailList2
						where t.MParentID == row.MEntryID
						orderby t.MSeq
						select t).ToList();
						if (list2.Count > 0)
						{
							row.MIsCanMark = false;
						}
						list.Add(row);
						list.AddRange(list2);
					}
					bankStatementDetailList.rows = list;
				}
			}
			return bankStatementDetailList;
		}

		public int StatementStatusUpdate(MContext ctx, string selectIds, int directType)
		{
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrWhiteSpace(selectIds))
			{
				operationResult.Success = false;
				return 1;
			}
			List<string> idList = selectIds.Replace("'", "").Split(',').ToList();
			operationResult = IVTransactionsRepository.CheckIsCanDeleteStatementStatus(ctx, idList);
			if (operationResult.Success)
			{
				return IVTransactionsRepository.StatementStatusUpdate(ctx, selectIds, directType);
			}
			List<string> messages = new List<string>
			{
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "BankStatementHadReconcile", "存在已经勾对的对账单，无法删除，请刷新页面重试!")
			};
			MActionException ex = new MActionException();
			ex.Messages = messages;
			throw ex;
		}

		public OperationResult StatementUpdate(MContext ctx, List<IVBankStatementViewModel> models)
		{
			return IVTransactionsRepository.StatementUpdate(ctx, models);
		}

		public BDBankAccountModel GetBankAccountModel(MContext ctx, string bankNo)
		{
			return IVTransactionsRepository.GetBankAccountModel(ctx, bankNo);
		}

		public BDBankTypeModel GetBankTypeModel(MContext ctx, string id)
		{
			return IVTransactionsRepository.GetBankTypeModel(ctx, id);
		}

		public List<BankBillImportSolutionModel> GetBankBillImportSolutionList(MContext ctx, string bankTypeId)
		{
			return IVTransactionsRepository.GetBankBillImportSolutionList(ctx, bankTypeId);
		}

		public BankBillImportSolutionModel GetBankBillImportSolutionModel(MContext ctx, string MItemID)
		{
			return IVTransactionsRepository.GetBankBillImportSolutionModel(ctx, MItemID);
		}

		public OperationResult SaveBankBillImportSolution(MContext ctx, BankBillImportSolutionModel model)
		{
			return IVTransactionsRepository.SaveBankBillImportSolution(ctx, model);
		}

		public OperationResult SaveImportBankBill(MContext ctx, IVBankBillModel model, DataTable importData)
		{
			return IVTransactionsRepository.SaveImportBankBill(ctx, model, importData);
		}

		public OperationResult IsCanDeleteStatementStatus(MContext ctx, string ids)
		{
			return new OperationResult();
		}

		public void LogonOnlineBank(MContext ctx, string userid, string password)
		{
			IVBankAPIReqRepository.LogonOnlineBank(ctx, userid, password);
		}

		public OperationResult GetBankFeeds(MContext ctx, IVBankFeedsModel feedModel)
		{
			return IVBankAPIReqRepository.GetBankFeeds(ctx, feedModel);
		}

		public void LogoutOnlineBank(MContext ctx)
		{
			IVBankAPIReqRepository.LogoutOnlineBank(ctx);
		}
	}
}
