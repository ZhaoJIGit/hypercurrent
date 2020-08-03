using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Utility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.IO
{
	public class ExpenseImport : IVImportBase, IImport
	{
		private List<CommandInfo> cmdList = new List<CommandInfo>();

		private List<BDAccountListModel> acctList = null;

		public virtual ImportResult ImportData(MContext ctx, IOImportDataModel data)
		{
			ImportResult importResult = new ImportResult();
			List<IVExpenseModel> list = new List<IVExpenseModel>();
			List<IVBatchPayHeadModel> list2 = new List<IVBatchPayHeadModel>();
			List<IOValidationResultModel> validationResult = new List<IOValidationResultModel>();
			List<IOExpenseImportModel> list3 = new List<IOExpenseImportModel>();
			try
			{
				list3 = base.ConvertToList<IOExpenseImportModel>(data.EffectiveData);
				int num = list3.Count((IOExpenseImportModel f) => f.MPrice == decimal.Zero);
				list3 = (from f in list3
				where f.MPrice != decimal.Zero
				select f).ToList();
				if (!list3.Any())
				{
					string item = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ImportTmplExistZeroRecords", "导入模版中有{0}条交易为0的记录，无法对这些记录进行导入！"), num);
					importResult.MessageList = new List<string>
					{
						item
					};
					return importResult;
				}
				List<IOExpenseImportModel> mainExpenseList = GetMainExpenseList(list3);
				base.SetCacheData(ctx);
				ValidateBasicData(ctx, mainExpenseList, list3, data, validationResult);
				foreach (IOExpenseImportModel item2 in mainExpenseList)
				{
					IVExpenseModel expenseModel = GetExpenseModel(ctx, item2, list3, list2, validationResult);
					list.Add(expenseModel);
				}
				base.SetValidationResult(ctx, importResult, validationResult, false);
				if (!importResult.Success)
				{
					return importResult;
				}
				cmdList.AddRange(IVExpenseRepository.GetImportExpenseCmdList(ctx, list));
				ImportResult importResult2 = ResultHelper.ToImportResult(list);
				if (!importResult2.Success)
				{
					return importResult2;
				}
				if (list2.Any())
				{
					string empty = string.Empty;
					foreach (IVBatchPayHeadModel item3 in list2)
					{
						foreach (IVBatchPaymentModel item4 in item3.PaymentEntry)
						{
							item4.MObject = list.FirstOrDefault((IVExpenseModel f) => f.MID == item4.MID);
						}
					}
					cmdList.AddRange(IVVerificationRepository.GetBatchPaymentCmdList(ctx, list2, ref empty));
					if (!string.IsNullOrWhiteSpace(empty))
					{
						importResult.MessageList = new List<string>
						{
							empty
						};
						return importResult;
					}
				}
			}
			catch (MConvertException ex2)
			{
				MConvertException ex;
				MConvertException ex3 = ex = ex2;
				List<string> list4 = new List<string>();
				if (ex.RowIndex > 0)
				{
					list4.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ErrorRowNo", "Row {0}: "), ex.RowIndex + 4));
				}
				IVExpenseBusiness iVExpenseBusiness = new IVExpenseBusiness();
				List<IOTemplateConfigModel> templateConfig = iVExpenseBusiness.GetTemplateConfig(ctx);
				IOTemplateConfigModel iOTemplateConfigModel = templateConfig.FirstOrDefault((IOTemplateConfigModel f) => f.MFieldName == ex.FieldName);
				if (iOTemplateConfigModel?.MLangList.ContainsKey(ctx.MLCID) ?? false)
				{
					ex.FieldName = iOTemplateConfigModel.MLangList[ctx.MLCID];
				}
				else
				{
					ex.FieldName = ex.FieldName.TrimStart('M');
				}
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataConvertFail", "该字段（{0}）的数据（{1}）转换失败！");
				list4.Add(string.Format(text, ex.FieldName, ex.FieldValue));
				importResult.MessageList.Add(string.Join("", list4));
				return importResult;
			}
			catch (Exception ex4)
			{
				throw new Exception(ex4.Message);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			importResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(cmdList) > 0);
			importResult.SuccessRowIndexes = string.Join(",", from f in list3
			select f.MRowIndex);
			return importResult;
		}

		protected void ValidateBasicData(MContext ctx, List<IOExpenseImportModel> mainList, List<IOExpenseImportModel> list, IOImportDataModel data, List<IOValidationResultModel> validationResult)
		{
			List<IOExpenseImportModel> list2 = (from f in mainList
			where f.MDueDate < f.MBizDate
			select f).ToList();
			if (list2.Any())
			{
				string msg = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DueDateMustGreaterOrEqualDate", "Due date must be greater than or equal to date!");
				list2.ForEach(delegate(IOExpenseImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.DueDate,
						RowIndex = f.MRowIndex,
						Message = msg
					});
				});
			}
			List<IOExpenseImportModel> list3 = (from f in mainList
			where f.MExpectedDate != DateTime.MinValue && f.MExpectedDate < f.MBizDate
			select f).ToList();
			if (list3.Any())
			{
				string msg2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ExpectedDateIsGreaterThanTheDate", "Expected Payment date must be greater than or equal to date!");
				list3.ForEach(delegate(IOExpenseImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.ExpectedDate,
						RowIndex = f.MRowIndex,
						Message = msg2
					});
				});
			}
			base.ValidateBizDate(ctx, mainList, validationResult, true, "MBizDate");
			List<IOExpenseImportModel> list4 = (from f in list
			where f.MPrice < decimal.Zero
			select f).ToList();
			if (list4.Any())
			{
				string msg3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "UnitPriceIsNegative", "单价必须大于0！");
				list4.ForEach(delegate(IOExpenseImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Price,
						RowIndex = f.MRowIndex,
						Message = msg3
					});
				});
			}
			List<IOExpenseImportModel> list5 = (from f in list
			where f.MTaxAmtFor < decimal.Zero
			select f).ToList();
			if (list5.Any())
			{
				string msg4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TaxAmountIsRequire", "税额必须大于等于0！");
				list5.ForEach(delegate(IOExpenseImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Price,
						RowIndex = f.MRowIndex,
						Message = msg4
					});
				});
			}
			base.CheckEmployeeExist(ctx, mainList, "MEmployee", validationResult);
			base.CheckCurrencyExist(ctx, mainList, "MCyID", validationResult);
			base.CheckCurrencyExchangeRateExist(ctx, mainList, validationResult, base.ForeignCurrencyList, "MCyID", "MBizDate");
			base.CheckExpenseItemExist(ctx, list, ref validationResult);
			base.CheckBankAccountExist(ctx, list, "MBankAccount", validationResult);
			base.CheckTrackExist(ctx, data.TrackItemNameList, list, validationResult);
			acctList = base.GetAccountList(ctx);
			foreach (IOExpenseImportModel item in list)
			{
				base.CheckAccountExist(ctx, item, acctList, "MDebitAccount", validationResult);
				base.CheckAccountExist(ctx, item, acctList, "MCreditAccount", validationResult);
				base.CheckAccountExist(ctx, item, acctList, "MTaxAccount", validationResult);
			}
		}

		private IVExpenseModel GetExpenseModel(MContext ctx, IOExpenseImportModel mainModel, List<IOExpenseImportModel> list, List<IVBatchPayHeadModel> paymentList, List<IOValidationResultModel> validationResult)
		{
			List<IOExpenseImportModel> list2 = (from t in list
			where t.GroupByID == mainModel.GroupByID
			select t).ToList();
			if (list2 == null || list2.Count == 0)
			{
				return null;
			}
			IVExpenseModel iVExpenseModel = new IVExpenseModel();
			iVExpenseModel.MID = UUIDHelper.GetGuid();
			iVExpenseModel.IsNew = true;
			iVExpenseModel.MEmployee = mainModel.MEmployee;
			iVExpenseModel.MContactID = mainModel.MEmployee;
			iVExpenseModel.MReference = mainModel.MReference;
			iVExpenseModel.MBizDate = mainModel.MBizDate;
			iVExpenseModel.MDueDate = mainModel.MDueDate;
			iVExpenseModel.MExpectedDate = mainModel.MExpectedDate;
			iVExpenseModel.MCyID = mainModel.MCyID;
			iVExpenseModel.MStatus = 1;
			iVExpenseModel.MRowIndex = mainModel.MRowIndex;
			iVExpenseModel.ExpenseEntry = GetExpenseEntryList(ctx, iVExpenseModel, list2, validationResult);
			iVExpenseModel.MTotalAmtFor = iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MAmountFor);
			iVExpenseModel.MTotalAmt = iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MAmount);
			iVExpenseModel.MTaxTotalAmtFor = iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MTaxAmountFor);
			iVExpenseModel.MTaxTotalAmt = iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MTaxAmount);
			if (mainModel.MPaymentDate != DateTime.MinValue && mainModel.MPaymentDate < mainModel.MBizDate)
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.PaymentDate,
					FieldValue = mainModel.MPaymentDate.ToOrgZoneDateString(null),
					RowIndex = mainModel.MRowIndex,
					Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "PaymentDateError", "支付日期：{{0}}不能早于单据日期：{0}!"), mainModel.MBizDate.ToOrgZoneDateString(null))
				});
			}
			decimal num = Math.Round(iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MQty * f.MPrice), 2);
			if (mainModel.MPaidAmount < decimal.Zero || mainModel.MPaidAmount > num)
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.PaidAmount,
					FieldValue = mainModel.MPaidAmount.ToString(),
					RowIndex = mainModel.MRowIndex,
					Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "PaidAmountError", "支付金额：{{0}}不能大于总金额：{0}!"), num)
				});
			}
			if (mainModel.MPaymentDate != DateTime.MinValue && mainModel.MPaidAmount > decimal.Zero && !string.IsNullOrWhiteSpace(list2[0].MBankAccount))
			{
				IVBatchPayHeadModel iVBatchPayHeadModel = new IVBatchPayHeadModel();
				iVBatchPayHeadModel.SelectObj = "Expense";
				iVBatchPayHeadModel.MPayDate = mainModel.MPaymentDate;
				iVBatchPayHeadModel.MPayBank = list2[0].MBankAccount;
				iVBatchPayHeadModel.PaymentEntry = new List<IVBatchPaymentModel>
				{
					new IVBatchPaymentModel
					{
						MID = iVExpenseModel.MID,
						MPayAmount = mainModel.MPaidAmount
					}
				};
				paymentList.Add(iVBatchPayHeadModel);
				iVExpenseModel.MStatus = 4;
			}
			List<IOExpenseImportModel> list3 = (from x in list2
			where x.MQty <= decimal.Zero
			select x).ToList();
			if (list3.Any())
			{
				string msg = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "QtyMustGreaterThanZere", "数量必须大于0！");
				list3.ForEach(delegate(IOExpenseImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Quantity,
						RowIndex = f.MRowIndex,
						Message = msg
					});
				});
			}
			if (iVExpenseModel.MTaxTotalAmtFor <= decimal.Zero)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TotalAmountNotToZero", "总金额必须大于零。");
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.TotalAmount,
					RowIndex = iVExpenseModel.MRowIndex,
					Message = text
				});
			}
			return iVExpenseModel;
		}

		protected IVExpenseModel GetExpenseModel(MContext ctx, IOExpenseImportModel mainModel, List<IOExpenseImportModel> list)
		{
			List<IOExpenseImportModel> list2 = (from t in list
			where t.GroupByID == mainModel.GroupByID
			select t).ToList();
			if (list2 == null || list2.Count == 0)
			{
				return null;
			}
			IVExpenseModel iVExpenseModel = new IVExpenseModel();
			iVExpenseModel.MID = UUIDHelper.GetGuid();
			iVExpenseModel.IsNew = true;
			iVExpenseModel.MEmployee = mainModel.MEmployee;
			iVExpenseModel.MContactID = mainModel.MEmployee;
			iVExpenseModel.MReference = mainModel.MReference;
			iVExpenseModel.MBizDate = mainModel.MBizDate;
			iVExpenseModel.MDueDate = mainModel.MDueDate;
			iVExpenseModel.MExpectedDate = mainModel.MExpectedDate;
			iVExpenseModel.MCyID = (string.IsNullOrWhiteSpace(mainModel.MCyID) ? ctx.MBasCurrencyID : mainModel.MCyID);
			if (iVExpenseModel.MCyID.Trim().IndexOf(ctx.MBasCurrencyID) == 0)
			{
				iVExpenseModel.MCyID = ctx.MBasCurrencyID;
			}
			iVExpenseModel.MStatus = 1;
			iVExpenseModel.MRowIndex = mainModel.MRowIndex;
			int num = 1;
			foreach (IOExpenseImportModel item in list2)
			{
				IVExpenseEntryModel iVExpenseEntryModel = ConvertToExpenseEntryModel(ctx, iVExpenseModel, item);
				iVExpenseEntryModel.MSeq = num;
				iVExpenseModel.AddEntry(iVExpenseEntryModel);
				num++;
			}
			iVExpenseModel.MTotalAmtFor = iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MAmountFor);
			iVExpenseModel.MTotalAmt = iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MAmount);
			iVExpenseModel.MTaxTotalAmtFor = iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MTaxAmountFor);
			iVExpenseModel.MTaxTotalAmt = iVExpenseModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MTaxAmount);
			return iVExpenseModel;
		}

		private List<IVExpenseEntryModel> GetExpenseEntryList(MContext ctx, IVExpenseModel expModel, List<IOExpenseImportModel> entryList, List<IOValidationResultModel> validationResult)
		{
			int num = 1;
			List<IVExpenseEntryModel> list = new List<IVExpenseEntryModel>();
			foreach (IOExpenseImportModel entry in entryList)
			{
				IVExpenseEntryModel iVExpenseEntryModel = ConvertToExpenseEntryModel(ctx, expModel, entry);
				iVExpenseEntryModel.MSeq = num;
				iVExpenseEntryModel.MTaxAmtFor = Math.Round(Math.Abs(entry.MTaxAmtFor), 2);
				base.SetExpenseEntryInfo(expModel, iVExpenseEntryModel);
				if (iVExpenseEntryModel.MTaxAmtFor > iVExpenseEntryModel.MTaxAmountFor)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.PaidAmount,
						FieldValue = iVExpenseEntryModel.MTaxAmtFor.ToString(),
						RowIndex = entry.MRowIndex,
						Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxAmountError", "税额：{{0}}不能大于总金额：{0}!"), iVExpenseEntryModel.MTaxAmountFor)
					});
				}
				list.Add(iVExpenseEntryModel);
				num++;
			}
			return list;
		}

		protected virtual IVExpenseEntryModel ConvertToExpenseEntryModel(MContext ctx, IVExpenseModel expModel, IOExpenseImportModel middleExpenseEntry)
		{
			IVExpenseEntryModel iVExpenseEntryModel = new IVExpenseEntryModel();
			iVExpenseEntryModel.MTaxTypeID = "Tax_Inclusive";
			iVExpenseEntryModel.MEntryID = UUIDHelper.GetGuid();
			iVExpenseEntryModel.IsNew = true;
			iVExpenseEntryModel.MItemID = middleExpenseEntry.MItemID;
			iVExpenseEntryModel.MDesc = middleExpenseEntry.MDesc;
			iVExpenseEntryModel.MQty = Math.Round(Math.Abs(middleExpenseEntry.MQty), 4);
			iVExpenseEntryModel.MPrice = Math.Round(Math.Abs(middleExpenseEntry.MPrice), 4);
			iVExpenseEntryModel.MDebitAccount = middleExpenseEntry.MDebitAccount;
			iVExpenseEntryModel.MCreditAccount = middleExpenseEntry.MCreditAccount;
			iVExpenseEntryModel.MTaxAccount = middleExpenseEntry.MTaxAccount;
			iVExpenseEntryModel.MTrackItem1 = middleExpenseEntry.MTrackItem1;
			iVExpenseEntryModel.MTrackItem2 = middleExpenseEntry.MTrackItem2;
			iVExpenseEntryModel.MTrackItem3 = middleExpenseEntry.MTrackItem3;
			iVExpenseEntryModel.MTrackItem4 = middleExpenseEntry.MTrackItem4;
			iVExpenseEntryModel.MTrackItem5 = middleExpenseEntry.MTrackItem5;
			iVExpenseEntryModel.MRowIndex = middleExpenseEntry.MRowIndex;
			iVExpenseEntryModel.MTaxAmtFor = Math.Round(Math.Abs(middleExpenseEntry.MTaxAmtFor), 2);
			base.SetExpenseEntryInfo(expModel, iVExpenseEntryModel);
			return iVExpenseEntryModel;
		}

		protected List<IOExpenseImportModel> GetMainExpenseList(List<IOExpenseImportModel> list)
		{
			return (from p in list
			group p by new
			{
				p.GroupByID,
				p.MEmployee,
				p.MReference,
				p.MBizDate,
				p.MDueDate,
				p.MExpectedDate,
				p.MCyID,
				p.MPaymentDate,
				p.MPaidAmount,
				p.MBankAccount
			} into g
			select new IOExpenseImportModel
			{
				GroupByID = g.Key.GroupByID,
				MEmployee = g.Key.MEmployee,
				MReference = g.Key.MReference,
				MBizDate = g.Key.MBizDate,
				MDueDate = g.Key.MDueDate,
				MExpectedDate = g.Key.MExpectedDate,
				MCyID = g.Key.MCyID,
				MPaymentDate = g.Key.MPaymentDate,
				MPaidAmount = g.Key.MPaidAmount,
				MBankAccount = g.Key.MBankAccount,
				MRowIndex = g.Key.GroupByID
			}).ToList();
		}
	}
}
