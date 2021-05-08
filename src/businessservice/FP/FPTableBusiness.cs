using JieNor.Megi.BusinessContract.FP;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Chart;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;

namespace JieNor.Megi.BusinessService.FP
{
	public class FPTableBusiness : IFPTableBusiness, IDataContract<FPTableModel>
	{
		private readonly FPTableRepository dal = new FPTableRepository();

		private JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

		public DataGridJson<FPTableViewModel> GetTableViewModelPageList(MContext ctx, FPTableViewFilterModel filter)
		{
			return dal.GetTableViewModelPageList(ctx, filter);
		}

		public DataGridJson<FPTableViewModel> GetTableViewModelGrid(MContext ctx, FPTableViewFilterModel filter)
		{
			return dal.GetTableViewModelGrid(ctx, filter);
		}

		public FPTableViewModel GetTableViewModel(MContext ctx, string tableId = null, string invoiceIds = null, int invoiceType = 0)
		{
			FPTableViewModel obj = new FPTableViewModel
			{
				MBizDate = ctx.DateNow
			};
			int nextTableNumber = GetNextTableNumber(ctx, invoiceType);
			obj.MNumber = nextTableNumber.ToString();
			obj.MInvoiceType = invoiceType;
			obj.MType = ((ctx.MOrgTaxType != 2) ? 1 : 0);
			FPTableViewModel fPTableViewModel = obj;
			if (!string.IsNullOrWhiteSpace(tableId))
			{
				return dal.GetViewModelByTableID(ctx, tableId);
			}
			if (!string.IsNullOrWhiteSpace(invoiceIds))
			{
				List<string> tableIdListByInvoiceIds = dal.GetTableIdListByInvoiceIds(ctx, invoiceIds);
				if (tableIdListByInvoiceIds != null && tableIdListByInvoiceIds.Count > 0 && (from x in tableIdListByInvoiceIds
				where !string.IsNullOrWhiteSpace(x)
				select x).Count() > 0)
				{
					return dal.GetViewModelByTableID(ctx, tableIdListByInvoiceIds[0]);
				}
				SqlWhere sqlWhere = new SqlWhere
				{
					PageSize = 0,
					rows = 0
				};
				sqlWhere.AddFilter("MID", SqlOperators.In, invoiceIds.Split(',').ToArray());
				List<IVInvoiceModel> invoiceListIncludeEntry = IVInvoiceRepository.GetInvoiceListIncludeEntry(ctx, sqlWhere);
				invoiceListIncludeEntry = (from x in invoiceListIncludeEntry
				where x.MStatus >= 3
				select x).ToList();
				BDContactsInfoModel bDContactsInfoModel = new BDContactsRepository().GetContactByIDs(ctx, new List<string>
				{
					invoiceListIncludeEntry[0].MContactID
				}, true).FirstOrDefault();
				if (invoiceListIncludeEntry.Count > 0)
				{
					fPTableViewModel.MContactName = bDContactsInfoModel.MContactName;
					fPTableViewModel.MContactTaxCode = bDContactsInfoModel.MTaxNo;
					fPTableViewModel.MContactID = invoiceListIncludeEntry[0].MContactID;
					FPTableViewModel fPTableViewModel2 = fPTableViewModel;
					nextTableNumber = GetNextTableNumber(ctx, invoiceType);
					fPTableViewModel2.MNumber = nextTableNumber.ToString();
					fPTableViewModel.MOrgID = ctx.MOrgID;
					//fPTableViewModel.MBankId= invoiceListIncludeEntry[0].ba
					fPTableViewModel.MTaxAmount = invoiceListIncludeEntry.Sum((IVInvoiceModel x) => x.InvoiceEntry.Sum((IVInvoiceEntryModel y) => y.MTaxAmt));
					fPTableViewModel.MTotalAmount = invoiceListIncludeEntry.Sum((IVInvoiceModel x) => x.MTaxTotalAmt);
					fPTableViewModel.MAmount = invoiceListIncludeEntry.Sum((IVInvoiceModel x) => x.MTotalAmt);
					fPTableViewModel.MFapiaoType = 0;
					fPTableViewModel.MBizDate = ctx.DateNow;
					fPTableViewModel.MInvoiceType = ((invoiceListIncludeEntry[0].MType.IndexOf("Invoice_Sale") < 0) ? 1 : 0);
				}
			}
			return fPTableViewModel;
		}

		public int GetNextTableNumber(MContext ctx, int invoiceType)
		{
			return dal.GetNextTableNumber(ctx, invoiceType);
		}

		public OperationResult SaveTable(MContext ctx, FPTableViewModel table)
		{
			return dal.SaveTable(ctx, table);
		}

		public List<FPFapiaoModel> GetFapiaoListByTableInvoice(MContext ctx, string tableId, string invoiceIds)
		{
			return dal.GetFapiaoListByTableInvoice(ctx, tableId, invoiceIds);
		}

		public OperationResult DeleteTableByInvoiceIds(MContext ctx, string invoiceIds)
		{
			return dal.DeleteTableByInvoiceIds(ctx, invoiceIds);
		}

