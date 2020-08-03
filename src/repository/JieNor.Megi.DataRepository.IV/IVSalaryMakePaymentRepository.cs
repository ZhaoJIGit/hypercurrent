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
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVSalaryMakePaymentRepository : IVMakePaymentAbstractRepository
	{
		public IVSalaryMakePaymentRepository(BillSourceType sourceType, CreateFromType creatFrom)
		{
			base._sourceType = sourceType;
			base._createFrom = creatFrom;
		}

		public IVSalaryMakePaymentRepository()
		{
		}

		public override MakePaymentResultModel GetToPayResult(MContext ctx, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel)
		{
			MakePaymentResultModel makePaymentResultModel = new MakePaymentResultModel();
			bool flag = true;
			string mMessage = "";
			List<CommandInfo> list = new List<CommandInfo>();
			string mTargetBillID = "";
			PASalaryPaymentModel salaryPaymentEditModel = PASalaryPaymentRepository.GetSalaryPaymentEditModel(ctx, makePaymentModel.MObjectID);
			OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, makePaymentModel.MPaidDate);
			if (!operationResult.Success)
			{
				makePaymentResultModel.MMessage = operationResult.Message;
				return makePaymentResultModel;
			}
			CheckPay(ctx, salaryPaymentEditModel, bankModel, makePaymentModel, out flag, out mMessage);
			if (!flag)
			{
				makePaymentResultModel.MMessage = mMessage;
				return makePaymentResultModel;
			}
			makePaymentModel.MLToORate = decimal.One;
			makePaymentModel.MPaidAmtFor = makePaymentModel.MPaidAmount;
			makePaymentModel.MPaidAmt = makePaymentModel.MPaidAmount;
			list.AddRange(GetPaymentCmd(ctx, salaryPaymentEditModel, makePaymentModel, bankModel, out mTargetBillID));
			makePaymentResultModel.MTargetBillType = "Payment";
			makePaymentResultModel.MTargetBillID = mTargetBillID;
			OperationResult operationResult2 = ResultHelper.ToOperationResult(salaryPaymentEditModel);
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

		public override MakePaymentResultModel GetToPayResults(MContext ctx, List<IVMakePaymentModel> makePaymentModels, BDBankAccountEditModel bankModel)
		{
			MakePaymentResultModel makePaymentResultModel = new MakePaymentResultModel();
			bool flag = true;
			string mMessage = "";
			List<CommandInfo> list = new List<CommandInfo>();
			List<PASalaryPaymentModel> list2 = new List<PASalaryPaymentModel>();
			foreach (IVMakePaymentModel makePaymentModel in makePaymentModels)
			{
				PASalaryPaymentModel salaryPaymentEditModel = PASalaryPaymentRepository.GetSalaryPaymentEditModel(ctx, makePaymentModel.MObjectID);
				if (salaryPaymentEditModel != null)
				{
					list2.Add(salaryPaymentEditModel);
				}
				OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, makePaymentModel.MPaidDate);
				if (!operationResult.Success)
				{
					makePaymentResultModel.MMessage = operationResult.Message;
					return makePaymentResultModel;
				}
				CheckPay(ctx, salaryPaymentEditModel, bankModel, makePaymentModel, out flag, out mMessage);
				if (!flag)
				{
					makePaymentResultModel.MMessage = mMessage;
					return makePaymentResultModel;
				}
				makePaymentModel.MLToORate = decimal.One;
				makePaymentModel.MPaidAmtFor = makePaymentModel.MPaidAmount;
				makePaymentModel.MPaidAmt = makePaymentModel.MPaidAmount;
			}
			list.AddRange(GetPaymentCmds(ctx, list2, makePaymentModels, bankModel));
			makePaymentResultModel.MTargetBillType = "Payment";
			foreach (PASalaryPaymentModel item in list2)
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

		private void CheckPay(MContext ctx, PASalaryPaymentModel salaryPaymentModel, BDBankAccountEditModel bankModel, IVMakePaymentModel makePaymentModel, out bool success, out string message)
		{
			DateTime dateTime = makePaymentModel.MPaidDate;
			DateTime date = dateTime.Date;
			dateTime = ctx.MBeginDate;
			if (date < dateTime.Date)
			{
				success = false;
				message = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "DateAfter", "The date should be later {0} or on this date.", ctx.MBeginDate.ToOrgZoneDateFormat(null));
			}
			else
			{
				base.CheckBank(ctx, bankModel, out success, out message);
				if (success)
				{
					if (salaryPaymentModel == null || string.IsNullOrEmpty(salaryPaymentModel.MID))
					{
						success = false;
						message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "TheSalaryListIsNotExists", "The Salary List is not exists!");
					}
					else
					{
						OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, makePaymentModel.MPaidDate);
						if (!operationResult.Success)
						{
							success = false;
							message = operationResult.Message;
						}
						else if (string.IsNullOrEmpty(makePaymentModel.SalaryPaymentIDLists))
						{
							base.CheckPaidAccount(ctx, salaryPaymentModel.MNetSalary, salaryPaymentModel.MVerificationAmt, makePaymentModel.MPaidAmtFor, out success, out message, true);
						}
					}
				}
			}
		}

		private List<CommandInfo> GetPaymentCmd(MContext ctx, PASalaryPaymentModel salaryPaymentModel, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel, out string paymentId)
		{
			IVPaymentModel paymentModel = GetPaymentModel(ctx, salaryPaymentModel, makePaymentModel, bankModel);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, paymentModel, null, true);
			paymentId = paymentModel.MID;
			string mID = salaryPaymentModel.MID;
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MRunID", makePaymentModel.PayRunID);
			List<PASalaryPaymentModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, sqlWhere, false, false);
			string salaryPaymentIDLists = makePaymentModel.SalaryPaymentIDLists;
			string[] salaryPaymentIdLists = salaryPaymentIDLists.Split(',');
			List<PASalaryPaymentModel> list = new List<PASalaryPaymentModel>();
			int i;
			for (i = 0; i < salaryPaymentIdLists.Length; i++)
			{
				PASalaryPaymentModel pASalaryPaymentModel = dataModelList.FirstOrDefault((PASalaryPaymentModel m) => m.MID == salaryPaymentIdLists[i]);
				if (pASalaryPaymentModel != null)
				{
					list.Add(pASalaryPaymentModel);
				}
				dataModelList.Remove(pASalaryPaymentModel);
			}
			if (makePaymentModel.IsPayRun)
			{
				if (!string.IsNullOrEmpty(salaryPaymentIDLists) && list.Any())
				{
					PASalaryToIVPayModel pASalaryToIVPayModel = new PASalaryToIVPayModel();
					pASalaryToIVPayModel.MEmployeeCount = list.Count;
					pASalaryToIVPayModel.MTotalTaxSalary = list.Sum((PASalaryPaymentModel m) => m.MTaxSalary);
					pASalaryToIVPayModel.MTotalNetSalary = list.Sum((PASalaryPaymentModel m) => m.MNetSalary);
					pASalaryToIVPayModel.MTotalVerificationAmt = list.Sum((PASalaryPaymentModel m) => m.MVerificationAmt);
					pASalaryToIVPayModel.MTotalVerifyAmtFor = list.Sum((PASalaryPaymentModel m) => m.MVerifyAmtFor);
					pASalaryToIVPayModel.MTotalVerifyAmt = list.Sum((PASalaryPaymentModel m) => m.MVerifyAmt);
					insertOrUpdateCmd.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PASalaryToIVPayModel>(ctx, pASalaryToIVPayModel, null, true));
					mID = pASalaryToIVPayModel.MID;
					List<PASalaryToIVPayEntryModel> list2 = new List<PASalaryToIVPayEntryModel>();
					for (int j = 0; j < list.Count; j++)
					{
						list[j].MStatus = 4;
						PASalaryToIVPayEntryModel pASalaryToIVPayEntryModel = new PASalaryToIVPayEntryModel();
						pASalaryToIVPayEntryModel.MOrgID = ctx.MOrgID;
						pASalaryToIVPayEntryModel.MSalaryPaymentID = list[j].MID;
						pASalaryToIVPayEntryModel.MAmount = list[j].MNetSalary - list[j].MVerifyAmtFor;
						pASalaryToIVPayEntryModel.MID = mID;
						list2.Add(pASalaryToIVPayEntryModel);
						CommandInfo commandInfo = new CommandInfo();
						commandInfo.CommandText = " UPDATE T_PA_SalaryPayment SET MStatus = @MStatus,MVerificationAmt=@MVerificationAmt ,\r\n                                                         MVerifyAmtFor=@MVerifyAmtFor ,MVerifyAmt=@MVerifyAmt \r\n                                                         WHERE MID =@MID AND MOrgID = @MOrgID AND MIsDelete = 0 ";
						MySqlParameter[] parameters = new MySqlParameter[6]
						{
							new MySqlParameter("@MVerificationAmt", list[j].MNetSalary),
							new MySqlParameter("@MVerifyAmtFor", list[j].MNetSalary),
							new MySqlParameter("@MVerifyAmt", list[j].MNetSalary),
							new MySqlParameter("@MID", list[j].MID),
							new MySqlParameter("@MStatus", 4),
							new MySqlParameter("@MOrgID", ctx.MOrgID)
						};
						DbParameter[] array = commandInfo.Parameters = parameters;
						insertOrUpdateCmd.Add(commandInfo);
					}
					if (list2.Any())
					{
						insertOrUpdateCmd.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, null, true));
					}
				}
			}
			else
			{
				insertOrUpdateCmd.Add(GetUpdateSalaryPaymentVerificationCmd(ctx, salaryPaymentModel.MID, makePaymentModel.MPaidAmount));
			}
			if (!string.IsNullOrEmpty(makePaymentModel.PayRunID) && dataModelList.All((PASalaryPaymentModel m) => m.MStatus == 4))
			{
				List<string> list3 = new List<string>();
				list3.Add("MStatus");
				PAPayRunModel pAPayRunModel = new PAPayRunModel();
				pAPayRunModel.MID = makePaymentModel.PayRunID;
				pAPayRunModel.MStatus = 4;
				insertOrUpdateCmd.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayRunModel>(ctx, pAPayRunModel, list3, true));
			}
			IVVerificationModel verificationModel = base.GetVerificationModel(ctx, paymentModel.MID, "Payment", mID, "PayRun", makePaymentModel.MPaidAmtFor, makePaymentModel.MPaidAmt);
			insertOrUpdateCmd.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, verificationModel, null, true));
			insertOrUpdateCmd.Add(PASalaryPaymentLogRepository.GetAddSalaryPaymentPaidLogCmd(ctx, salaryPaymentModel.MID, makePaymentModel.MPaidDate, makePaymentModel.MPaidAmount));
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, paymentModel, null);
			if (operationResult.Success)
			{
				insertOrUpdateCmd.AddRange(operationResult.OperationCommands);
			}
			else
			{
				salaryPaymentModel.ValidationErrors = paymentModel.ValidationErrors;
			}
			return insertOrUpdateCmd;
		}

		private List<CommandInfo> GetPaymentCmds(MContext ctx, List<PASalaryPaymentModel> salaryPaymentModels, List<IVMakePaymentModel> makePaymentModels, BDBankAccountEditModel bankModel)
		{
			List<IVPaymentModel> list = new List<IVPaymentModel>();
			List<IVVerificationModel> list2 = new List<IVVerificationModel>();
			List<CommandInfo> list3 = new List<CommandInfo>();
			foreach (IVMakePaymentModel makePaymentModel in makePaymentModels)
			{
				PASalaryPaymentModel pASalaryPaymentModel = (from t in salaryPaymentModels
				where t.MID == makePaymentModel.MObjectID
				select t).FirstOrDefault();
				if (pASalaryPaymentModel != null)
				{
					IVPaymentModel paymentModel = GetPaymentModel(ctx, pASalaryPaymentModel, makePaymentModel, bankModel);
					list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, paymentModel, null, true));
					string mID = pASalaryPaymentModel.MID;
					SqlWhere sqlWhere = new SqlWhere();
					sqlWhere.Equal("MRunID", makePaymentModel.PayRunID);
					List<PASalaryPaymentModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, sqlWhere, false, false);
					string salaryPaymentIDLists = makePaymentModel.SalaryPaymentIDLists;
					string[] salaryPaymentIdLists = salaryPaymentIDLists.Split(',');
					List<PASalaryPaymentModel> list4 = new List<PASalaryPaymentModel>();
					int i;
					for (i = 0; i < salaryPaymentIdLists.Length; i++)
					{
						PASalaryPaymentModel pASalaryPaymentModel2 = dataModelList.FirstOrDefault((PASalaryPaymentModel m) => m.MID == salaryPaymentIdLists[i]);
						if (pASalaryPaymentModel2 != null)
						{
							list4.Add(pASalaryPaymentModel2);
						}
						dataModelList.Remove(pASalaryPaymentModel2);
					}
					if (makePaymentModel.IsPayRun)
					{
						if (!string.IsNullOrEmpty(salaryPaymentIDLists) && list4.Any())
						{
							PASalaryToIVPayModel pASalaryToIVPayModel = new PASalaryToIVPayModel();
							pASalaryToIVPayModel.MEmployeeCount = list4.Count;
							pASalaryToIVPayModel.MTotalTaxSalary = list4.Sum((PASalaryPaymentModel m) => m.MTaxSalary);
							pASalaryToIVPayModel.MTotalNetSalary = list4.Sum((PASalaryPaymentModel m) => m.MNetSalary);
							pASalaryToIVPayModel.MTotalVerificationAmt = list4.Sum((PASalaryPaymentModel m) => m.MVerificationAmt);
							pASalaryToIVPayModel.MTotalVerifyAmtFor = list4.Sum((PASalaryPaymentModel m) => m.MVerifyAmtFor);
							pASalaryToIVPayModel.MTotalVerifyAmt = list4.Sum((PASalaryPaymentModel m) => m.MVerifyAmt);
							list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PASalaryToIVPayModel>(ctx, pASalaryToIVPayModel, null, true));
							mID = pASalaryToIVPayModel.MID;
							List<PASalaryToIVPayEntryModel> list5 = new List<PASalaryToIVPayEntryModel>();
							for (int j = 0; j < list4.Count; j++)
							{
								list4[j].MStatus = 4;
								PASalaryToIVPayEntryModel pASalaryToIVPayEntryModel = new PASalaryToIVPayEntryModel();
								pASalaryToIVPayEntryModel.MOrgID = ctx.MOrgID;
								pASalaryToIVPayEntryModel.MSalaryPaymentID = list4[j].MID;
								pASalaryToIVPayEntryModel.MAmount = list4[j].MNetSalary - list4[j].MVerifyAmtFor;
								pASalaryToIVPayEntryModel.MID = mID;
								list5.Add(pASalaryToIVPayEntryModel);
								CommandInfo commandInfo = new CommandInfo();
								commandInfo.CommandText = " UPDATE T_PA_SalaryPayment SET MStatus = @MStatus,MVerificationAmt=@MVerificationAmt ,\r\n                                                         MVerifyAmtFor=@MVerifyAmtFor ,MVerifyAmt=@MVerifyAmt \r\n                                                         WHERE MID =@MID AND MOrgID = @MOrgID AND MIsDelete = 0 ";
								MySqlParameter[] parameters = new MySqlParameter[6]
								{
									new MySqlParameter("@MVerificationAmt", list4[j].MNetSalary),
									new MySqlParameter("@MVerifyAmtFor", list4[j].MNetSalary),
									new MySqlParameter("@MVerifyAmt", list4[j].MNetSalary),
									new MySqlParameter("@MID", list4[j].MID),
									new MySqlParameter("@MStatus", 4),
									new MySqlParameter("@MOrgID", ctx.MOrgID)
								};
								DbParameter[] array = commandInfo.Parameters = parameters;
								list3.Add(commandInfo);
							}
							if (list5.Any())
							{
								list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list5, null, true));
							}
						}
					}
					else
					{
						list3.Add(GetUpdateSalaryPaymentVerificationCmd(ctx, pASalaryPaymentModel.MID, makePaymentModel.MPaidAmount));
					}
					if (!string.IsNullOrEmpty(makePaymentModel.PayRunID) && dataModelList.All((PASalaryPaymentModel m) => m.MStatus == 4))
					{
						List<string> list6 = new List<string>();
						list6.Add("MStatus");
						PAPayRunModel pAPayRunModel = new PAPayRunModel();
						pAPayRunModel.MID = makePaymentModel.PayRunID;
						pAPayRunModel.MStatus = 4;
						list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayRunModel>(ctx, pAPayRunModel, list6, true));
					}
					IVVerificationModel verificationModel = base.GetVerificationModel(ctx, paymentModel.MID, "Payment", mID, "PayRun", makePaymentModel.MPaidAmtFor, makePaymentModel.MPaidAmt);
					list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, verificationModel, null, true));
					list3.Add(PASalaryPaymentLogRepository.GetAddSalaryPaymentPaidLogCmd(ctx, pASalaryPaymentModel.MID, makePaymentModel.MPaidDate, makePaymentModel.MPaidAmount));
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
				foreach (PASalaryPaymentModel salaryPaymentModel in salaryPaymentModels)
				{
					IVVerificationModel vriModel = (from t in list2
					where t.MTargetBillID == salaryPaymentModel.MID
					select t).FirstOrDefault();
					if (vriModel != null)
					{
						IVPaymentModel iVPaymentModel = (from t in list
						where t.MID == vriModel.MSourceBillID
						select t).FirstOrDefault();
						if (iVPaymentModel != null)
						{
							salaryPaymentModel.ValidationErrors = iVPaymentModel.ValidationErrors;
						}
					}
				}
			}
			return list3;
		}

		private IVPaymentModel GetPaymentModel(MContext ctx, PASalaryPaymentModel salaryPaymentModel, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel)
		{
			string paymentBizType = base.GetPaymentBizType("Pay_Salary");
			decimal one = decimal.One;
			decimal mPaidAmount = makePaymentModel.MPaidAmount;
			string mDesc = $"Payment:{salaryPaymentModel.MReference}";
			IVPaymentModel iVPaymentModel = new IVPaymentModel();
			iVPaymentModel.MBankID = bankModel.MItemID;
			iVPaymentModel.MCyID = bankModel.MCyID;
			iVPaymentModel.MContactType = "Employees";
			iVPaymentModel.MNumber = salaryPaymentModel.MDate.ToString("yyyy-MM");
			if (!makePaymentModel.IsPayRun)
			{
				iVPaymentModel.MContactID = salaryPaymentModel.MEmployeeID;
			}
			iVPaymentModel.MReference = makePaymentModel.MRef;
			iVPaymentModel.MDesc = mDesc;
			iVPaymentModel.MBizDate = makePaymentModel.MPaidDate;
			iVPaymentModel.MType = paymentBizType;
			iVPaymentModel.MTaxID = "No_Tax";
			iVPaymentModel.MExchangeRate = one;
			iVPaymentModel.MOrgID = ctx.MOrgID;
			iVPaymentModel.MVerificationAmt = mPaidAmount;
			iVPaymentModel.MVerifyAmtFor = mPaidAmount;
			iVPaymentModel.MSource = Convert.ToInt32(base._sourceType);
			iVPaymentModel.MCreateFrom = Convert.ToInt32(base._createFrom);
			iVPaymentModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
			IVPaymentEntryModel iVPaymentEntryModel = new IVPaymentEntryModel();
			iVPaymentEntryModel.MID = iVPaymentModel.MID;
			iVPaymentEntryModel.MQty = decimal.One;
			iVPaymentEntryModel.MPrice = mPaidAmount;
			iVPaymentEntryModel.MDesc = mDesc;
			iVPaymentEntryModel.MTaxAmt = decimal.Zero;
			iVPaymentEntryModel.MTaxAmtFor = decimal.Zero;
			iVPaymentModel.PaymentEntry = new List<IVPaymentEntryModel>
			{
				iVPaymentEntryModel
			};
			return iVPaymentModel;
		}

		private CommandInfo GetUpdateSalaryPaymentVerificationCmd(MContext ctx, string pkId, decimal amt)
		{
			string sqlText = string.Format(" Update T_PA_SalaryPayment Set MStatus= CASE WHEN MNetSalary=IFNULL(MVerificationAmt,0)+{0} THEN {1} ELSE {2} END,\r\n                                         MVerificationAmt =IFNULL(MVerificationAmt,0)+{0} ,\r\n                                        MVerifyAmtFor =IFNULL(MVerifyAmtFor,0)+{0} ,\r\n                                        MVerifyAmt =IFNULL(MVerifyAmt,0)+{0} \r\n                                         Where MID='{3}' and MOrgID = '{4}' and MIsDelete = 0 ", amt, Convert.ToInt32(IVInvoiceStatusEnum.Paid), Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment), pkId, ctx.MOrgID);
			return new CommandInfo(sqlText, null);
		}
	}
}
