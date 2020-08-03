using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.BD.InitDocument;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.Log;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDInitDocumentBusiness : IBDInitDocumentBusiness, IDataContract<BDInitDocumentModel>
	{
		private GLUtility utility = new GLUtility();

		private readonly BASLangBusiness lang = new BASLangBusiness();

		private readonly BDInitDocumentRepository dal = new BDInitDocumentRepository();

		private readonly BDAccountRepository accountDal = new BDAccountRepository();

		private List<string> GetBankIDList(BDInitDocumentViewModel model)
		{
			List<string> list = new List<string>();
			int num = 0;
			while (model.MPaymentList != null && num < model.MPaymentList.Count)
			{
				list.Add(model.MPaymentList[num].MBankID);
				num++;
			}
			int num2 = 0;
			while (model.MReceiveList != null && num2 < model.MReceiveList.Count)
			{
				list.Add(model.MReceiveList[num2].MBankID);
				num2++;
			}
			return list.Distinct().ToList();
		}

		public List<MActionResultCodeEnum> ValidateInitDocumentModel(MContext ctx, BDInitDocumentViewModel model)
		{
			ValidateQueryModel validateInitBalanceOverSql = utility.GetValidateInitBalanceOverSql(ctx, false);
			List<ValidateQueryModel> validateCheckGroupValueModel = utility.GetValidateCheckGroupValueModel((from x in model.InitBillList
			select x.MCheckGroupValueModel ?? new GLCheckGroupValueModel()).ToList());
			ValidateQueryModel validateInvoiceSaleNumberSql = utility.GetValidateInvoiceSaleNumberSql(model.MSaleInvoiceList);
			ValidateQueryModel validateCommonModelSql = utility.GetValidateCommonModelSql<BDBankAccountModel>(MActionResultCodeEnum.MBankAccountInvalid, GetBankIDList(model), null, null);
			List<string> ids = (from x in model.InitBillList
			select x.MCurrentAccountCode).ToList();
			ValidateQueryModel validateCommonModelSql2 = utility.GetValidateCommonModelSql<BDAccountModel>(MActionResultCodeEnum.MAccountInvalid, ids, "MCode", null);
			ValidateQueryModel validateAccountHasSubSql = utility.GetValidateAccountHasSubSql(ids, true);
			List<ValidateQueryModel> queryList = new List<ValidateQueryModel>
			{
				validateInitBalanceOverSql,
				validateInvoiceSaleNumberSql,
				validateCommonModelSql2,
				validateAccountHasSubSql,
				validateCommonModelSql
			}.Concat(validateCheckGroupValueModel).ToList();
			utility.CheckInitDocumentCheckGroupValueMatchWithAccount(ctx, model.InitBillList);
			return utility.QueryValidateSql(ctx, queryList, true);
		}

		public List<CommandInfo> GetInitDocumentInsertOrUpdateCmds(MContext ctx, GLInitBalanceModel initBalance, string preBillNumber, List<BDExchangeRateModel> insertExchangeRateList, out string billID, out string billNumber, bool isMatchBill = true)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			billID = string.Empty;
			billNumber = string.Empty;
			if (string.IsNullOrWhiteSpace(initBalance.MBillID) && initBalance.MInitBalance == decimal.Zero && initBalance.MInitBalanceFor == decimal.Zero)
			{
				billNumber = preBillNumber;
				return list;
			}
			if (!string.IsNullOrWhiteSpace(initBalance.MBillID) && initBalance.MInitBalance == decimal.Zero && initBalance.MInitBalanceFor == decimal.Zero)
			{
				list.AddRange(GetDeleteAutoCreateInitBillCmds(ctx, initBalance));
				billNumber = preBillNumber;
				return list;
			}
			if (string.IsNullOrWhiteSpace(initBalance.MBillType) && string.IsNullOrWhiteSpace(initBalance.MBillID))
			{
				billNumber = preBillNumber;
				return list;
			}
			string mBillType = initBalance.MBillType;
			if (!string.IsNullOrWhiteSpace(initBalance.MBillID) && string.IsNullOrWhiteSpace(initBalance.MBillType))
			{
				list.AddRange(GetDeleteAutoCreateInitBillCmds(ctx, initBalance));
				billNumber = preBillNumber;
				return list;
			}
			bool flag = string.IsNullOrWhiteSpace(initBalance.MBillID);
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "FromAutoCreateBill", "from account initial balance");
			if (flag)
			{
				if (!isMatchBill)
				{
					initBalance.MBillID = string.Empty;
				}
				else
				{
					DataSet billIdByInitBalance = dal.GetBillIdByInitBalance(ctx, initBalance);
					if (billIdByInitBalance != null && billIdByInitBalance.Tables.Count > 0 && billIdByInitBalance.Tables[0].Rows.Count > 0)
					{
						billID = Convert.ToString(billIdByInitBalance.Tables[0].Rows[0][0]);
						return list;
					}
					initBalance.MBillID = string.Empty;
				}
			}
			decimal num = CalculateExchange(initBalance.MInitBalance, initBalance.MInitBalanceFor, initBalance.MExchangeRate);
			DateTime dateTime;
			if (mBillType == "Invoice_Sale" || mBillType == "Invoice_Purchase")
			{
				IVInvoiceModel iVInvoiceModel = null;
				if (!string.IsNullOrWhiteSpace(initBalance.MBillID))
				{
					SqlWhere sqlWhere = new SqlWhere();
					sqlWhere.Equal("MID", initBalance.MBillID);
					sqlWhere.Equal("MIsDelete", 0);
					iVInvoiceModel = IVInvoiceRepository.GetInvoiceListIncludeEntry(ctx, sqlWhere).FirstOrDefault();
				}
				if (iVInvoiceModel == null)
				{
					iVInvoiceModel = new IVInvoiceModel();
					iVInvoiceModel.MID = UUIDHelper.GetGuid();
					iVInvoiceModel.IsNew = true;
					billID = iVInvoiceModel.MID;
					flag = true;
				}
				else
				{
					billID = iVInvoiceModel.MID;
				}
				if (string.IsNullOrWhiteSpace(iVInvoiceModel.MNumber) && mBillType == "Invoice_Sale")
				{
					if (string.IsNullOrWhiteSpace(preBillNumber))
					{
						billNumber = IVInvoiceRepository.GetInvoiceNumber(mBillType, ctx.MOrgID, ctx, billID, null);
					}
					else if (mBillType == "Invoice_Sale")
					{
						string s = preBillNumber.Split('-').LastOrDefault();
						int num2 = int.Parse(s) + 1;
						num2 = 10000 + num2;
						billNumber = string.Format("{0}-{1}", "INV", num2.ToString().Substring(1, 4));
					}
					iVInvoiceModel.MNumber = billNumber;
				}
				iVInvoiceModel.MContactID = initBalance.MCheckGroupValueModel.MContactID;
				IVInvoiceModel iVInvoiceModel2 = iVInvoiceModel;
				dateTime = ctx.MBeginDate;
				iVInvoiceModel2.MBizDate = dateTime.AddDays(-1.0);
				IVInvoiceModel iVInvoiceModel3 = iVInvoiceModel;
				dateTime = ctx.MBeginDate;
				iVInvoiceModel3.MExpectedDate = dateTime.AddDays(-1.0);
				dateTime = iVInvoiceModel.MBizDate;
				DateTime dateTime2 = dateTime.AddMonths(1);
				dateTime = iVInvoiceModel.MBizDate;
				int year = dateTime.Year;
				dateTime = iVInvoiceModel.MBizDate;
				dateTime = dateTime.AddMonths(1);
				int day = DateTime.DaysInMonth(year, dateTime.Month);
				iVInvoiceModel.MDueDate = new DateTime(dateTime2.Year, dateTime2.Month, day);
				iVInvoiceModel.MCyID = initBalance.MCurrencyID;
				iVInvoiceModel.MType = mBillType;
				iVInvoiceModel.MExchangeRate = num;
				iVInvoiceModel.MCurrentAccountCode = initBalance.MAccountCode;
				iVInvoiceModel.MReference = (flag ? text : iVInvoiceModel.MReference);
				iVInvoiceModel.MTaxTotalAmt = initBalance.MInitBalance;
				iVInvoiceModel.MTaxTotalAmtFor = initBalance.MInitBalanceFor;
				iVInvoiceModel.MTotalAmt = initBalance.MInitBalance;
				iVInvoiceModel.MTotalAmtFor = initBalance.MInitBalanceFor;
				iVInvoiceModel.MTaxID = "No_Tax";
				iVInvoiceModel.MStatus = 3;
				iVInvoiceModel.MIsAutoAmount = false;
				IVInvoiceEntryModel iVInvoiceEntryModel = new IVInvoiceEntryModel();
				iVInvoiceEntryModel.MIsAutoAmount = false;
				iVInvoiceEntryModel.MID = iVInvoiceModel.MID;
				if (flag)
				{
					iVInvoiceEntryModel.IsNew = true;
				}
				else
				{
					iVInvoiceEntryModel = iVInvoiceModel.MEntryList.First();
				}
				iVInvoiceEntryModel.MItemID = initBalance.MCheckGroupValueModel.MMerItemID;
				iVInvoiceEntryModel.MTrackItem1 = initBalance.MCheckGroupValueModel.MTrackItem1;
				iVInvoiceEntryModel.MTrackItem2 = initBalance.MCheckGroupValueModel.MTrackItem2;
				iVInvoiceEntryModel.MTrackItem3 = initBalance.MCheckGroupValueModel.MTrackItem3;
				iVInvoiceEntryModel.MTrackItem4 = initBalance.MCheckGroupValueModel.MTrackItem4;
				iVInvoiceEntryModel.MTrackItem5 = initBalance.MCheckGroupValueModel.MTrackItem5;
				iVInvoiceEntryModel.MQty = decimal.One;
				iVInvoiceEntryModel.MPrice = initBalance.MInitBalanceFor;
				iVInvoiceEntryModel.MTaxAmount = initBalance.MInitBalance;
				iVInvoiceEntryModel.MTaxAmountFor = initBalance.MInitBalanceFor;
				iVInvoiceEntryModel.MAmount = initBalance.MInitBalance;
				iVInvoiceEntryModel.MAmountFor = initBalance.MInitBalanceFor;
				iVInvoiceEntryModel.MExchangeRate = num;
				iVInvoiceEntryModel.MDesc = text;
				if (flag)
				{
					iVInvoiceModel.InvoiceEntry.Add(iVInvoiceEntryModel);
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVInvoiceModel>(ctx, iVInvoiceModel, null, true));
			}
			else if (mBillType == "Receive_Sale")
			{
				IVReceiveModel iVReceiveModel = null;
				if (!string.IsNullOrWhiteSpace(initBalance.MBillID))
				{
					SqlWhere sqlWhere2 = new SqlWhere();
					sqlWhere2.Equal("MID", initBalance.MBillID);
					sqlWhere2.Equal("MIsDelete", 0);
					iVReceiveModel = IVReceiveRepository.GetReceiveModelIncludeEntry(ctx, sqlWhere2).FirstOrDefault();
				}
				if (iVReceiveModel == null || string.IsNullOrWhiteSpace(iVReceiveModel.MID))
				{
					iVReceiveModel = new IVReceiveModel();
					iVReceiveModel.MID = UUIDHelper.GetGuid();
					iVReceiveModel.IsNew = true;
					billID = iVReceiveModel.MID;
					flag = true;
				}
				else
				{
					billID = iVReceiveModel.MID;
				}
				iVReceiveModel.MContactID = GetContactValue(initBalance);
				iVReceiveModel.MContactType = initBalance.MContactTypeFromBill;
				iVReceiveModel.MReconcileStatu = 201;
				IVReceiveModel iVReceiveModel2 = iVReceiveModel;
				dateTime = ctx.MBeginDate;
				iVReceiveModel2.MBizDate = dateTime.AddDays(-1.0);
				iVReceiveModel.MCyID = initBalance.MCurrencyID;
				iVReceiveModel.MType = mBillType;
				iVReceiveModel.MExchangeRate = num;
				iVReceiveModel.MCurrentAccountCode = initBalance.MAccountCode;
				if (string.IsNullOrWhiteSpace(iVReceiveModel.MBankID) || iVReceiveModel.MBankID == initBalance.MBankID)
				{
					iVReceiveModel.MBankID = initBalance.MBankID;
				}
				else if (iVReceiveModel.MBankID != initBalance.MBankID)
				{
					iVReceiveModel.MBankID = iVReceiveModel.MBankID;
				}
				iVReceiveModel.MTaxTotalAmt = initBalance.MInitBalance;
				iVReceiveModel.MTaxTotalAmtFor = initBalance.MInitBalanceFor;
				iVReceiveModel.MTotalAmt = initBalance.MInitBalance;
				iVReceiveModel.MTotalAmtFor = initBalance.MInitBalanceFor;
				iVReceiveModel.MTaxID = "No_Tax";
				iVReceiveModel.MReference = text;
				iVReceiveModel.MIsAutoAmount = false;
				IVReceiveEntryModel iVReceiveEntryModel = new IVReceiveEntryModel();
				iVReceiveEntryModel.MID = iVReceiveModel.MID;
				iVReceiveEntryModel.MIsAutoAmount = false;
				if (flag)
				{
					iVReceiveEntryModel.IsNew = true;
				}
				else
				{
					iVReceiveEntryModel = iVReceiveModel.MEntryList.First();
				}
				iVReceiveEntryModel.MItemID = initBalance.MCheckGroupValueModel.MMerItemID;
				if (initBalance.MContactType == "Employees")
				{
					iVReceiveEntryModel.MItemID = initBalance.MCheckGroupValueModel.MExpItemID;
				}
				iVReceiveEntryModel.MTrackItem1 = initBalance.MCheckGroupValueModel.MTrackItem1;
				iVReceiveEntryModel.MTrackItem2 = initBalance.MCheckGroupValueModel.MTrackItem2;
				iVReceiveEntryModel.MTrackItem3 = initBalance.MCheckGroupValueModel.MTrackItem3;
				iVReceiveEntryModel.MTrackItem4 = initBalance.MCheckGroupValueModel.MTrackItem4;
				iVReceiveEntryModel.MTrackItem5 = initBalance.MCheckGroupValueModel.MTrackItem5;
				iVReceiveEntryModel.MQty = decimal.One;
				iVReceiveEntryModel.MPrice = initBalance.MInitBalanceFor;
				iVReceiveEntryModel.MTaxAmount = initBalance.MInitBalance;
				iVReceiveEntryModel.MTaxAmountFor = initBalance.MInitBalanceFor;
				iVReceiveEntryModel.MAmount = initBalance.MInitBalance;
				iVReceiveEntryModel.MAmountFor = initBalance.MInitBalanceFor;
				iVReceiveEntryModel.MExchangeRate = num;
				iVReceiveEntryModel.MDesc = text;
				if (flag)
				{
					iVReceiveModel.ReceiveEntry.Add(iVReceiveEntryModel);
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVReceiveModel>(ctx, iVReceiveModel, null, true));
			}
			else if (mBillType == "Pay_Purchase")
			{
				IVPaymentModel iVPaymentModel = null;
				if (!string.IsNullOrWhiteSpace(initBalance.MBillID))
				{
					SqlWhere sqlWhere3 = new SqlWhere();
					sqlWhere3.Equal("MID", initBalance.MBillID);
					sqlWhere3.Equal("MIsDelete", 0);
					iVPaymentModel = IVPaymentRepository.GetPaymentModelIncludeEntry(ctx, sqlWhere3).FirstOrDefault();
				}
				if (iVPaymentModel == null || string.IsNullOrWhiteSpace(iVPaymentModel.MID))
				{
					iVPaymentModel = new IVPaymentModel();
					iVPaymentModel.MID = UUIDHelper.GetGuid();
					iVPaymentModel.IsNew = true;
					flag = true;
					billID = iVPaymentModel.MID;
				}
				else
				{
					billID = iVPaymentModel.MID;
				}
				iVPaymentModel.MContactID = GetContactValue(initBalance);
				iVPaymentModel.MContactType = initBalance.MContactTypeFromBill;
				IVPaymentModel iVPaymentModel2 = iVPaymentModel;
				dateTime = ctx.MBeginDate;
				iVPaymentModel2.MBizDate = dateTime.AddDays(-1.0);
				iVPaymentModel.MReconcileStatu = 201;
				iVPaymentModel.MCyID = initBalance.MCurrencyID;
				iVPaymentModel.MType = mBillType;
				iVPaymentModel.MExchangeRate = num;
				iVPaymentModel.MCurrentAccountCode = initBalance.MAccountCode;
				if (string.IsNullOrWhiteSpace(iVPaymentModel.MBankID) || iVPaymentModel.MBankID == initBalance.MBankID)
				{
					iVPaymentModel.MBankID = initBalance.MBankID;
				}
				else if (iVPaymentModel.MBankID != initBalance.MBankID)
				{
					iVPaymentModel.MBankID = iVPaymentModel.MBankID;
				}
				iVPaymentModel.MTaxTotalAmt = initBalance.MInitBalance;
				iVPaymentModel.MTaxTotalAmtFor = initBalance.MInitBalanceFor;
				iVPaymentModel.MTotalAmt = initBalance.MInitBalance;
				iVPaymentModel.MTotalAmtFor = initBalance.MInitBalanceFor;
				iVPaymentModel.MTaxID = "No_Tax";
				iVPaymentModel.MReference = text;
				iVPaymentModel.MIsAutoAmount = false;
				IVPaymentEntryModel iVPaymentEntryModel = new IVPaymentEntryModel();
				iVPaymentEntryModel.MIsAutoAmount = false;
				iVPaymentEntryModel.MID = iVPaymentModel.MID;
				if (flag)
				{
					iVPaymentEntryModel.IsNew = true;
				}
				else
				{
					iVPaymentEntryModel = iVPaymentModel.PaymentEntry.First();
				}
				iVPaymentEntryModel.MItemID = initBalance.MCheckGroupValueModel.MMerItemID;
				if (initBalance.MContactType == "Employees")
				{
					iVPaymentEntryModel.MItemID = initBalance.MCheckGroupValueModel.MExpItemID;
				}
				iVPaymentEntryModel.MTrackItem1 = initBalance.MCheckGroupValueModel.MTrackItem1;
				iVPaymentEntryModel.MTrackItem2 = initBalance.MCheckGroupValueModel.MTrackItem2;
				iVPaymentEntryModel.MTrackItem3 = initBalance.MCheckGroupValueModel.MTrackItem3;
				iVPaymentEntryModel.MTrackItem4 = initBalance.MCheckGroupValueModel.MTrackItem4;
				iVPaymentEntryModel.MTrackItem5 = initBalance.MCheckGroupValueModel.MTrackItem5;
				iVPaymentEntryModel.MQty = decimal.One;
				iVPaymentEntryModel.MPrice = initBalance.MInitBalanceFor;
				iVPaymentEntryModel.MTaxAmount = initBalance.MInitBalance;
				iVPaymentEntryModel.MTaxAmountFor = initBalance.MInitBalanceFor;
				iVPaymentEntryModel.MAmount = initBalance.MInitBalance;
				iVPaymentEntryModel.MAmountFor = initBalance.MInitBalanceFor;
				iVPaymentEntryModel.MExchangeRate = num;
				iVPaymentEntryModel.MDesc = text;
				if (flag)
				{
					iVPaymentModel.PaymentEntry.Add(iVPaymentEntryModel);
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, iVPaymentModel, null, true));
			}
			else if (mBillType == "Expense_Claims")
			{
				IVExpenseModel iVExpenseModel = null;
				if (!string.IsNullOrWhiteSpace(initBalance.MBillID))
				{
					SqlWhere sqlWhere4 = new SqlWhere();
					sqlWhere4.Equal("MID", initBalance.MBillID);
					sqlWhere4.Equal("MIsDelete", 0);
					iVExpenseModel = IVExpenseRepository.GetExpenseModelIncludeEntry(ctx, sqlWhere4).FirstOrDefault();
				}
				if (iVExpenseModel == null || string.IsNullOrWhiteSpace(iVExpenseModel.MID))
				{
					iVExpenseModel = new IVExpenseModel();
					iVExpenseModel.MID = billID;
					iVExpenseModel.IsNew = true;
				}
				iVExpenseModel.MID = billID;
				iVExpenseModel.IsNew = flag;
				iVExpenseModel.MContactID = GetContactValue(initBalance);
				iVExpenseModel.MEmployee = iVExpenseModel.MContactID;
				IVExpenseModel iVExpenseModel2 = iVExpenseModel;
				dateTime = ctx.MBeginDate;
				iVExpenseModel2.MBizDate = dateTime.AddDays(-1.0);
				dateTime = iVExpenseModel.MBizDate;
				DateTime dateTime3 = dateTime.AddMonths(1);
				dateTime = iVExpenseModel.MBizDate;
				int year2 = dateTime.Year;
				dateTime = iVExpenseModel.MBizDate;
				dateTime = dateTime.AddMonths(1);
				int day2 = DateTime.DaysInMonth(year2, dateTime.Month);
				iVExpenseModel.MDueDate = new DateTime(dateTime3.Year, dateTime3.Month, day2);
				iVExpenseModel.MCyID = initBalance.MCurrencyID;
				iVExpenseModel.MType = mBillType;
				iVExpenseModel.MExchangeRate = num;
				iVExpenseModel.MCurrentAccountCode = initBalance.MAccountCode;
				iVExpenseModel.MTaxTotalAmt = initBalance.MInitBalance;
				iVExpenseModel.MTaxTotalAmtFor = initBalance.MInitBalanceFor;
				iVExpenseModel.MTotalAmt = initBalance.MInitBalance;
				iVExpenseModel.MTotalAmtFor = initBalance.MInitBalanceFor;
				iVExpenseModel.MTaxID = "No_Tax";
				iVExpenseModel.MReference = text;
				iVExpenseModel.MStatus = 3;
				iVExpenseModel.MIsAutoAmount = false;
				IVExpenseEntryModel iVExpenseEntryModel = new IVExpenseEntryModel();
				iVExpenseEntryModel.MIsAutoAmount = false;
				iVExpenseEntryModel.MID = iVExpenseModel.MID;
				if (flag)
				{
					iVExpenseEntryModel.IsNew = true;
				}
				else
				{
					iVExpenseEntryModel = iVExpenseModel.MEntryList.First();
				}
				iVExpenseEntryModel.MItemID = initBalance.MCheckGroupValueModel.MExpItemID;
				iVExpenseEntryModel.MTrackItem1 = initBalance.MCheckGroupValueModel.MTrackItem1;
				iVExpenseEntryModel.MTrackItem2 = initBalance.MCheckGroupValueModel.MTrackItem2;
				iVExpenseEntryModel.MTrackItem3 = initBalance.MCheckGroupValueModel.MTrackItem3;
				iVExpenseEntryModel.MTrackItem4 = initBalance.MCheckGroupValueModel.MTrackItem4;
				iVExpenseEntryModel.MTrackItem5 = initBalance.MCheckGroupValueModel.MTrackItem5;
				iVExpenseEntryModel.MQty = decimal.One;
				iVExpenseEntryModel.MPrice = initBalance.MInitBalanceFor;
				iVExpenseEntryModel.MTaxAmount = initBalance.MInitBalance;
				iVExpenseEntryModel.MTaxAmountFor = initBalance.MInitBalanceFor;
				iVExpenseEntryModel.MAmount = initBalance.MInitBalance;
				iVExpenseEntryModel.MAmountFor = initBalance.MInitBalanceFor;
				iVExpenseEntryModel.MExchangeRate = num;
				iVExpenseEntryModel.MDesc = text;
				if (flag)
				{
					iVExpenseModel.ExpenseEntry.Add(iVExpenseEntryModel);
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVExpenseModel>(ctx, iVExpenseModel, null, true));
			}
			if (initBalance.MCurrencyID != ctx.MBasCurrencyID)
			{
				BDExchangeRateFilterModel bDExchangeRateFilterModel = new BDExchangeRateFilterModel();
				bDExchangeRateFilterModel.Equal("MSourceCurrencyID", ctx.MBasCurrencyID);
				bDExchangeRateFilterModel.Equal("MTargetCurrencyID", initBalance.MCurrencyID);
				BDExchangeRateFilterModel bDExchangeRateFilterModel2 = bDExchangeRateFilterModel;
				dateTime = ctx.MBeginDate;
				dateTime = dateTime.AddDays(-1.0);
				bDExchangeRateFilterModel2.Equal("MRateDate", dateTime.ToString("yyyy-MM-dd"));
				List<BDExchangeRateModel> modelList = new BDExchangeRateRepository().GetModelList(ctx, bDExchangeRateFilterModel, false);
				bool flag2 = insertExchangeRateList.Exists(delegate(BDExchangeRateModel x)
				{
					int result;
					if (x.MSourceCurrencyID == ctx.MBasCurrencyID && x.MTargetCurrencyID == initBalance.MCurrencyID)
					{
						DateTime dateTime4 = x.MRateDate;
						string a = dateTime4.ToString("yyyy-MM-dd");
						dateTime4 = ctx.MBeginDate;
						dateTime4 = dateTime4.AddDays(-1.0);
						result = ((a == dateTime4.ToString("yyyy-MM-dd")) ? 1 : 0);
					}
					else
					{
						result = 0;
					}
					return (byte)result != 0;
				});
				if ((modelList == null || modelList.Count() == 0) && !flag2)
				{
					BDExchangeRateModel bDExchangeRateModel = new BDExchangeRateModel();
					bDExchangeRateModel.MUserRate = ((initBalance.MInitBalance == decimal.Zero) ? decimal.One : Math.Round(initBalance.MInitBalanceFor / initBalance.MInitBalance, 6));
					bDExchangeRateModel.MRate = num;
					bDExchangeRateModel.MSourceCurrencyID = ctx.MBasCurrencyID;
					bDExchangeRateModel.MTargetCurrencyID = initBalance.MCurrencyID;
					BDExchangeRateModel bDExchangeRateModel2 = bDExchangeRateModel;
					dateTime = ctx.MBeginDate;
					bDExchangeRateModel2.MRateDate = dateTime.AddDays(-1.0);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDExchangeRateModel>(ctx, bDExchangeRateModel, null, true));
					insertExchangeRateList.Add(bDExchangeRateModel);
				}
			}
			return list;
		}

		private List<CommandInfo> GetDeleteAutoCreateInitBillCmds(MContext ctx, GLInitBalanceModel initBalance)
		{
			ParamBase paramBase = new ParamBase();
			paramBase.KeyIDs = initBalance.MBillID;
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> deleteBillCmd = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVInvoiceModel>(ctx, paramBase, null);
			if (deleteBillCmd != null)
			{
				list.AddRange(deleteBillCmd);
			}
			List<CommandInfo> deleteBillCmd2 = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVReceiveModel>(ctx, paramBase, null);
			if (deleteBillCmd2 != null)
			{
				list.AddRange(deleteBillCmd2);
			}
			List<CommandInfo> deleteBillCmd3 = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVPaymentModel>(ctx, paramBase, null);
			if (deleteBillCmd3 != null)
			{
				list.AddRange(deleteBillCmd3);
			}
			List<CommandInfo> deleteBillCmd4 = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVExpenseModel>(ctx, paramBase, null);
			if (deleteBillCmd4 != null)
			{
				list.AddRange(deleteBillCmd4);
			}
			return list;
		}

		private decimal CalculateExchange(decimal baseAmount, decimal foreignAmount, decimal exchange)
		{
			if (Math.Round(foreignAmount * exchange, 2) == baseAmount || baseAmount == decimal.Zero || foreignAmount == decimal.Zero)
			{
				return exchange;
			}
			exchange = Math.Round(baseAmount / foreignAmount, 6);
			return exchange;
		}

		private string GetBillIdByInitBalance(MContext ctx, GLInitBalanceModel initBalance)
		{
			return string.Empty;
		}

		public List<CommandInfo> GetDeleteInitDocCmds(MContext ctx, GLInitBalanceModel initBalance)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (string.IsNullOrWhiteSpace(initBalance.MBillID) || string.IsNullOrWhiteSpace(initBalance.MBillType))
			{
				return list;
			}
			if (initBalance.MBillType == "Invoice_Sale" || initBalance.MBillType == "Invoice_Purchase")
			{
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVInvoiceModel>(ctx, initBalance.MBillID));
			}
			else if (initBalance.MBillType == "Pay_Purchase")
			{
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVPaymentModel>(ctx, initBalance.MBillID));
			}
			else if (initBalance.MBillType == "Receive_Sale")
			{
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVReceiveModel>(ctx, initBalance.MBillID));
			}
			else if (initBalance.MBillType == "Expense_Claims")
			{
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVExpenseModel>(ctx, initBalance.MBillID));
			}
			return list;
		}

		private string GetContactValue(GLInitBalanceModel initBalance)
		{
			string empty = string.Empty;
			string mContactTypeFromBill = initBalance.MContactTypeFromBill;
			if (mContactTypeFromBill == "Employees")
			{
				return initBalance.MCheckGroupValueModel.MEmployeeID;
			}
			return initBalance.MCheckGroupValueModel.MContactID;
		}

		public List<NameValueModel> GetInitDocumentData(MContext ctx, int type = 0)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			if (type == 1)
			{
				List<BDInitBillViewModel> initBillViewModelList = dal.GetInitBillViewModelList(ctx, new BDInitDocumentFilterModel
				{
					MTypeList = new List<int>
					{
						0,
						1,
						2,
						3,
						4
					}
				});
				decimal num = (from x in initBillViewModelList
				where x.MType.ToLower().IndexOf("invoice_sale") == 0
				select x).ToList().Sum((BDInitBillViewModel x) => x.MTaxTotalAmt);
				decimal num2 = (from x in initBillViewModelList
				where x.MType.ToLower().IndexOf("invoice_purchase") == 0
				select x).ToList().Sum((BDInitBillViewModel x) => x.MTaxTotalAmt);
				decimal num3 = (from x in initBillViewModelList
				where x.MType.ToLower().IndexOf("receive") == 0
				select x).Sum((BDInitBillViewModel x) => x.MTaxTotalAmt);
				decimal num4 = (from x in initBillViewModelList
				where x.MType.ToLower().IndexOf("pay") == 0
				select x).Sum((BDInitBillViewModel x) => x.MTaxTotalAmt);
				decimal num5 = (from x in initBillViewModelList
				where x.MType.ToLower().IndexOf("expense") == 0
				select x).Sum((BDInitBillViewModel x) => x.MTaxTotalAmt);
				list = new List<NameValueModel>
				{
					new NameValueModel
					{
						MName = "0",
						MValue = num.ToString()
					},
					new NameValueModel
					{
						MName = "1",
						MValue = num2.ToString()
					},
					new NameValueModel
					{
						MName = "2",
						MValue = num3.ToString()
					},
					new NameValueModel
					{
						MName = "3",
						MValue = num4.ToString()
					},
					new NameValueModel
					{
						MName = "4",
						MValue = num5.ToString()
					}
				};
			}
			else
			{
				GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
				SqlWhere filter = BuilderFilter("1122");
				List<GLInitBalanceModel> initBalanceModels = gLInitBalanceRepository.GetInitBalanceModels(ctx, filter);
				list.Add(new NameValueModel
				{
					MName = "1122",
					MValue = ConvertBalance2String(initBalanceModels)
				});
				filter = BuilderFilter("2203");
				initBalanceModels = gLInitBalanceRepository.GetInitBalanceModels(ctx, filter);
				list.Add(new NameValueModel
				{
					MName = "2203",
					MValue = ConvertBalance2String(initBalanceModels)
				});
				filter = BuilderFilter("1221");
				initBalanceModels = gLInitBalanceRepository.GetInitBalanceModels(ctx, filter);
				list.Add(new NameValueModel
				{
					MName = "1221",
					MValue = ConvertBalance2String(initBalanceModels)
				});
				filter = BuilderFilter("2202");
				initBalanceModels = gLInitBalanceRepository.GetInitBalanceModels(ctx, filter);
				list.Add(new NameValueModel
				{
					MName = "2202",
					MValue = ConvertBalance2String(initBalanceModels)
				});
				filter = BuilderFilter("1123");
				initBalanceModels = gLInitBalanceRepository.GetInitBalanceModels(ctx, filter);
				list.Add(new NameValueModel
				{
					MName = "1123",
					MValue = ConvertBalance2String(initBalanceModels)
				});
				filter = BuilderFilter("2241");
				initBalanceModels = gLInitBalanceRepository.GetInitBalanceModels(ctx, filter);
				list.Add(new NameValueModel
				{
					MName = "2241",
					MValue = ConvertBalance2String(initBalanceModels)
				});
			}
			return list;
		}

		private SqlWhere BuilderFilter(string accountCode)
		{
			return new SqlWhere().AddFilter("MCode", SqlOperators.Equal, accountCode).AddFilter("MCheckGroupValueID", SqlOperators.Equal, "0").AddFilter("a.MIsActive", SqlOperators.Equal, 1)
				.AddFilter("a.MIsDelete", SqlOperators.Equal, 0);
		}

		private void UpdateInvoiceRedType(List<IVInvoiceModel> invoices)
		{
			int num = 0;
			while (invoices != null && num < invoices.Count)
			{
				if (invoices[num].MType == "Invoice_Purchase_Red" || invoices[num].MType == "Invoice_Sale_Red")
				{
					invoices[num].MTaxTotalAmt = -1.0m * invoices[num].MTaxTotalAmt;
					invoices[num].MTaxTotalAmtFor = -1.0m * invoices[num].MTaxTotalAmtFor;
					invoices[num].MTotalAmt = -1.0m * invoices[num].MTotalAmt;
					invoices[num].MTotalAmtFor = -1.0m * invoices[num].MTotalAmtFor;
				}
				invoices[num].InvoiceEntry.ForEach(delegate(IVInvoiceEntryModel x)
				{
					x.MAmount = -1.0m * x.MAmount;
					x.MAmountFor = -1.0m * x.MAmountFor;
					x.MTaxAmount = -1.0m * x.MTaxAmount;
					x.MTaxAmountFor = -1.0m * x.MTaxAmountFor;
				});
				num++;
			}
		}

		private string ConvertBalance2String(List<GLInitBalanceModel> list)
		{
			if (list == null || list.Count == 0)
			{
				return "0.00,0.00,0.00";
			}
			decimal num = list.Sum((GLInitBalanceModel x) => x.MInitBalance);
			decimal num2 = list.Sum((GLInitBalanceModel x) => x.MYtdCredit);
			decimal num3 = list.Sum((GLInitBalanceModel x) => x.MYtdDebit);
			return num.ToString() + "," + num3 + "," + num2;
		}

		public BDInitDocumentViewModel GetInitDocumentList(MContext ctx, BDInitDocumentFilterModel query)
		{
			if (!string.IsNullOrWhiteSpace(query.MCurrentAccountCode) && string.IsNullOrWhiteSpace(query.MAccountID))
			{
				BDAccountModel bDAccountModel = (from x in new BDAccountBusiness().GetAccountListWithCheckType(ctx, null, false)
				orderby x.MNumber
				where x.MCode.IndexOf(query.MCurrentAccountCode) == 0
				select x).FirstOrDefault() ?? new BDAccountModel();
				query.MAccountID = bDAccountModel.MItemID;
				query.MCurrentAccountCode = bDAccountModel.MCode;
			}
			BDInitDocumentViewModel bDInitDocumentViewModel = new BDInitDocumentViewModel
			{
				InitBillList = new List<BDInitBillViewModel>()
			};
			GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
			List<BDInitBillViewModel> list = bDInitDocumentViewModel.InitBillList = dal.GetInitBillViewModelList(ctx, query);
			if (!string.IsNullOrWhiteSpace(query.MAccountID))
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MAccountID", query.MAccountID);
				sqlWhere.Equal("MCheckGroupValueID", (query.MCheckGroupValueModel == null) ? "0" : utility.GetCheckGroupValueModel(ctx, query.MCheckGroupValueModel).MItemID);
				List<GLInitBalanceModel> initBalance = gLInitBalanceRepository.GetInitBalanceModels(ctx, sqlWhere);
				List<InitBillAmountFor> list2 = new List<InitBillAmountFor>();
				if (initBalance == null || initBalance.Count == 0)
				{
					list2.Add(new InitBillAmountFor
					{
						MCyID = ctx.MBasCurrencyID
					});
				}
				else
				{
					int i;
					for (i = 0; i < initBalance.Count; i++)
					{
						if ((from x in list2
						where x.MCyID == initBalance[i].MCurrencyID
						select x).Count() > 0)
						{
							list2.ForEach(delegate(InitBillAmountFor x)
							{
								if (x.MCyID == initBalance[i].MCurrencyID)
								{
									x.MInitBalance += initBalance[i].MInitBalance;
									x.MInitBalanceFor += initBalance[i].MInitBalanceFor;
								}
							});
						}
						else
						{
							list2.Add(new InitBillAmountFor
							{
								MCyID = initBalance[i].MCurrencyID,
								MInitBalance = initBalance[i].MInitBalance,
								MInitBalanceFor = initBalance[i].MInitBalanceFor
							});
						}
					}
				}
				bDInitDocumentViewModel.InitBillAmountFor = list2;
			}
			return bDInitDocumentViewModel;
		}

		[Obsolete]
		private void HandleBillVerificationAmt(MContext ctx, List<BDInitBillViewModel> billList)
		{
			List<BDInitBillViewModel> list = new List<BDInitBillViewModel>();
			for (int i = 0; i < billList.Count; i++)
			{
				BDInitBillViewModel bDInitBillViewModel = billList[i];
				if (Math.Abs(bDInitBillViewModel.MVerifyAmt) > decimal.Zero)
				{
					IVVerificationListFilterModel filter = new IVVerificationListFilterModel
					{
						MBillID = bDInitBillViewModel.MID,
						MBizBillType = bDInitBillViewModel.MType.Substring(0, (bDInitBillViewModel.MType.IndexOf("_") > 0) ? bDInitBillViewModel.MType.IndexOf("_") : bDInitBillViewModel.MType.Length),
						MViewVerif = true
					};
					List<IVVerificationListModel> historyVerifData = IVVerificationRepository.GetHistoryVerifData(ctx, filter);
					IEnumerable<IVVerificationListModel> source = from x in historyVerifData
					where x.MBizDate < ctx.MGLBeginDate
					select x;
					Func<IVVerificationListModel, decimal> selector = (IVVerificationListModel x) => Math.Abs(x.MAmountTotalFor);
					decimal num2 = bDInitBillViewModel.MVerifyAmtFor = source.Sum(selector);
					if (!(Math.Abs(bDInitBillViewModel.MTaxTotalAmtFor) <= Math.Abs(bDInitBillViewModel.MVerifyAmt)))
					{
						bDInitBillViewModel.MExchangeRate = ((bDInitBillViewModel.MExchangeRate == decimal.Zero) ? 1.0m : bDInitBillViewModel.MExchangeRate);
						bDInitBillViewModel.MTaxTotalAmt -= bDInitBillViewModel.MVerifyAmt;
						bDInitBillViewModel.MTaxTotalAmtFor -= bDInitBillViewModel.MVerifyAmtFor;
						goto IL_0189;
					}
					continue;
				}
				goto IL_0189;
				IL_0189:
				list.Add(bDInitBillViewModel);
			}
		}

		private void HandleAmountPricise(BDInitDocumentViewModel model)
		{
			if (model.InitBillList != null && model.InitBillList.Count > 0)
			{
				model.InitBillList.ForEach(delegate(BDInitBillViewModel x)
				{
					x.MTaxTotalAmt = Math.Round(x.MTaxTotalAmt, 2, MidpointRounding.AwayFromZero);
					x.MTaxTotalAmtFor = Math.Round(x.MTaxTotalAmtFor, 2, MidpointRounding.AwayFromZero);
					x.MCheckGroupValueModel = (x.MCheckGroupValueModel ?? new GLCheckGroupValueModel());
				});
			}
		}

		public OperationResult SaveInitDocumentModel(MContext ctx, BDInitDocumentViewModel model)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			List<CommandInfo> list = new List<CommandInfo>();
			HandleAmountPricise(model);
			if (model.InitBillList != null && model.InitBillList.Count > 0)
			{
				ValidateInitDocumentModel(ctx, model);
				list.AddRange(GetInsertOrUpdateBillCmds(ctx, model, operationResult));
				operationResult.Success = (list.Count == 0 || BDRepository.ExecuteSqlTran(ctx, list) > 0);
			}
			return operationResult;
		}

		private string GetBillTypeName(MContext ctx, string mtype)
		{
			switch (mtype)
			{
			case "Invoice_Sale":
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Invoice", "销售单");
			case "Invoice_Sale_Red":
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Invoice_Sale_Red", "红字销售单");
			case "Invoice_Purchase":
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Invoice_Purchase", "采购单");
			case "Invoice_Purchase_Red":
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Invoice_Purchase_Red", "红字采购单");
			default:
				return string.Empty;
			}
		}

		private List<CommandInfo> GetInsertOrUpdateBillCmds(MContext ctx, BDInitDocumentViewModel model, OperationResult result)
		{
			model.MBillInvoiceList = new List<IVInvoiceModel>();
			model.MSaleInvoiceList = new List<IVInvoiceModel>();
			model.MPaymentList = new List<IVPaymentModel>();
			model.MReceiveList = new List<IVReceiveModel>();
			model.MExpenseList = new List<IVExpenseModel>();
			List<CommandInfo> list = new List<CommandInfo>();
			List<BDInitBillViewModel> initBillList = model.InitBillList;
			List<BDInitBillViewModel> list2 = (from x in initBillList
			where string.IsNullOrWhiteSpace(x.MID)
			select x).ToList();
			foreach (BDInitBillViewModel item in list2)
			{
				if (item.MCyID == ctx.MBasCurrencyID)
				{
					item.MTaxTotalAmtFor = item.MTaxTotalAmt;
				}
				switch (item.MType)
				{
				case "Invoice_Sale":
				case "Invoice_Sale_Red":
				{
					IVInvoiceModel invoiceUpdateModel2 = GetInvoiceUpdateModel(ctx, item, (item.MType == "Invoice_Sale_Red" || item.MType == "Invoice_Purchase_Red") ? -1.0m : 1.0m);
					model.MSaleInvoiceList.Add(invoiceUpdateModel2);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVInvoiceModel>(ctx, invoiceUpdateModel2, null, true));
					list.AddRange(IVInvoiceLogRepository.GetAddInvoiceEditLogCmd(ctx, invoiceUpdateModel2));
					break;
				}
				case "Invoice_Purchase":
				case "Invoice_Purchase_Red":
				{
					IVInvoiceModel invoiceUpdateModel = GetInvoiceUpdateModel(ctx, item, (item.MType == "Invoice_Sale_Red" || item.MType == "Invoice_Purchase_Red") ? -1.0m : 1.0m);
					model.MBillInvoiceList.Add(invoiceUpdateModel);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVInvoiceModel>(ctx, invoiceUpdateModel, null, true));
					list.AddRange(IVInvoiceLogRepository.GetAddInvoiceEditLogCmd(ctx, invoiceUpdateModel));
					break;
				}
				case "Receive_Sale":
				{
					IVReceiveModel receiveUpdateModel = GetReceiveUpdateModel(ctx, item);
					model.MReceiveList.Add(receiveUpdateModel);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVReceiveModel>(ctx, receiveUpdateModel, null, true));
					break;
				}
				case "Pay_Purchase":
				{
					IVPaymentModel paymentUpdateModel = GetPaymentUpdateModel(ctx, item);
					model.MPaymentList.Add(paymentUpdateModel);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, paymentUpdateModel, null, true));
					break;
				}
				case "Expense_Claims":
				{
					IVExpenseModel expenseUpdateModel = GetExpenseUpdateModel(ctx, item);
					model.MExpenseList.Add(expenseUpdateModel);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVExpenseModel>(ctx, expenseUpdateModel, null, true));
					list.AddRange(IVExpenseLogHelper.GetSaveLogCmd(ctx, expenseUpdateModel));
					break;
				}
				}
			}
			return list;
		}

		private string GetItemDesc(MContext ctx, BDInitBillViewModel billModel, bool isEmployee = false)
		{
			string text = isEmployee ? GLUtility.GetExpItemDesc(ctx, billModel.MCheckGroupValueModel.MExpItemID) : GLUtility.GetMerItemDesc(ctx, billModel.MCheckGroupValueModel.MMerItemID);
			return string.IsNullOrWhiteSpace(text) ? billModel.MReference : text;
		}

		private IVInvoiceModel GetInvoiceUpdateModel(MContext ctx, BDInitBillViewModel billModel, decimal dir = default(decimal))
		{
			IVInvoiceModel iVInvoiceModel = new IVInvoiceModel
			{
				MTaxTotalAmt = billModel.MTaxTotalAmt * dir,
				MTaxTotalAmtFor = billModel.MTaxTotalAmtFor * dir,
				MTotalAmt = billModel.MTaxTotalAmt * dir,
				MTotalAmtFor = billModel.MTaxTotalAmtFor * dir,
				MContactID = billModel.MCheckGroupValueModel.MContactID,
				MBizDate = billModel.MBizDate,
				MType = billModel.MType,
				MCyID = billModel.MCyID,
				MDueDate = billModel.MDueDate,
				MOrgID = ctx.MOrgID,
				MNumber = billModel.MNumber,
				MTaxID = "No_Tax",
				MReference = billModel.MReference,
				MCurrentAccountCode = billModel.MCurrentAccountCode,
				MExchangeRate = Math.Round(billModel.MTaxTotalAmt / billModel.MTaxTotalAmtFor * 1.0m, 6),
				MStatus = 3,
				InvoiceEntry = new List<IVInvoiceEntryModel>(),
				IsNew = true,
				MIsAutoAmount = false
			};
			IVInvoiceEntryModel item = new IVInvoiceEntryModel
			{
				MDesc = GetItemDesc(ctx, billModel, false),
				MAmount = billModel.MTaxTotalAmt * dir,
				MTaxAmountFor = billModel.MTaxTotalAmtFor * dir,
				MAmountFor = billModel.MTaxTotalAmtFor * dir,
				MTaxAmount = billModel.MTaxTotalAmt * dir,
				MExchangeRate = Math.Round(billModel.MTaxTotalAmt / billModel.MTaxTotalAmtFor * 1.0m, 6),
				MTaxAmt = decimal.Zero,
				MTaxAmtFor = decimal.Zero,
				MQty = decimal.One * dir,
				MPrice = billModel.MTaxTotalAmtFor,
				IsNew = true,
				MItemID = billModel.MCheckGroupValueModel.MMerItemID,
				MTrackItem1 = billModel.MCheckGroupValueModel.MTrackItem1,
				MTrackItem2 = billModel.MCheckGroupValueModel.MTrackItem2,
				MTrackItem3 = billModel.MCheckGroupValueModel.MTrackItem3,
				MTrackItem4 = billModel.MCheckGroupValueModel.MTrackItem4,
				MTrackItem5 = billModel.MCheckGroupValueModel.MTrackItem5
			};
			iVInvoiceModel.InvoiceEntry.Add(item);
			return iVInvoiceModel;
		}

		private string getContactType(int contactType)
		{
			switch (contactType)
			{
			case 0:
				return "Customer";
			case 1:
				return "Supplier";
			case 2:
				return "Other";
			default:
				return "Employees";
			}
		}

		private IVReceiveModel GetReceiveUpdateModel(MContext ctx, BDInitBillViewModel billModel)
		{
			bool flag = !string.IsNullOrWhiteSpace(billModel.MCheckGroupValueModel.MEmployeeID);
			string contactType = getContactType(flag ? 4 : billModel.MCheckGroupValueModel.MContactType);
			IVReceiveModel iVReceiveModel = new IVReceiveModel
			{
				MTaxTotalAmt = billModel.MTaxTotalAmt,
				MTaxTotalAmtFor = billModel.MTaxTotalAmtFor,
				MTotalAmt = billModel.MTaxTotalAmt,
				MTotalAmtFor = billModel.MTaxTotalAmtFor,
				MContactID = (flag ? billModel.MCheckGroupValueModel.MEmployeeID : billModel.MCheckGroupValueModel.MContactID),
				MBizDate = billModel.MBizDate,
				MType = (flag ? "Receive_Other" : billModel.MType),
				MCyID = billModel.MCyID,
				MOrgID = ctx.MOrgID,
				MNumber = billModel.MNumber,
				MReference = billModel.MReference,
				MExchangeRate = Math.Round(billModel.MTaxTotalAmt / billModel.MTaxTotalAmtFor * 1.0m, 6),
				MContactType = billModel.MContactType,
				IsNew = true,
				MTaxID = "No_Tax",
				MCurrentAccountCode = billModel.MCurrentAccountCode,
				MBankID = billModel.MBankID,
				MIsAutoAmount = false,
				ReceiveEntry = new List<IVReceiveEntryModel>(),
				MReconcileStatu = 201
			};
			IVReceiveEntryModel item = new IVReceiveEntryModel
			{
				MAmount = iVReceiveModel.MTaxTotalAmt,
				MDesc = GetItemDesc(ctx, billModel, false),
				MTaxAmountFor = iVReceiveModel.MTaxTotalAmtFor,
				MAmountFor = iVReceiveModel.MTaxTotalAmtFor,
				MTaxAmount = iVReceiveModel.MTaxTotalAmt,
				MTaxAmt = decimal.Zero,
				MTaxAmtFor = decimal.Zero,
				MQty = decimal.One,
				MPrice = iVReceiveModel.MTaxTotalAmtFor,
				MExchangeRate = Math.Round(billModel.MTaxTotalAmt / billModel.MTaxTotalAmtFor * 1.0m, 6),
				IsNew = true,
				MItemID = billModel.MCheckGroupValueModel.MMerItemID,
				MTrackItem1 = billModel.MCheckGroupValueModel.MTrackItem1,
				MTrackItem2 = billModel.MCheckGroupValueModel.MTrackItem2,
				MTrackItem3 = billModel.MCheckGroupValueModel.MTrackItem3,
				MTrackItem4 = billModel.MCheckGroupValueModel.MTrackItem4,
				MTrackItem5 = billModel.MCheckGroupValueModel.MTrackItem5
			};
			iVReceiveModel.ReceiveEntry.Add(item);
			return iVReceiveModel;
		}

		private IVPaymentModel GetPaymentUpdateModel(MContext ctx, BDInitBillViewModel billModel)
		{
			bool flag = !string.IsNullOrWhiteSpace(billModel.MCheckGroupValueModel.MEmployeeID);
			string mContactType = flag ? "Employees" : billModel.MContactType;
			IVPaymentModel iVPaymentModel = new IVPaymentModel
			{
				MTaxTotalAmt = billModel.MTaxTotalAmt,
				MTaxTotalAmtFor = billModel.MTaxTotalAmtFor,
				MTotalAmt = billModel.MTaxTotalAmt,
				MTotalAmtFor = billModel.MTaxTotalAmtFor,
				MContactID = (flag ? billModel.MCheckGroupValueModel.MEmployeeID : billModel.MCheckGroupValueModel.MContactID),
				MBizDate = billModel.MBizDate,
				MType = (flag ? "Pay_Other" : billModel.MType),
				MCyID = billModel.MCyID,
				MOrgID = ctx.MOrgID,
				MNumber = billModel.MNumber,
				MReference = billModel.MReference,
				MExchangeRate = Math.Round(billModel.MTaxTotalAmt / billModel.MTaxTotalAmtFor * 1.0m, 6),
				MContactType = mContactType,
				IsNew = true,
				MTaxID = "No_Tax",
				MCurrentAccountCode = billModel.MCurrentAccountCode,
				MBankID = billModel.MBankID,
				MIsAutoAmount = false,
				PaymentEntry = new List<IVPaymentEntryModel>(),
				MReconcileStatu = 201
			};
			IVPaymentEntryModel item = new IVPaymentEntryModel
			{
				MDesc = GetItemDesc(ctx, billModel, flag),
				MAmount = iVPaymentModel.MTaxTotalAmt,
				MTaxAmountFor = iVPaymentModel.MTaxTotalAmtFor,
				MAmountFor = iVPaymentModel.MTaxTotalAmtFor,
				MTaxAmount = iVPaymentModel.MTaxTotalAmt,
				MTaxAmt = decimal.Zero,
				MTaxAmtFor = decimal.Zero,
				MQty = decimal.One,
				MPrice = iVPaymentModel.MTaxTotalAmtFor,
				IsNew = true,
				MItemID = (flag ? billModel.MCheckGroupValueModel.MExpItemID : billModel.MCheckGroupValueModel.MMerItemID),
				MTrackItem1 = billModel.MCheckGroupValueModel.MTrackItem1,
				MTrackItem2 = billModel.MCheckGroupValueModel.MTrackItem2,
				MTrackItem3 = billModel.MCheckGroupValueModel.MTrackItem3,
				MTrackItem4 = billModel.MCheckGroupValueModel.MTrackItem4,
				MTrackItem5 = billModel.MCheckGroupValueModel.MTrackItem5,
				MExchangeRate = Math.Round(billModel.MTaxTotalAmt / billModel.MTaxTotalAmtFor * 1.0m, 6)
			};
			iVPaymentModel.PaymentEntry.Add(item);
			return iVPaymentModel;
		}

		private IVExpenseModel GetExpenseUpdateModel(MContext ctx, BDInitBillViewModel billModel)
		{
			IVExpenseModel iVExpenseModel = new IVExpenseModel
			{
				MTaxTotalAmt = billModel.MTaxTotalAmt,
				MTaxTotalAmtFor = billModel.MTaxTotalAmtFor,
				MTotalAmt = billModel.MTaxTotalAmt,
				MTotalAmtFor = billModel.MTaxTotalAmtFor,
				MContactID = billModel.MCheckGroupValueModel.MEmployeeID,
				MEmployee = billModel.MCheckGroupValueModel.MEmployeeID,
				MBizDate = billModel.MBizDate,
				MDueDate = billModel.MDueDate,
				MType = billModel.MType,
				MCyID = billModel.MCyID,
				MOrgID = ctx.MOrgID,
				MTaxID = "No_Tax",
				MNumber = billModel.MNumber,
				MReference = billModel.MReference,
				MExchangeRate = Math.Round(billModel.MTaxTotalAmt / billModel.MTaxTotalAmtFor * 1.0m, 6),
				MStatus = 3,
				IsNew = true,
				MCurrentAccountCode = billModel.MCurrentAccountCode,
				MIsAutoAmount = false,
				ExpenseEntry = new List<IVExpenseEntryModel>()
			};
			IVExpenseEntryModel item = new IVExpenseEntryModel
			{
				MDesc = GetItemDesc(ctx, billModel, false),
				MAmount = iVExpenseModel.MTaxTotalAmt,
				MTaxAmountFor = iVExpenseModel.MTaxTotalAmtFor,
				MAmountFor = iVExpenseModel.MTaxTotalAmtFor,
				MTaxAmount = iVExpenseModel.MTaxTotalAmt,
				MTaxAmt = decimal.Zero,
				MTaxAmtFor = decimal.Zero,
				MQty = decimal.One,
				MPrice = iVExpenseModel.MTaxTotalAmtFor,
				IsNew = true,
				MItemID = billModel.MCheckGroupValueModel.MExpItemID,
				MTrackItem1 = billModel.MCheckGroupValueModel.MTrackItem1,
				MTrackItem2 = billModel.MCheckGroupValueModel.MTrackItem2,
				MTrackItem3 = billModel.MCheckGroupValueModel.MTrackItem3,
				MTrackItem4 = billModel.MCheckGroupValueModel.MTrackItem4,
				MTrackItem5 = billModel.MCheckGroupValueModel.MTrackItem5,
				MExchangeRate = Math.Round(billModel.MTaxTotalAmt / billModel.MTaxTotalAmtFor * 1.0m, 6)
			};
			iVExpenseModel.ExpenseEntry.Add(item);
			return iVExpenseModel;
		}

		public OperationResult CheckInitBalanceEqualWithBill(MContext ctx)
		{
			return dal.CheckInitBalanceEqualWithBill(ctx);
		}

		public void SetBankCurrencyData(MContext ctx, BDInitDocumentModel doc)
		{
			List<BDBankAccountEditModel> bankList = new BDBankAccountBusiness().GetBDBankAccountEditList(ctx, ctx.MBeginDate, ctx.DateNow, null, false, false, false);
			List<REGCurrencyViewModel> currencyList = new REGCurrencyBusiness().GetCurrencyViewList(ctx, null, true);
			if (doc.MReceiveList != null && doc.MReceiveList.Count > 0)
			{
				doc.MReceiveList.ForEach(delegate(IVReceiveModel x)
				{
					x.MCyName = (string.IsNullOrWhiteSpace(x.MCyID) ? "" : currencyList.FirstOrDefault((REGCurrencyViewModel y) => y.MCurrencyID == x.MCyID).MName);
					x.MBankName = (string.IsNullOrWhiteSpace(x.MBankID) ? "" : bankList.FirstOrDefault((BDBankAccountEditModel y) => y.MItemID == x.MBankID).MBankName);
				});
			}
			if (doc.MPaymentList != null && doc.MPaymentList.Count > 0)
			{
				doc.MPaymentList.ForEach(delegate(IVPaymentModel x)
				{
					x.MCyName = (string.IsNullOrWhiteSpace(x.MCyID) ? "" : currencyList.FirstOrDefault((REGCurrencyViewModel y) => y.MCurrencyID == x.MCyID).MName);
					x.MBankName = (string.IsNullOrWhiteSpace(x.MBankID) ? "" : bankList.FirstOrDefault((BDBankAccountEditModel y) => y.MItemID == x.MBankID).MBankName);
				});
			}
			if (doc.MBillInvoiceList != null && doc.MBillInvoiceList.Count > 0)
			{
				doc.MBillInvoiceList.ForEach(delegate(IVInvoiceModel x)
				{
					x.MCyName = (string.IsNullOrWhiteSpace(x.MCyID) ? "" : currencyList.FirstOrDefault((REGCurrencyViewModel y) => y.MCurrencyID == x.MCyID).MName);
				});
			}
			if (doc.MSaleInvoiceList != null && doc.MSaleInvoiceList.Count > 0)
			{
				doc.MSaleInvoiceList.ForEach(delegate(IVInvoiceModel x)
				{
					x.MCyName = (string.IsNullOrWhiteSpace(x.MCyID) ? "" : currencyList.FirstOrDefault((REGCurrencyViewModel y) => y.MCurrencyID == x.MCyID).MName);
				});
			}
			if (doc.MExpenseList != null && doc.MExpenseList.Count > 0)
			{
				doc.MExpenseList.ForEach(delegate(IVExpenseModel x)
				{
					x.MCyName = (string.IsNullOrWhiteSpace(x.MCyID) ? "" : currencyList.FirstOrDefault((REGCurrencyViewModel y) => y.MCurrencyID == x.MCyID).MName);
				});
			}
		}

		public OperationResult UpdateDocCurrentAccountCode(MContext ctx, string docType, string docId, string accountCode)
		{
			return dal.UpdateDocCurrentAccountCode(ctx, docType, docId, accountCode);
		}

		public OperationResult CheckIsExistInitBill(MContext ctx)
		{
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			if (ctx.MGLBeginDate == DateTime.MinValue)
			{
				throw new Exception("CheckIsExistInitBill : MGLBeginDate is null");
			}
			return CheckIsExistInitBill(ctx);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDInitDocumentModel modelData, string fields = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDInitDocumentModel> modelData, string fields = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			throw new NotImplementedException();
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			throw new NotImplementedException();
		}

		public BDInitDocumentModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			throw new NotImplementedException();
		}

		public BDInitDocumentModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public List<BDInitDocumentModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<BDInitDocumentModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}
	}
}