		public OperationResult FPAddLog(MContext ctx, FPFapiaoModel model)
		{
			return dal.FPAddLog(ctx, model);
		}

		public OperationResult DeleteTableByTableIds(MContext ctx, string tableIds)
		{
			return dal.DeleteTableByTableIds(ctx, tableIds);
		}

		public int GetNextTableNumber(MContext ctx, string tableId, int invoiceType)
		{
			return dal.GetNextTableNumber(ctx, tableId, invoiceType);
		}

		public bool IsTableNumberAvailable(MContext ctx, DateTime date, int number)
		{
			if (date.Year == 1900)
			{
				date = ctx.DateNow;
			}
			List<FPTableViewModel> tableViewModelList = GetTableViewModelList(ctx, new FPTableViewFilterModel
			{
				MTableDate = date
			});
			return (from x in tableViewModelList
			where int.Parse(x.MNumber) == number
			select x).Count() == 0;
		}

		public List<FPTableViewModel> GetTableViewModelList(MContext ctx, FPTableViewFilterModel filter)
		{
			return dal.GetTableViewModelList(ctx, filter);
		}

		public List<NameValueModel> GetTableHomeData(MContext ctx, int invoiceType, DateTime date)
		{
			return dal.GetTableHomeData(ctx, invoiceType, date);
		}

		public FPTableViewModel GetTableViewModelByInvoiceID(MContext ctx, string invoiceId)
		{
			return dal.GetTableViewModelByInvoiceID(ctx, invoiceId);
		}

		public string GetChartStackedDictionary(MContext ctx, int fapiaoType, DateTime startDate, DateTime endDate)
		{
			//string text = "";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			double num = 0.0;
			List<string> chartColumnList = GetChartColumnList(ctx, startDate, endDate);
			List<ChartColumnStacked2DModel> list = (List<ChartColumnStacked2DModel>)(dictionary["data"] = GetBarChartDataList(ctx, startDate, endDate, fapiaoType));
			dictionary["labels"] = chartColumnList;
			dictionary["scalSpace"] = Math.Ceiling(num / 30.0) * 10.0;
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			List<PieChart2DModel> list2 = (List<PieChart2DModel>)(dictionary2["data"] = GetPieChartDataList(ctx, startDate, endDate, fapiaoType));
			dictionary2["totalAmount"] = ((list2.Count() >= 2) ? list2[1].MTotalAmount : decimal.Zero);
			object obj = new
			{
				BarChartData = dictionary,
				PieChartData = dictionary2
			};
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(obj);
		}

		private List<string> GetChartColumnList(MContext ctx, DateTime startDate, DateTime endDate)
		{
			List<string> list = new List<string>();
			for (int num = 5; num >= 0; num--)
			{
				int month = endDate.AddMonths(-num).Month;
				DateMonth dateMonth = (DateMonth)month;
				string text = dateMonth.ToString();
				string key = "Month_" + text;
				text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, key, text);
				list.Add(text);
			}
			return list;
		}

