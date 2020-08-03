using JieNor.Megi.Common.Converter;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.Core.Utility;
using JieNor.Megi.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVExpenseMakePaymentRepository : IVMakePaymentAbstractRepository
	{
		public IVExpenseMakePaymentRepository(BillSourceType sourceType, CreateFromType creatFrom)
		{
			base._sourceType = sourceType;
			base._createFrom = creatFrom;
		}

		public IVExpenseMakePaymentRepository()
		{
		}

		public override MakePaymentResultModel GetToPayResult(MContext ctx, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel)
		{
			MakePaymentResultModel makePaymentResultModel = new MakePaymentResultModel();
			bool flag = true;
			string mMessage = "";
			List<CommandInfo> list = null;
			string mTargetBillID = "";
			IVExpenseModel iVExpenseModel = makePaymentModel.MObject as IVExpenseModel;
			if (iVExpenseModel == null)
			{
				iVExpenseModel = IVExpenseRepository.GetExpenseModel(ctx, makePaymentModel.MObjectID);
			}
			if (makePaymentModel.MIsPayAll)
			{
				makePaymentModel.MPaidAmount = Math.Abs(iVExpenseModel.MTaxTotalAmtFor - iVExpenseModel.MVerificationAmt);
			}
			CheckPay(ctx, iVExpenseModel, bankModel, makePaymentModel.MPaidAmount, makePaymentModel.MPaidDate, out flag, out mMessage);
			if (!flag)
			{
				makePaymentResultModel.MMessage = mMessage;
				return makePaymentResultModel;
			}
			BDExchangeRateModel enableExchangeRate = BDExchangeRateRepository.GetEnableExchangeRate(ctx, makePaymentModel.MPaidDate, iVExpenseModel.MCyID);
			if (enableExchangeRate == null)
			{
				makePaymentResultModel.MMessage = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "NoEffectiveExchangeRate", "{0} 没有有效汇率", makePaymentModel.MPaidDate.ToOrgZoneDateFormat(null));
				makePaymentResultModel.MSuccess = false;
				return makePaymentResultModel;
			}
			makePaymentModel.MOToLRate = enableExchangeRate.MRate;
			makePaymentModel.MLToORate = enableExchangeRate.MUserRate;
			makePaymentModel.MPaidAmtFor = makePaymentModel.MPaidAmount;
			makePaymentModel.MPaidAmt = (makePaymentModel.MPaidAmount * makePaymentModel.MOToLRate).ToRound(2);
			if (makePaymentModel.MRefFromBill && string.IsNullOrEmpty(makePaymentModel.MRef))
			{
				makePaymentModel.MRef = iVExpenseModel.MReference;
			}
			list = GetPaymentCmd(ctx, iVExpenseModel, makePaymentModel, bankModel, out mTargetBillID);
			makePaymentResultModel.MTargetBillType = "Payment";
			makePaymentResultModel.MTargetBillID = mTargetBillID;
			OperationResult operationResult = ResultHelper.ToOperationResult(iVExpenseModel);
			if (!operationResult.Success)
			{
				makePaymentResultModel.MSuccess = operationResult.Success;
				makePaymentResultModel.MMessage = operationResult.Message;
			}
			makePaymentResultModel.MCommand = list;
			makePaymentResultModel.MSuccess = flag;
			makePaymentResultModel.MMessage = mMessage;
			return makePaymentResultModel;
		}

		public override MakePaymentResultModel GetToPayResults(MContext ctx, List<IVMakePaymentModel> makePaymentModels, BDBankAccountEditModel bankModel)
		{
			MakePaymentResultModel makePaymentResultModel = new MakePaymentResultModel();
			bool flag = true;
			string mMessage = "";
			List<CommandInfo> list = null;
			string mTargetBillID = "";
			List<IVExpenseModel> list2 = new List<IVExpenseModel>();
			List<string> list3 = new List<string>();
			foreach (IVMakePaymentModel makePaymentModel2 in makePaymentModels)
			{
				IVExpenseModel iVExpenseModel = makePaymentModel2.MObject as IVExpenseModel;
				if (iVExpenseModel == null)
				{
					list3.Add(makePaymentModel2.MObjectID);
				}
				else
				{
					list2.Add(iVExpenseModel);
				}
			}
			if (list3.Count > 0)
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.In("MID", (from t in makePaymentModels
				select t.MObjectID).ToArray());
				list2.AddRange(IVExpenseRepository.GetExpenseListIncludeEntry(ctx, sqlWhere));
			}
			foreach (IVMakePaymentModel makePaymentModel3 in makePaymentModels)
			{
				IVExpenseModel iVExpenseModel2 = (from t in list2
				where t.MID == makePaymentModel3.MObjectID
				select t).FirstOrDefault();
				if (iVExpenseModel2 != null)
				{
					if (makePaymentModel3.MIsPayAll)
					{
						makePaymentModel3.MPaidAmount = Math.Abs(iVExpenseModel2.MTaxTotalAmtFor - iVExpenseModel2.MVerificationAmt);
					}
					CheckPay(ctx, iVExpenseModel2, bankModel, makePaymentModel3.MPaidAmount, makePaymentModel3.MPaidDate, out flag, out mMessage);
					if (!flag)
					{
						makePaymentResultModel.MMessage = mMessage;
						return makePaymentResultModel;
					}
					BDExchangeRateModel enableExchangeRate = BDExchangeRateRepository.GetEnableExchangeRate(ctx, makePaymentModel3.MPaidDate, iVExpenseModel2.MCyID);
					if (enableExchangeRate == null)
					{
						makePaymentResultModel.MMessage = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "NoEffectiveExchangeRate", "{0} 没有有效汇率", makePaymentModel3.MPaidDate.ToOrgZoneDateFormat(null));
						makePaymentResultModel.MSuccess = false;
						return makePaymentResultModel;
					}
					makePaymentModel3.MOToLRate = enableExchangeRate.MRate;
					makePaymentModel3.MLToORate = enableExchangeRate.MUserRate;
					makePaymentModel3.MPaidAmtFor = makePaymentModel3.MPaidAmount;
					makePaymentModel3.MPaidAmt = (makePaymentModel3.MPaidAmount * makePaymentModel3.MOToLRate).ToRound(2);
					if (makePaymentModel3.MRefFromBill && string.IsNullOrEmpty(makePaymentModel3.MRef))
					{
						makePaymentModel3.MRef = iVExpenseModel2.MReference;
					}
				}
			}
			list = GetPaymentCmds(ctx, list2, makePaymentModels, bankModel);
			makePaymentResultModel.MTargetBillType = "Payment";
			makePaymentResultModel.MTargetBillID = mTargetBillID;
			foreach (IVExpenseModel item in list2)
			{
				OperationResult operationResult = ResultHelper.ToOperationResult(item);
				if (!operationResult.Success)
				{
					makePaymentResultModel.MSuccess = operationResult.Success;
					makePaymentResultModel.MMessage = operationResult.Message;
					return makePaymentResultModel;
				}
			}
			makePaymentResultModel.MCommand = list;
			makePaymentResultModel.MSuccess = flag;
			makePaymentResultModel.MMessage = mMessage;
			return makePaymentResultModel;
		}

		private void CheckPay(MContext ctx, IVExpenseModel expenseModel, BDBankAccountEditModel bankModel, decimal toPaidAmtFor, DateTime toPaidDate, out bool success, out string message)
		{
			if (toPaidDate.Date < ctx.MBeginDate.Date)
			{
				success = false;
				message = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "DateAfter", "The date should be later {0} or on this date.", ctx.MBeginDate.ToOrgZoneDateFormat(null));
			}
			else
			{
				base.CheckBank(ctx, bankModel, out success, out message);
				if (success)
				{
					IVMakePaymentAbstractRepository.CheckCurrency(ctx, bankModel.MCyID, expenseModel.MCyID, out success, out message, expenseModel.MRowIndex);
					if (success)
					{
						if (expenseModel == null || string.IsNullOrEmpty(expenseModel.MID))
						{
							success = false;
							message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TheExpenseIsNotExists", "The Expense is not exists!");
						}
						else
						{
							OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, toPaidDate);
							if (!operationResult.Success)
							{
								success = false;
								message = operationResult.Message;
							}
							else
							{
								base.CheckPaidAccount(ctx, expenseModel.MTaxTotalAmtFor, expenseModel.MVerificationAmt, toPaidAmtFor, out success, out message, false);
							}
						}
					}
				}
			}
		}

		private List<CommandInfo> GetPaymentCmd(MContext ctx, IVExpenseModel expenseModel, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel, out string paymentId)
		{
			IVPaymentModel paymentModel = GetPaymentModel(ctx, expenseModel, makePaymentModel, bankModel);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, paymentModel, null, true);
			paymentId = paymentModel.MID;
			insertOrUpdateCmd.AddRange(IVExpenseLogHelper.GetPaidLogCmd(ctx, expenseModel, makePaymentModel.MPaidDate, makePaymentModel.MPaidAmtFor));
			insertOrUpdateCmd.AddRange(GetUpdateExpenseVerificationCmd(ctx, expenseModel, makePaymentModel));
			IVVerificationModel verificationModel = base.GetVerificationModel(ctx, paymentModel.MID, "Payment", expenseModel.MID, "Expense", makePaymentModel.MPaidAmtFor, makePaymentModel.MPaidAmt);
			insertOrUpdateCmd.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, verificationModel, null, true));
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, paymentModel, null);
			if (operationResult.Success)
			{
				insertOrUpdateCmd.AddRange(operationResult.OperationCommands);
			}
			else
			{
				expenseModel.ValidationErrors = paymentModel.ValidationErrors;
			}
			return insertOrUpdateCmd;
		}

		private List<CommandInfo> GetPaymentCmds(MContext ctx, List<IVExpenseModel> expenseModels, List<IVMakePaymentModel> makePaymentModels, BDBankAccountEditModel bankModel)
		{
			List<IVPaymentModel> list = new List<IVPaymentModel>();
			List<IVVerificationModel> list2 = new List<IVVerificationModel>();
			List<CommandInfo> list3 = new List<CommandInfo>();
			ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, new IVPaymentModel(), null, true);
			foreach (IVMakePaymentModel makePaymentModel in makePaymentModels)
			{
				IVExpenseModel iVExpenseModel = (from t in expenseModels
				where t.MID == makePaymentModel.MObjectID
				select t).FirstOrDefault();
				if (iVExpenseModel != null)
				{
					IVPaymentModel paymentModel = GetPaymentModel(ctx, iVExpenseModel, makePaymentModel, bankModel);
					list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, paymentModel, null, true));
					list3.AddRange(IVExpenseLogHelper.GetPaidLogCmd(ctx, iVExpenseModel, makePaymentModel.MPaidDate, makePaymentModel.MPaidAmtFor));
					list3.AddRange(GetUpdateExpenseVerificationCmd(ctx, iVExpenseModel, makePaymentModel));
					IVVerificationModel verificationModel = base.GetVerificationModel(ctx, paymentModel.MID, "Payment", iVExpenseModel.MID, "Expense", makePaymentModel.MPaidAmtFor, makePaymentModel.MPaidAmt);
					list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, verificationModel, null, true));
					list.Add(paymentModel);
					list2.Add(verificationModel);
				}
			}
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBills(ctx, list, null);
			if (operationResult.Success)
			{
				list3.AddRange(operationResult.OperationCommands);
			}
			else
			{
				foreach (IVExpenseModel expenseModel in expenseModels)
				{
					IVVerificationModel vriModel = (from t in list2
					where t.MTargetBillID == expenseModel.MID
					select t).FirstOrDefault();
					if (vriModel != null)
					{
						IVPaymentModel iVPaymentModel = (from t in list
						where t.MID == vriModel.MSourceBillID
						select t).FirstOrDefault();
						if (iVPaymentModel != null)
						{
							expenseModel.ValidationErrors = iVPaymentModel.ValidationErrors;
						}
					}
				}
			}
			return list3;
		}

		private IVPaymentModel GetPaymentModel(MContext ctx, IVExpenseModel expenseModel, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel)
		{
			string paymentBizType = base.GetPaymentBizType("Expense_Claims");
			decimal mPaidAmtFor = makePaymentModel.MPaidAmtFor;
			string text = string.IsNullOrEmpty(expenseModel.MReference) ? "Expense Claims" : expenseModel.MReference;
			IVPaymentModel iVPaymentModel = new IVPaymentModel();
			iVPaymentModel.MBankID = bankModel.MItemID;
			iVPaymentModel.MCyID = bankModel.MCyID;
			iVPaymentModel.MContactType = "Employees";
			iVPaymentModel.MDepartment = expenseModel.MDepartment;
			iVPaymentModel.MContactID = expenseModel.MEmployee;
			iVPaymentModel.MReference = makePaymentModel.MRef;
			iVPaymentModel.MDesc = $"Payment:{text}";
			iVPaymentModel.MBizDate = makePaymentModel.MPaidDate;
			iVPaymentModel.MType = paymentBizType;
			iVPaymentModel.MTaxID = "Tax_Inclusive";
			iVPaymentModel.MExchangeRate = makePaymentModel.MOToLRate;
			iVPaymentModel.MOToLRate = makePaymentModel.MOToLRate;
			iVPaymentModel.MLToORate = makePaymentModel.MLToORate;
			iVPaymentModel.MOrgID = ctx.MOrgID;
			iVPaymentModel.MVerificationAmt = mPaidAmtFor;
			iVPaymentModel.MVerifyAmtFor = mPaidAmtFor;
			iVPaymentModel.MSource = Convert.ToInt32(base._sourceType);
			iVPaymentModel.MCreateFrom = Convert.ToInt32(base._createFrom);
			iVPaymentModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
			List<IVPaymentEntryModel> list = new List<IVPaymentEntryModel>();
			if (makePaymentModel.MPaidAmtFor == Math.Abs(expenseModel.MTaxTotalAmtFor))
			{
				foreach (IVExpenseEntryModel item in expenseModel.ExpenseEntry)
				{
					IVPaymentEntryModel iVPaymentEntryModel = new IVPaymentEntryModel();
					iVPaymentEntryModel.MTaxTypeID = iVPaymentModel.MTaxID;
					iVPaymentEntryModel.MID = iVPaymentModel.MID;
					iVPaymentEntryModel.MQty = item.MQty;
					iVPaymentEntryModel.MPrice = item.MPrice;
					iVPaymentEntryModel.MDiscount = item.MDiscount;
					iVPaymentEntryModel.MItemID = item.MItemID;
					iVPaymentEntryModel.MDesc = item.MDesc;
					iVPaymentEntryModel.MTaxID = item.MTaxID;
					iVPaymentEntryModel.MTaxAmtFor = item.MTaxAmtFor;
					iVPaymentEntryModel.MSeq = item.MSeq;
					iVPaymentEntryModel.MTrackItem1 = item.MTrackItem1;
					iVPaymentEntryModel.MTrackItem2 = item.MTrackItem2;
					iVPaymentEntryModel.MTrackItem3 = item.MTrackItem3;
					iVPaymentEntryModel.MTrackItem4 = item.MTrackItem4;
					iVPaymentEntryModel.MTrackItem5 = item.MTrackItem5;
					iVPaymentEntryModel.MDebitAccount = item.MCreditAccount;
					list.Add(iVPaymentEntryModel);
				}
			}
			else
			{
				IVPaymentEntryModel iVPaymentEntryModel2 = new IVPaymentEntryModel();
				iVPaymentEntryModel2.MID = iVPaymentModel.MID;
				iVPaymentEntryModel2.MQty = decimal.One;
				iVPaymentEntryModel2.MPrice = mPaidAmtFor;
				iVPaymentEntryModel2.MDesc = text;
				iVPaymentEntryModel2.MTaxAmtFor = decimal.Zero;
				if (expenseModel.ExpenseEntry.Count == 1)
				{
					iVPaymentEntryModel2.MItemID = expenseModel.ExpenseEntry[0].MItemID;
					iVPaymentEntryModel2.MTrackItem1 = expenseModel.ExpenseEntry[0].MTrackItem1;
					iVPaymentEntryModel2.MTrackItem2 = expenseModel.ExpenseEntry[0].MTrackItem2;
					iVPaymentEntryModel2.MTrackItem3 = expenseModel.ExpenseEntry[0].MTrackItem3;
					iVPaymentEntryModel2.MTrackItem4 = expenseModel.ExpenseEntry[0].MTrackItem4;
					iVPaymentEntryModel2.MTrackItem5 = expenseModel.ExpenseEntry[0].MTrackItem5;
				}
				iVPaymentEntryModel2.MDebitAccount = expenseModel.ExpenseEntry[0].MCreditAccount;
				list.Add(iVPaymentEntryModel2);
			}
			iVPaymentModel.PaymentEntry = list;
			return iVPaymentModel;
		}

		private List<CommandInfo> GetUpdateExpenseVerificationCmd(MContext ctx, IVExpenseModel expenseModel, IVMakePaymentModel makePaymentModel)
		{
			expenseModel.MVerificationAmt += makePaymentModel.MPaidAmtFor;
			expenseModel.MVerifyAmtFor += makePaymentModel.MPaidAmtFor;
			if (expenseModel.MVerifyAmtFor == expenseModel.MTaxTotalAmtFor)
			{
				expenseModel.MStatus = Convert.ToInt32(IVExpenseStatusEnum.Paid);
			}
			else
			{
				expenseModel.MStatus = Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment);
			}
			if (expenseModel.IsNew)
			{
				expenseModel.IsNew = false;
				foreach (IVExpenseEntryModel item in expenseModel.ExpenseEntry)
				{
					if (item.IsNew)
					{
						item.IsNew = false;
					}
				}
			}
			expenseModel.MExchangeRate = makePaymentModel.MOToLRate;
			return ModelInfoManager.GetInsertOrUpdateCmd<IVExpenseModel>(ctx, expenseModel, new List<string>
			{
				"MVerificationAmt",
				"MVerifyAmtFor",
				"MVerifyAmt",
				"MStatus"
			}, false);
		}
	}
}
