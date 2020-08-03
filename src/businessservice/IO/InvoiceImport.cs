using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Utility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.IO
{
	public class InvoiceImport : IVImportBase, IImport
	{
		private List<CommandInfo> cmdList = new List<CommandInfo>();

		private List<BDAccountListModel> acctList = null;

		private ImportTypeEnum importType = ImportTypeEnum.None;

		public InvoiceImport(ImportTypeEnum importType)
		{
			this.importType = importType;
		}

		public ImportResult ImportData(MContext ctx, IOImportDataModel data)
		{
			ImportResult importResult = new ImportResult();
			List<IVInvoiceModel> list = new List<IVInvoiceModel>();
			List<IVBatchPayHeadModel> paymentList = new List<IVBatchPayHeadModel>();
			List<IOValidationResultModel> validationResult = new List<IOValidationResultModel>();
			List<IOInvoiceImportModel> list2 = base.ConvertToList<IOInvoiceImportModel>(data.EffectiveData);
			foreach (IOInvoiceImportModel item in list2)
			{
				switch (importType)
				{
				case ImportTypeEnum.Invoice:
					item.MType = "Invoice_Sale";
					break;
				case ImportTypeEnum.Purchase:
					item.MType = "Invoice_Purchase";
					break;
				}
			}
			List<IOInvoiceImportModel> mainExpenseList = GetMainExpenseList(list2);
			base.SetCacheData(ctx);
			ValidateBasicData(ctx, mainExpenseList, list2, data, validationResult);
			foreach (IOInvoiceImportModel item2 in mainExpenseList)
			{
				IVInvoiceModel invoiceModel = GetInvoiceModel(ctx, item2, list2, paymentList, validationResult);
				list.Add(invoiceModel);
			}
			base.SetValidationResult(ctx, importResult, validationResult, false);
			if (!importResult.Success)
			{
				return importResult;
			}
			cmdList.AddRange(IVInvoiceRepository.GetImportInvoiceCmdList(ctx, list));
			ImportResult importResult2 = ResultHelper.ToImportResult(list);
			if (!importResult2.Success)
			{
				return importResult2;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			importResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(cmdList) > 0);
			importResult.SuccessRowIndexes = string.Join(",", from f in list2
			select f.MRowIndex);
			return importResult;
		}

		private static void ValidateNumberDuplicate(MContext ctx, List<IOValidationResultModel> validationResult, List<string> noList, IOInvoiceImportModel model)
		{
			if (!string.IsNullOrWhiteSpace(model.MNumber))
			{
				if (!noList.Contains(model.MNumber))
				{
					noList.Add(model.MNumber);
				}
				else
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.InvoiceNumber,
						FieldValue = model.MNumber,
						RowIndex = model.MRowIndex,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DuplicateInvoiceNo", "导入的销售单号:{0}已存在!")
					});
				}
			}
		}

		private void ValidateBasicData(MContext ctx, List<IOInvoiceImportModel> mainList, List<IOInvoiceImportModel> list, IOImportDataModel data, List<IOValidationResultModel> validationResult)
		{
			List<IOInvoiceImportModel> list2 = (from f in mainList
			where f.MDueDate < f.MBizDate
			select f).ToList();
			if (list2.Any())
			{
				string msg = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DueDateMustGreaterOrEqualDate", "Due date must be greater than or equal to date!");
				list2.ForEach(delegate(IOInvoiceImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.DueDate,
						RowIndex = f.MRowIndex,
						Message = msg
					});
				});
			}
			IEnumerable<IOInvoiceImportModel> source = from f in mainList
			where f.MExpectedDate != DateTime.MinValue && f.MExpectedDate < f.MBizDate
			select f;
			if (source.Any())
			{
				string msg2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ReceiptDateGreaterThanDate", "Expected Payment date must be greater than or equal to date!");
				list2.ForEach(delegate(IOInvoiceImportModel f)
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
			List<string> noList = new List<string>();
			foreach (IOInvoiceImportModel main in mainList)
			{
				ValidateNumberDuplicate(ctx, validationResult, noList, main);
			}
			List<IOInvoiceImportModel> list3 = (from x in list
			where x.MQty <= decimal.Zero
			select x).ToList();
			if (list3.Any())
			{
				string msg3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "QtyMustGreaterThanZere", "数量必须大于0！");
				list3.ForEach(delegate(IOInvoiceImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Quantity,
						RowIndex = f.MRowIndex,
						Message = msg3
					});
				});
			}
			List<IOInvoiceImportModel> list4 = (from x in list
			where x.MDiscount > 100m || x.MDiscount < decimal.Zero
			select x).ToList();
			if (list4.Any())
			{
				string msg4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DiscountInvoid", "折扣率必须大于等于零且小于等于100。");
				list4.ForEach(delegate(IOInvoiceImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Quantity,
						RowIndex = f.MRowIndex,
						Message = msg4
					});
				});
			}
			List<IOInvoiceImportModel> list5 = (from f in list
			where f.MPrice < decimal.Zero
			select f).ToList();
			if (list5.Any())
			{
				string msg5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "UnitPriceIsNegative", "单价必须大于0！");
				list5.ForEach(delegate(IOInvoiceImportModel f)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Price,
						RowIndex = f.MRowIndex,
						Message = msg5
					});
				});
			}
			base.CheckInvoiceNumberExist(ctx, mainList, validationResult);
			base.CheckContactExist(ctx, mainList, validationResult);
			base.CheckCurrencyExist(ctx, mainList, "MCyID", validationResult);
			base.CheckCurrencyExchangeRateExist(ctx, mainList, validationResult, base.ForeignCurrencyList, "MCyID", "MBizDate");
			base.CheckTaxTypeExist(ctx, list, validationResult, "MTaxTypeID");
			base.CheckItemExist(ctx, list, validationResult);
			base.CheckTaxRateExist(ctx, list, validationResult);
			base.CheckTrackExist(ctx, data.TrackItemNameList, list, validationResult);
			acctList = base.GetAccountList(ctx);
			foreach (IOInvoiceImportModel item in list)
			{
				base.CheckAccountExist(ctx, item, acctList, "MDebitAccount", validationResult);
				base.CheckAccountExist(ctx, item, acctList, "MCreditAccount", validationResult);
				base.CheckAccountExist(ctx, item, acctList, "MTaxAccount", validationResult);
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TaxRateUnSelect", "请选择税率！");
			foreach (IOInvoiceImportModel item2 in list)
			{
				if (string.IsNullOrWhiteSpace(item2.MTaxID) && (item2.MTaxTypeID == "Tax_Exclusive" || item2.MTaxTypeID == "Tax_Inclusive"))
				{
					validationResult.Add(new IOValidationResultModel
					{
						RowIndex = item2.MRowIndex,
						FieldType = IOValidationTypeEnum.TaxRate,
						Message = text
					});
				}
				else if (item2.MTaxTypeID == "No_Tax")
				{
					item2.MTaxID = string.Empty;
				}
			}
		}

		private IVInvoiceModel GetInvoiceModel(MContext ctx, IOInvoiceImportModel mainModel, List<IOInvoiceImportModel> list, List<IVBatchPayHeadModel> paymentList, List<IOValidationResultModel> validationResult)
		{
			List<IOInvoiceImportModel> list2 = (from t in list
			where t.GroupByID == mainModel.GroupByID
			select t).ToList();
			if (list2 == null || list2.Count == 0)
			{
				return null;
			}
			IVInvoiceModel iVInvoiceModel = new IVInvoiceModel();
			iVInvoiceModel.MID = UUIDHelper.GetGuid();
			iVInvoiceModel.MType = ((importType == ImportTypeEnum.Invoice) ? "Invoice_Sale" : "Invoice_Purchase");
			iVInvoiceModel.IsNew = true;
			iVInvoiceModel.MContactID = mainModel.MContactID;
			iVInvoiceModel.MNumber = mainModel.MNumber;
			iVInvoiceModel.MReference = mainModel.MReference;
			iVInvoiceModel.MBizDate = mainModel.MBizDate;
			iVInvoiceModel.MDueDate = mainModel.MDueDate;
			iVInvoiceModel.MExpectedDate = mainModel.MExpectedDate;
			iVInvoiceModel.MCyID = mainModel.MCyID;
			iVInvoiceModel.MTaxID = list2[0].MTaxTypeID;
			iVInvoiceModel.MStatus = 1;
			base.SetT1Info<IVInvoiceModel, IVInvoiceEntryModel>(iVInvoiceModel);
			iVInvoiceModel.InvoiceEntry = GetInvoiceEntryList(iVInvoiceModel, list2);
			ValidateModel(ctx, iVInvoiceModel, validationResult);
			return iVInvoiceModel;
		}

		private void ValidateModel(MContext ctx, IVInvoiceModel model, List<IOValidationResultModel> validationResult)
		{
			if (model.MTaxTotalAmtFor <= decimal.Zero)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TotalAmountNotToZero", "总金额必须大于零。");
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.TotalAmount,
					RowIndex = model.MRowIndex,
					Message = text
				});
			}
		}

		private List<IVInvoiceEntryModel> GetInvoiceEntryList(IVInvoiceModel ivModel, List<IOInvoiceImportModel> entryList)
		{
			int num = 1;
			List<IVInvoiceEntryModel> list = new List<IVInvoiceEntryModel>();
			foreach (IOInvoiceImportModel entry in entryList)
			{
				IVInvoiceEntryModel iVInvoiceEntryModel = new IVInvoiceEntryModel();
				iVInvoiceEntryModel.MItemID = entry.MItemID;
				iVInvoiceEntryModel.MDesc = entry.MDesc;
				iVInvoiceEntryModel.MQty = Math.Abs(entry.MQty);
				iVInvoiceEntryModel.MPrice = Math.Abs(entry.MPrice);
				iVInvoiceEntryModel.MTaxID = entry.MTaxID;
				iVInvoiceEntryModel.MDiscount = Math.Abs(entry.MDiscount);
				iVInvoiceEntryModel.MDebitAccount = entry.MDebitAccount;
				iVInvoiceEntryModel.MCreditAccount = entry.MCreditAccount;
				iVInvoiceEntryModel.MTaxAccount = entry.MTaxAccount;
				iVInvoiceEntryModel.MTrackItem1 = entry.MTrackItem1;
				iVInvoiceEntryModel.MTrackItem2 = entry.MTrackItem2;
				iVInvoiceEntryModel.MTrackItem3 = entry.MTrackItem3;
				iVInvoiceEntryModel.MTrackItem4 = entry.MTrackItem4;
				iVInvoiceEntryModel.MTrackItem5 = entry.MTrackItem5;
				iVInvoiceEntryModel.MSeq = num;
				base.SetT2Info(ivModel, iVInvoiceEntryModel);
				list.Add(iVInvoiceEntryModel);
				num++;
			}
			return list;
		}

		private List<IOInvoiceImportModel> GetMainExpenseList(List<IOInvoiceImportModel> list)
		{
			return (from p in list
			group p by new
			{
				p.GroupByID,
				p.MContactID,
				p.MNumber,
				p.MReference,
				p.MBizDate,
				p.MDueDate,
				p.MExpectedDate,
				p.MCyID,
				p.MTaxTypeID,
				p.MType
			} into g
			select new IOInvoiceImportModel
			{
				GroupByID = g.Key.GroupByID,
				MNumber = g.Key.MNumber,
				MContactID = g.Key.MContactID,
				MReference = g.Key.MReference,
				MBizDate = g.Key.MBizDate,
				MDueDate = g.Key.MDueDate,
				MExpectedDate = g.Key.MExpectedDate,
				MCyID = g.Key.MCyID,
				MTaxTypeID = g.Key.MTaxTypeID,
				MRowIndex = g.Key.GroupByID,
				MType = g.Key.MType
			}).ToList();
		}
	}
}