		private List<ChartColumnStacked2DModel> GetBarChartDataList(MContext ctx, DateTime startDate, DateTime endDate, int fapiaoType)
		{
			List<ChartColumnStacked2DModel> list = new List<ChartColumnStacked2DModel>();
			FPFapiaoFilterModel fPFapiaoFilterModel = new FPFapiaoFilterModel();
			fPFapiaoFilterModel.MStartDate = startDate;
			fPFapiaoFilterModel.MEndDate = endDate;
			fPFapiaoFilterModel.MFapiaoCategory = fapiaoType;
			FPFapiaoRepository fPFapiaoRepository = new FPFapiaoRepository();
			DataTable fapiaoSummaryByDate = fPFapiaoRepository.GetFapiaoSummaryByDate(ctx, fPFapiaoFilterModel);
			FPTableRepository fPTableRepository = new FPTableRepository();
			DataTable notReconcileTableSummaryByDate = fPTableRepository.GetNotReconcileTableSummaryByDate(ctx, fPFapiaoFilterModel);
			ChartColumnStacked2DModel chartColumnStacked2DModel = new ChartColumnStacked2DModel();
			var obj = new
			{
				name = ((fapiaoType == 0) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "InvoicedAmount", "已开发票金额") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "InputFapiaoRecieveAmount", "已收到发票金额")),
				MContactID = "",
				MChartFirstName = "",
				MChartLastName = "",
				MChartDueOrOwing = ""
			};
			chartColumnStacked2DModel.name = jsSerializer.Serialize(obj);
			chartColumnStacked2DModel.value = ConvertToValueList(fapiaoSummaryByDate, startDate, endDate).ToArray();
			list.Add(chartColumnStacked2DModel);
			ChartColumnStacked2DModel chartColumnStacked2DModel2 = new ChartColumnStacked2DModel();
			var obj2 = new
			{
				name = ((fapiaoType == 0) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "NotInvoiceAmount", "已开发票金额") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "InputFapiaoNotRecieveAmount", "未收到发票金额")),
				MContactID = "",
				MChartFirstName = "",
				MChartLastName = "",
				MChartDueOrOwing = ""
			};
			chartColumnStacked2DModel2.name = jsSerializer.Serialize(obj2);
			chartColumnStacked2DModel2.value = ConvertToValueList(notReconcileTableSummaryByDate, startDate, endDate).ToArray();
			list.Add(chartColumnStacked2DModel2);
			return list;
		}

		private List<double> ConvertToValueList(DataTable dt, DateTime startDate, DateTime endDate)
		{
			List<double> list = new List<double>();
			if (dt == null || dt.Rows.Count == 0)
			{
				return new List<double>
				{
					0.0,
					0.0,
					0.0,
					0.0,
					0.0,
					0.0
				};
			}
			for (int num = 5; num >= 0; num--)
			{
				int month = endDate.AddMonths(-num).Month;
				double item = 0.0;
				foreach (DataRow row in dt.Rows)
				{
					int num2 = (row["MONTH"] != DBNull.Value) ? Convert.ToInt32(Convert.ToDouble(row["MONTH"])) : 0;
					if (num2 == month)
					{
						item = ((row["MTotalAmount"] != DBNull.Value) ? Convert.ToDouble(row["MTotalAmount"]) : 0.0);
						break;
					}
				}
				list.Add(item);
			}
			return list;
		}

		private List<PieChart2DModel> GetPieChartDataList(MContext ctx, DateTime startDate, DateTime endDate, int fapiaoType)
		{
			int year = ctx.DateNow.Year;
			FPTableViewFilterModel fPTableViewFilterModel = new FPTableViewFilterModel();
			fPTableViewFilterModel.MStartDate = startDate.Date;
			fPTableViewFilterModel.MEndDate = endDate.Date;
			fPTableViewFilterModel.MInvoiceType = string.Concat(fapiaoType);
			List<PieChart2DModel> list = new List<PieChart2DModel>();
			FPTableRepository fPTableRepository = new FPTableRepository();
			DataTable ytdTabelSummaryAmount = fPTableRepository.GetYtdTabelSummaryAmount(ctx, year, fPTableViewFilterModel);
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			if (ytdTabelSummaryAmount != null && ytdTabelSummaryAmount.Rows.Count > 0)
			{
				foreach (DataRow row in ytdTabelSummaryAmount.Rows)
				{
					string a = row.Field<string>("SummaryType");
					if (a == "1")
					{
						num = ((row["MTotalAmount"] != DBNull.Value) ? row.Field<decimal>("MTotalAmount") : decimal.Zero);
					}
					else if (a == "2")
					{
						num2 = ((row["MTotalAmount"] != DBNull.Value) ? row.Field<decimal>("MTotalAmount") : decimal.Zero);
					}
				}
			}
			decimal num3 = (num != decimal.Zero) ? (Math.Round(num2 / num, 4) * 100m) : decimal.Zero;
			PieChart2DModel pieChart2DModel = new PieChart2DModel();
			pieChart2DModel.name = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "NotInvoiceAmount", "已开发票金额");
			pieChart2DModel.value = num3;
			pieChart2DModel.MTotalAmount = num;
			list.Add(pieChart2DModel);
			PieChart2DModel pieChart2DModel2 = new PieChart2DModel();
			pieChart2DModel2.name = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "NotInvoiceAmount", "已开发票金额");
			pieChart2DModel2.value = 100m - num3;
			pieChart2DModel2.MTotalAmount = num2;
			list.Add(pieChart2DModel2);
			return list;
		}

		public string GetDashboardTableData(MContext ctx)
		{
			//string text = "";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			DataTable fapiaoTaxByMonth = new FPFapiaoRepository().GetFapiaoTaxByMonth(ctx, ctx.DateNow);
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			if (fapiaoTaxByMonth != null && fapiaoTaxByMonth.Rows.Count > 0)
			{
				foreach (DataRow row3 in fapiaoTaxByMonth.Rows)
				{
					int num3 = row3.Field<int>("MInvoiceType");
					if (num3 == 0)
					{
						num = row3.Field<decimal>("TaxAmount");
					}
					else if (1 == num3)
					{
						num2 = row3.Field<decimal>("TaxAmount");
					}
				}
			}
			dictionary["InputTax"] = num2;
			dictionary["OutputTax"] = num;
			List<object> list = new List<object>();
			DataTable supplierUnReconcileFapiaoTop = new FPTableRepository().GetSupplierUnReconcileFapiaoTop(ctx, ctx.DateNow, 3);
			if (supplierUnReconcileFapiaoTop != null && supplierUnReconcileFapiaoTop.Rows.Count > 0)
			{
				foreach (DataRow row4 in supplierUnReconcileFapiaoTop.Rows)
				{
					var item = new
					{
						ContactName = row4.Field<string>("ContactName"),
						Amount = row4.Field<decimal>("MTotalAmount")
					};
					list.Add(item);
				}
			}
			var obj = new
			{
				TaxData = dictionary,
				SupplierData = list
			};
			return jsSerializer.Serialize(obj);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FPTableModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FPTableModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public FPTableModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public FPTableModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<FPTableModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FPTableModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
