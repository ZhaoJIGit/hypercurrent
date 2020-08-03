using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Invoice;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVInvoiceBusiness : IDataContract<IVInvoiceModel>
	{
		OperationResult UpdateRepeatInvoiceMessage(MContext ctx, IVInvoiceEmailSendModel model);

		IVRepeatInvoiceModel GetRepeatInvoiceEditModel(MContext ctx, string pkID);

		IVRepeatInvoiceModel GetRepeatInvoiceCopyModel(MContext ctx, string pkID);

		OperationResult UpdateRepeatInvoice(MContext ctx, IVRepeatInvoiceModel model);

		OperationResult UpdateRepeatInvoiceStatus(MContext ctx, ParamBase param);

		OperationResult DeleteRepeatInvoiceList(MContext ctx, ParamBase param);

		OperationResult AddRepeatInvoiceNoteLog(MContext ctx, IVRepeatInvoiceModel model);

		OperationResult UpdateRepeatInvoiceExpectedInfo(MContext ctx, IVRepeatInvoiceModel model);

		DataGridJson<IVRepeatInvoiceListModel> GetRepeatInvoiceList(MContext ctx, IVInvoiceListFilterModel param);

		IVGoldenTaxInvoiceModel GetGoldenTaxInvoiceEditModel(MContext ctx, string pkID);

		OperationResult UpdateGoldenTaxInvoice(MContext ctx, IVGoldenTaxInvoiceModel model);

		OperationResult UpdateGoldenTaxCourierInfo(MContext ctx, IVGoldenTaxInvoiceModel model);

		OperationResult DeleteGoldenTaxInvoiceList(MContext ctx, ParamBase param);

		OperationResult ArchiveGoldenTaxInvoiceList(MContext ctx, ParamBase param);

		OperationResult UpdateGoldenTaxInvoicePrintStatusList(MContext ctx, ParamBase param, bool isPrint);

		OperationResult AddGoldenTaxInvoiceNoteLog(MContext ctx, IVGoldenTaxInvoiceModel model);

		DataGridJson<IVGoldenTaxInvoiceListModel> GetGoldenTaxInvoiceList(MContext ctx, IVGoldenTaxInvoiceListFilterModel filter);

		OperationResult AddInvoiceNoteLog(MContext ctx, IVInvoiceModel model);

		IVInvoiceSummaryModel GetInvoiceSummaryModel(MContext ctx, string type, DateTime startDate, DateTime endDate);

		IVContactInvoiceSummaryModel GetInvoiceSummaryModelByContact(MContext ctx, string contactId);

		DataGridJson<IVInvoiceListModel> GetInvoiceList(MContext ctx, IVInvoiceListFilterModel param);

		List<NameValueModel> GetFPInvoiceSummary(MContext ctx, IVInvoiceListFilterModel filter);

		List<IVInvoiceModel> GetInvoiceListForPrint(MContext ctx, IVInvoiceListFilterModel param);

		List<IVInvoiceModel> GetInvoiceListForExport(MContext ctx, IVInvoiceListFilterModel param);

		OperationResult UpdateInvoice(MContext ctx, IVInvoiceModel model);

		OperationResult UpdateInvoiceStatus(MContext ctx, ParamBase param);

		OperationResult UnApproveInvoice(MContext ctx, string invoiceId);

		OperationResult ApproveInvoice(MContext ctx, ParamBase param);

		OperationResult UpdateInvoiceExpectedInfo(MContext ctx, IVInvoiceModel model);

		IVInvoiceModel GetInvoiceEditModel(MContext ctx, string pkID, string bizType);

		IVInvoiceModel GetInvoiceCopyModel(MContext ctx, string pkID, bool isCopyCredit);

		OperationResult DeleteInvoiceList(MContext ctx, ParamBase param);

		OperationResult PayToInvoice(MContext ctx, IVMakePaymentModel model);

		string GetChartStackedDictionary(MContext ctx, string Type, DateTime startDate, DateTime endDate, string contactId = null);

		List<ChartPie2DModel> GetChartPieDictionary(MContext ctx, string Type, DateTime startDate, DateTime endDate);

		List<IVStatementsModel> GetStatementData(MContext ctx, IVStatementListFilterModel param);

		List<IVViewStatementModel> GetViewStatementData(MContext ctx, string contactID, string type, DateTime BeginDate, DateTime EndDate);

		List<IVViewStatementModel> GetViewStatementOpeningBalanceDate(MContext ctx, string statementType, string contactId, DateTime beginDate);

		string GetOverPastDictionary(MContext ctx, string contactID);

		List<IVInvoiceSendModel> GetSendInvoiceList(MContext ctx, IVInvoiceSendParam param);

		OperationResult SendInvoiceList(MContext ctx, IVInvoiceEmailSendModel model);

		List<IVEmailTmplPreviewModel> PreviewEmailTmpl(MContext ctx, IVInvoiceEmailSendModel model);

		OperationResult ImportInvoiceList(MContext ctx, List<IVInvoiceModel> list, bool isFromApi = false);

		ImportTemplateModel GetImportTemplateModel(MContext ctx, string invoiceType);

		List<BDCheckInactiveModel> GetInactiveList(MContext ctx);
	}
}
