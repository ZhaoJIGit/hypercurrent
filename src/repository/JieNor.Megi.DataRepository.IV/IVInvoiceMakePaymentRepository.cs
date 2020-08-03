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
	public class IVInvoiceMakePaymentRepository : IVMakePaymentAbstractRepository
	{
		public IVInvoiceMakePaymentRepository(BillSourceType sourceType, CreateFromType creatFrom)
		{
			base._sourceType = sourceType;
			base._createFrom = creatFrom;
		}

		public IVInvoiceMakePaymentRepository()
		{
		}

		public override MakePaymentResultModel GetToPayResult(MContext ctx, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel)
		{
			MakePaymentResultModel makePaymentResultModel = new MakePaymentResultModel();
			bool flag = true;
			string mMessage = "";
			List<CommandInfo> list = null;
			string mTargetBillID = "";
			IVInvoiceModel invoiceEditModel = IVInvoiceRepository.GetInvoiceEditModel(ctx, makePaymentModel.MObjectID);
			if (makePaymentModel.MIsPayAll)
			{
				makePaymentModel.MPaidAmount = Math.Abs(invoiceEditModel.MTaxTotalAmtFor - invoiceEditModel.MVerificationAmt);
			}
			OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, makePaymentModel.MPaidDate);
			if (!operationResult.Success)
			{
				makePaymentResultModel.MMessage = operationResult.Message;
				return makePaymentResultModel;
			}
			CheckPay(ctx, invoiceEditModel, bankModel, makePaymentModel.MPaidAmount, makePaymentModel.MPaidDate, out flag, out mMessage);
			if (!flag)
			{
				makePaymentResultModel.MMessage = mMessage;
				return makePaymentResultModel;
			}
			BDExchangeRateModel enableExchangeRate = BDExchangeRateRepository.GetEnableExchangeRate(ctx, makePaymentModel.MPaidDate, invoiceEditModel.MCyID);
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
				makePaymentModel.MRef = invoiceEditModel.MReference;
			}
			OperationResult operationResult2;
			switch (invoiceEditModel.MType)
			{
			case "Invoice_Sale":
				list = GetReceiveCmd(ctx, invoiceEditModel, makePaymentModel, bankModel, "Customer", out mTargetBillID);
				makePaymentResultModel.MTargetBillType = "Receive";
				makePaymentResultModel.MTargetBillID = mTargetBillID;
				goto IL_028d;
			case "Invoice_Sale_Red":
				list = GetPaymentCmd(ctx, invoiceEditModel, makePaymentModel, bankModel, "Customer", out mTargetBillID);
				makePaymentResultModel.MTargetBillType = "Payment";
				makePaymentResultModel.MTargetBillID = mTargetBillID;
				goto IL_028d;
			case "Invoice_Purchase":
				list = GetPaymentCmd(ctx, invoiceEditModel, makePaymentModel, bankModel, "Supplier", out mTargetBillID);
				makePaymentResultModel.MTargetBillType = "Payment";
				makePaymentResultModel.MTargetBillID = mTargetBillID;
				goto IL_028d;
			case "Invoice_Purchase_Red":
				list = GetReceiveCmd(ctx, invoiceEditModel, makePaymentModel, bankModel, "Supplier", out mTargetBillID);
				makePaymentResultModel.MTargetBillType = "Receive";
				makePaymentResultModel.MTargetBillID = mTargetBillID;
				goto IL_028d;
			default:
				{
					return null;
				}
				IL_028d:
				operationResult2 = ResultHelper.ToOperationResult(invoiceEditModel);
				if (!operationResult2.Success)
				{
					makePaymentResultModel.MSuccess = operationResult2.Success;
					makePaymentResultModel.MMessage = operationResult2.Message;
					return makePaymentResultModel;
				}
				makePaymentResultModel.MCommand = list;
				makePaymentResultModel.MSuccess = flag;
				makePaymentResultModel.MMessage = mMessage;
				return makePaymentResultModel;
			}
		}

		public override MakePaymentResultModel GetToPayResults(MContext ctx, List<IVMakePaymentModel> makePaymentModels, BDBankAccountEditModel bankModel)
		{
			MakePaymentResultModel makePaymentResultModel = new MakePaymentResultModel();
			bool flag = true;
			string mMessage = "";
			List<CommandInfo> list = new List<CommandInfo>();
			string mTargetBillID = "";
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.In("MID", (from t in makePaymentModels
			select t.MObjectID).ToArray());
			List<IVInvoiceModel> invoiceListIncludeEntry = IVInvoiceRepository.GetInvoiceListIncludeEntry(ctx, sqlWhere);
			foreach (IVMakePaymentModel makePaymentModel in makePaymentModels)
			{
				IVInvoiceModel iVInvoiceModel = (from t in invoiceListIncludeEntry
				where t.MID == makePaymentModel.MObjectID
				select t).FirstOrDefault();
				if (iVInvoiceModel != null && makePaymentModel.MIsPayAll)
				{
					makePaymentModel.MPaidAmount = Math.Abs(iVInvoiceModel.MTaxTotalAmtFor - iVInvoiceModel.MVerificationAmt);
				}
				OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, makePaymentModel.MPaidDate);
				if (!operationResult.Success)
				{
					makePaymentResultModel.MMessage = operationResult.Message;
					return makePaymentResultModel;
				}
				CheckPay(ctx, iVInvoiceModel, bankModel, makePaymentModel.MPaidAmount, makePaymentModel.MPaidDate, out flag, out mMessage);
				if (!flag)
				{
					makePaymentResultModel.MMessage = mMessage;
					return makePaymentResultModel;
				}
				BDExchangeRateModel enableExchangeRate = BDExchangeRateRepository.GetEnableExchangeRate(ctx, makePaymentModel.MPaidDate, iVInvoiceModel.MCyID);
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
					makePaymentModel.MRef = iVInvoiceModel.MReference;
				}
			}
			List<IVInvoiceModel> list2 = (from t in invoiceListIncludeEntry
			where t.MType == "Invoice_Sale_Red" || t.MType == "Invoice_Purchase_Red"
			select t).ToList();
			if (list2 != null && list2.Count > 0)
			{
				string mType = list2[0].MType;
				if (!(mType == "Invoice_Sale_Red"))
				{
					if (mType == "Invoice_Purchase_Red")
					{
						list.AddRange(GetReceiveCmds(ctx, list2, makePaymentModels, bankModel, "Supplier"));
						makePaymentResultModel.MTargetBillType = "Receive";
						makePaymentResultModel.MTargetBillID = mTargetBillID;
						goto IL_0376;
					}
					return null;
				}
				list.AddRange(GetPaymentCmds(ctx, list2, makePaymentModels, bankModel, "Customer"));
				makePaymentResultModel.MTargetBillType = "Payment";
				makePaymentResultModel.MTargetBillID = mTargetBillID;
			}
			goto IL_0376;
			IL_0376:
			List<IVInvoiceModel> list3 = (from t in invoiceListIncludeEntry
			where t.MType == "Invoice_Sale" || t.MType == "Invoice_Purchase"
			select t).ToList();
			if (list3 != null && list3.Count > 0)
			{
				string mType2 = list3[0].MType;
				if (!(mType2 == "Invoice_Sale"))
				{
					if (mType2 == "Invoice_Purchase")
					{
						list.AddRange(GetPaymentCmds(ctx, list3, makePaymentModels, bankModel, "Supplier"));
						makePaymentResultModel.MTargetBillType = "Payment";
						makePaymentResultModel.MTargetBillID = mTargetBillID;
						goto IL_0450;
					}
					return null;
				}
				list.AddRange(GetReceiveCmds(ctx, list3, makePaymentModels, bankModel, "Customer"));
				makePaymentResultModel.MTargetBillType = "Receive";
				makePaymentResultModel.MTargetBillID = mTargetBillID;
			}
			goto IL_0450;
			IL_0450:
			foreach (IVInvoiceModel item in invoiceListIncludeEntry)
			{
				OperationResult operationResult2 = ResultHelper.ToOperationResult(item);
				if (!operationResult2.Success)
				{
					makePaymentResultModel.MSuccess = operationResult2.Success;
					makePaymentResultModel.MMessage = operationResult2.Message;
					return makePaymentResultModel;
				}
			}
			makePaymentResultModel.MCommand = list;
			makePaymentResultModel.MSuccess = flag;
			makePaymentResultModel.MMessage = mMessage;
			return makePaymentResultModel;
		}

		private void CheckPay(MContext ctx, IVInvoiceModel ivModel, BDBankAccountEditModel bankModel, decimal toPaidAmtFor, DateTime toPaidDate, out bool success, out string message)
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
					IVMakePaymentAbstractRepository.CheckCurrency(ctx, bankModel.MCyID, ivModel.MCyID, out success, out message, 0);
					if (success)
					{
						if (ivModel == null || string.IsNullOrEmpty(ivModel.MID))
						{
							success = false;
							message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TheInvoiceIsNotExists", "The Invoice is not exists!");
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
								base.CheckPaidAccount(ctx, ivModel.MTaxTotalAmtFor, ivModel.MVerificationAmt, toPaidAmtFor, out success, out message, false);
							}
						}
					}
				}
			}
		}

		private List<CommandInfo> GetPaymentCmd(MContext ctx, IVInvoiceModel ivModel, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel, string contactType, out string paymentId)
		{
			IVPaymentModel paymentModel = GetPaymentModel(ctx, ivModel, makePaymentModel, bankModel, contactType);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, paymentModel, null, true);
			paymentId = paymentModel.MID;
			insertOrUpdateCmd.AddRange(IVInvoiceLogHelper.GetPaidLogCmd(ctx, ivModel, makePaymentModel.MPaidDate, makePaymentModel.MPaidAmount));
			insertOrUpdateCmd.AddRange(GetUpdateInvoiceVerificationCmd(ctx, ivModel, makePaymentModel));
			IVVerificationModel verificationModel = base.GetVerificationModel(ctx, paymentModel.MID, "Payment", ivModel.MID, "Invoice", makePaymentModel.MPaidAmtFor, makePaymentModel.MPaidAmt);
			insertOrUpdateCmd.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, verificationModel, null, true));
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, paymentModel, null);
			if (operationResult.Success)
			{
				insertOrUpdateCmd.AddRange(operationResult.OperationCommands);
			}
			else
			{
				ivModel.ValidationErrors = (ivModel.ValidationErrors ?? new List<ValidationError>());
				ivModel.ValidationErrors.AddRange(paymentModel.ValidationErrors);
			}
			return insertOrUpdateCmd;
		}

		private List<CommandInfo> GetPaymentCmds(MContext ctx, List<IVInvoiceModel> ivModels, List<IVMakePaymentModel> makePaymentModels, BDBankAccountEditModel bankModel, string contactType)
		{
			List<IVPaymentModel> list = new List<IVPaymentModel>();
			List<IVVerificationModel> list2 = new List<IVVerificationModel>();
			List<CommandInfo> list3 = new List<CommandInfo>();
			foreach (IVMakePaymentModel makePaymentModel in makePaymentModels)
			{
				IVInvoiceModel iVInvoiceModel = (from t in ivModels
				where t.MID == makePaymentModel.MObjectID
				select t).FirstOrDefault();
				if (iVInvoiceModel != null)
				{
					IVPaymentModel paymentModel = GetPaymentModel(ctx, iVInvoiceModel, makePaymentModel, bankModel, contactType);
					list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, paymentModel, null, true));
					list3.AddRange(IVInvoiceLogHelper.GetPaidLogCmd(ctx, iVInvoiceModel, makePaymentModel.MPaidDate, makePaymentModel.MPaidAmount));
					list3.AddRange(GetUpdateInvoiceVerificationCmd(ctx, iVInvoiceModel, makePaymentModel));
					IVVerificationModel verificationModel = base.GetVerificationModel(ctx, paymentModel.MID, "Payment", iVInvoiceModel.MID, "Invoice", makePaymentModel.MPaidAmtFor, makePaymentModel.MPaidAmt);
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
				foreach (IVInvoiceModel ivModel in ivModels)
				{
					IVVerificationModel vriModel = (from t in list2
					where t.MTargetBillID == ivModel.MID
					select t).FirstOrDefault();
					if (vriModel != null)
					{
						IVPaymentModel iVPaymentModel = (from t in list
						where t.MID == vriModel.MSourceBillID
						select t).FirstOrDefault();
						if (iVPaymentModel != null)
						{
							ivModel.ValidationErrors = (ivModel.ValidationErrors ?? new List<ValidationError>());
							ivModel.ValidationErrors.AddRange(iVPaymentModel.ValidationErrors);
						}
					}
				}
			}
			return list3;
		}

		private IVPaymentModel GetPaymentModel(MContext ctx, IVInvoiceModel ivModel, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel, string contactType)
		{
			string paymentBizType = base.GetPaymentBizType(ivModel.MType);
			decimal mPaidAmtFor = makePaymentModel.MPaidAmtFor;
			string mDesc = $"Payment:{ivModel.MReference}";
			IVPaymentModel iVPaymentModel = new IVPaymentModel();
			iVPaymentModel.MBankID = bankModel.MItemID;
			iVPaymentModel.MCyID = bankModel.MCyID;
			iVPaymentModel.MContactType = contactType;
			iVPaymentModel.MContactID = ivModel.MContactID;
			iVPaymentModel.MReference = makePaymentModel.MRef;
			iVPaymentModel.MDesc = mDesc;
			iVPaymentModel.MBizDate = makePaymentModel.MPaidDate;
			iVPaymentModel.MType = paymentBizType;
			iVPaymentModel.MTaxID = ivModel.MTaxID;
			iVPaymentModel.MExchangeRate = makePaymentModel.MOToLRate;
			iVPaymentModel.MOToLRate = makePaymentModel.MOToLRate;
			iVPaymentModel.MLToORate = makePaymentModel.MLToORate;
			iVPaymentModel.MOrgID = ctx.MOrgID;
			iVPaymentModel.MSource = Convert.ToInt32(base._sourceType);
			iVPaymentModel.MCreateFrom = Convert.ToInt32(base._createFrom);
			iVPaymentModel.MVerificationAmt = mPaidAmtFor;
			iVPaymentModel.MVerifyAmtFor = mPaidAmtFor;
			iVPaymentModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
			List<IVPaymentEntryModel> list = new List<IVPaymentEntryModel>();
			if (makePaymentModel.MPaidAmtFor == Math.Abs(ivModel.MTaxTotalAmtFor))
			{
				foreach (IVInvoiceEntryModel item in ivModel.InvoiceEntry)
				{
					IVPaymentEntryModel iVPaymentEntryModel = new IVPaymentEntryModel();
					iVPaymentEntryModel.MTaxTypeID = iVPaymentModel.MTaxID;
					iVPaymentEntryModel.MID = iVPaymentModel.MID;
					iVPaymentEntryModel.MQty = Math.Abs(item.MQty);
					iVPaymentEntryModel.MPrice = Math.Abs(item.MPrice);
					iVPaymentEntryModel.MDiscount = item.MDiscount;
					iVPaymentEntryModel.MItemID = item.MItemID;
					iVPaymentEntryModel.MDesc = item.MDesc;
					iVPaymentEntryModel.MTaxID = item.MTaxID;
					iVPaymentEntryModel.MTaxAmtFor = Math.Abs(item.MTaxAmtFor);
					iVPaymentEntryModel.MSeq = item.MSeq;
					iVPaymentEntryModel.MTrackItem1 = item.MTrackItem1;
					iVPaymentEntryModel.MTrackItem2 = item.MTrackItem2;
					iVPaymentEntryModel.MTrackItem3 = item.MTrackItem3;
					iVPaymentEntryModel.MTrackItem4 = item.MTrackItem4;
					iVPaymentEntryModel.MTrackItem5 = item.MTrackItem5;
					list.Add(iVPaymentEntryModel);
				}
			}
			else
			{
				IVPaymentEntryModel iVPaymentEntryModel2 = new IVPaymentEntryModel();
				iVPaymentEntryModel2.MID = iVPaymentModel.MID;
				iVPaymentEntryModel2.MQty = decimal.One;
				iVPaymentEntryModel2.MDesc = ivModel.MReference;
				iVPaymentEntryModel2.MTaxAmtFor = decimal.Zero;
				iVPaymentEntryModel2.MPrice = mPaidAmtFor;
				if (ivModel.InvoiceEntry.Count == 1)
				{
					iVPaymentEntryModel2.MItemID = ivModel.InvoiceEntry[0].MItemID;
					iVPaymentEntryModel2.MTrackItem1 = ivModel.InvoiceEntry[0].MTrackItem1;
					iVPaymentEntryModel2.MTrackItem2 = ivModel.InvoiceEntry[0].MTrackItem2;
					iVPaymentEntryModel2.MTrackItem3 = ivModel.InvoiceEntry[0].MTrackItem3;
					iVPaymentEntryModel2.MTrackItem4 = ivModel.InvoiceEntry[0].MTrackItem4;
					iVPaymentEntryModel2.MTrackItem5 = ivModel.InvoiceEntry[0].MTrackItem5;
				}
				list.Add(iVPaymentEntryModel2);
			}
			iVPaymentModel.PaymentEntry = list;
			return iVPaymentModel;
		}

		private List<CommandInfo> GetReceiveCmd(MContext ctx, IVInvoiceModel ivModel, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel, string contactType, out string receiveId)
		{
			IVReceiveModel receiveModel = GetReceiveModel(ctx, ivModel, makePaymentModel, bankModel, contactType);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVReceiveModel>(ctx, receiveModel, null, true);
			receiveId = receiveModel.MID;
			insertOrUpdateCmd.AddRange(IVInvoiceLogHelper.GetPaidLogCmd(ctx, ivModel, makePaymentModel.MPaidDate, makePaymentModel.MPaidAmount));
			insertOrUpdateCmd.AddRange(GetUpdateInvoiceVerificationCmd(ctx, ivModel, makePaymentModel));
			IVVerificationModel verificationModel = base.GetVerificationModel(ctx, receiveModel.MID, "Receive", ivModel.MID, "Invoice", makePaymentModel.MPaidAmtFor, makePaymentModel.MPaidAmt);
			insertOrUpdateCmd.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, verificationModel, null, true));
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, receiveModel, null);
			if (operationResult.Success)
			{
				insertOrUpdateCmd.AddRange(operationResult.OperationCommands);
			}
			else
			{
				ivModel.ValidationErrors = (ivModel.ValidationErrors ?? new List<ValidationError>());
				ivModel.ValidationErrors.AddRange(receiveModel.ValidationErrors);
			}
			return insertOrUpdateCmd;
		}

		private List<CommandInfo> GetReceiveCmds(MContext ctx, List<IVInvoiceModel> ivModels, List<IVMakePaymentModel> makePaymentModels, BDBankAccountEditModel bankModel, string contactType)
		{
			List<IVReceiveModel> list = new List<IVReceiveModel>();
			List<IVVerificationModel> list2 = new List<IVVerificationModel>();
			List<CommandInfo> list3 = new List<CommandInfo>();
			foreach (IVMakePaymentModel makePaymentModel in makePaymentModels)
			{
				IVInvoiceModel iVInvoiceModel = (from t in ivModels
				where t.MID == makePaymentModel.MObjectID
				select t).FirstOrDefault();
				if (iVInvoiceModel != null)
				{
					IVReceiveModel receiveModel = GetReceiveModel(ctx, iVInvoiceModel, makePaymentModel, bankModel, contactType);
					list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVReceiveModel>(ctx, receiveModel, null, true));
					list3.AddRange(IVInvoiceLogHelper.GetPaidLogCmd(ctx, iVInvoiceModel, makePaymentModel.MPaidDate, makePaymentModel.MPaidAmount));
					list3.AddRange(GetUpdateInvoiceVerificationCmd(ctx, iVInvoiceModel, makePaymentModel));
					IVVerificationModel verificationModel = base.GetVerificationModel(ctx, receiveModel.MID, "Receive", iVInvoiceModel.MID, "Invoice", makePaymentModel.MPaidAmtFor, makePaymentModel.MPaidAmt);
					list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, verificationModel, null, true));
					list.Add(receiveModel);
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
				foreach (IVInvoiceModel ivModel in ivModels)
				{
					IVVerificationModel vriModel = (from t in list2
					where t.MTargetBillID == ivModel.MID
					select t).FirstOrDefault();
					if (vriModel != null)
					{
						IVReceiveModel iVReceiveModel = (from t in list
						where t.MID == vriModel.MSourceBillID
						select t).FirstOrDefault();
						if (iVReceiveModel != null)
						{
							ivModel.ValidationErrors = (ivModel.ValidationErrors ?? new List<ValidationError>());
							ivModel.ValidationErrors.AddRange(iVReceiveModel.ValidationErrors);
						}
					}
				}
			}
			return list3;
		}

		private IVReceiveModel GetReceiveModel(MContext ctx, IVInvoiceModel ivModel, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel, string contactType)
		{
			string paymentBizType = base.GetPaymentBizType(ivModel.MType);
			decimal mPaidAmtFor = makePaymentModel.MPaidAmtFor;
			string mDesc = $"Receive:{ivModel.MReference}";
			IVReceiveModel iVReceiveModel = new IVReceiveModel();
			iVReceiveModel.MBankID = bankModel.MItemID;
			iVReceiveModel.MCyID = bankModel.MCyID;
			iVReceiveModel.MContactType = contactType;
			iVReceiveModel.MContactID = ivModel.MContactID;
			iVReceiveModel.MReference = makePaymentModel.MRef;
			iVReceiveModel.MDesc = mDesc;
			iVReceiveModel.MBizDate = makePaymentModel.MPaidDate;
			iVReceiveModel.MType = paymentBizType;
			iVReceiveModel.MTaxID = ivModel.MTaxID;
			iVReceiveModel.MExchangeRate = makePaymentModel.MOToLRate;
			iVReceiveModel.MOToLRate = makePaymentModel.MOToLRate;
			iVReceiveModel.MLToORate = makePaymentModel.MLToORate;
			iVReceiveModel.MOrgID = ctx.MOrgID;
			iVReceiveModel.MSource = Convert.ToInt32(base._sourceType);
			iVReceiveModel.MCreateFrom = Convert.ToInt32(base._createFrom);
			iVReceiveModel.MVerificationAmt = mPaidAmtFor;
			iVReceiveModel.MVerifyAmtFor = mPaidAmtFor;
			iVReceiveModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
			List<IVReceiveEntryModel> list = new List<IVReceiveEntryModel>();
			if (makePaymentModel.MPaidAmtFor == Math.Abs(ivModel.MTaxTotalAmtFor))
			{
				foreach (IVInvoiceEntryModel item in ivModel.InvoiceEntry)
				{
					IVReceiveEntryModel iVReceiveEntryModel = new IVReceiveEntryModel();
					iVReceiveEntryModel.MID = iVReceiveModel.MID;
					iVReceiveEntryModel.MQty = Math.Abs(item.MQty);
					iVReceiveEntryModel.MPrice = Math.Abs(item.MPrice);
					iVReceiveEntryModel.MDiscount = item.MDiscount;
					iVReceiveEntryModel.MItemID = item.MItemID;
					iVReceiveEntryModel.MDesc = item.MDesc;
					iVReceiveEntryModel.MTaxID = item.MTaxID;
					iVReceiveEntryModel.MTaxAmtFor = Math.Abs(item.MTaxAmtFor);
					iVReceiveEntryModel.MSeq = item.MSeq;
					iVReceiveEntryModel.MTrackItem1 = item.MTrackItem1;
					iVReceiveEntryModel.MTrackItem2 = item.MTrackItem2;
					iVReceiveEntryModel.MTrackItem3 = item.MTrackItem3;
					iVReceiveEntryModel.MTrackItem4 = item.MTrackItem4;
					iVReceiveEntryModel.MTrackItem5 = item.MTrackItem5;
					list.Add(iVReceiveEntryModel);
				}
			}
			else
			{
				IVReceiveEntryModel iVReceiveEntryModel2 = new IVReceiveEntryModel();
				iVReceiveEntryModel2.MID = iVReceiveModel.MID;
				iVReceiveEntryModel2.MQty = decimal.One;
				iVReceiveEntryModel2.MPrice = mPaidAmtFor;
				iVReceiveEntryModel2.MDesc = ivModel.MReference;
				iVReceiveEntryModel2.MTaxAmt = decimal.Zero;
				iVReceiveEntryModel2.MTaxAmtFor = decimal.Zero;
				if (ivModel.InvoiceEntry.Count == 1)
				{
					iVReceiveEntryModel2.MItemID = ivModel.InvoiceEntry[0].MItemID;
					iVReceiveEntryModel2.MTrackItem1 = ivModel.InvoiceEntry[0].MTrackItem1;
					iVReceiveEntryModel2.MTrackItem2 = ivModel.InvoiceEntry[0].MTrackItem2;
					iVReceiveEntryModel2.MTrackItem3 = ivModel.InvoiceEntry[0].MTrackItem3;
					iVReceiveEntryModel2.MTrackItem4 = ivModel.InvoiceEntry[0].MTrackItem4;
					iVReceiveEntryModel2.MTrackItem5 = ivModel.InvoiceEntry[0].MTrackItem5;
				}
				list.Add(iVReceiveEntryModel2);
			}
			iVReceiveModel.ReceiveEntry = list;
			return iVReceiveModel;
		}

		private List<CommandInfo> GetUpdateInvoiceVerificationCmd(MContext ctx, IVInvoiceModel ivModel, IVMakePaymentModel makePaymentModel)
		{
			if (ivModel.MType == "Invoice_Sale_Red" || ivModel.MType == "Invoice_Purchase_Red")
			{
				ivModel.MVerificationAmt -= makePaymentModel.MPaidAmtFor;
				ivModel.MVerifyAmtFor -= makePaymentModel.MPaidAmtFor;
			}
			else
			{
				ivModel.MVerificationAmt += makePaymentModel.MPaidAmtFor;
				ivModel.MVerifyAmtFor += makePaymentModel.MPaidAmtFor;
			}
			if (ivModel.MVerifyAmtFor == ivModel.MTaxTotalAmtFor)
			{
				ivModel.MStatus = Convert.ToInt32(IVInvoiceStatusEnum.Paid);
			}
			else
			{
				ivModel.MStatus = Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment);
			}
			ivModel.MExchangeRate = makePaymentModel.MOToLRate;
			return ModelInfoManager.GetInsertOrUpdateCmd<IVInvoiceModel>(ctx, ivModel, new List<string>
			{
				"MVerificationAmt",
				"MVerifyAmtFor",
				"MVerifyAmt",
				"MStatus"
			}, false);
		}
	}
}
