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
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVInvoice
	{
		[OperationContract]
		MActionResult<OperationResult> UpdateRepeatInvoiceMessage(IVInvoiceEmailSendModel model, string accessToken = null);

		[OperationContract]
		MActionResult<IVRepeatInvoiceModel> GetRepeatInvoiceEditModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<IVRepeatInvoiceModel> GetRepeatInvoiceCopyModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateRepeatInvoice(IVRepeatInvoiceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateRepeatInvoiceStatus(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteRepeatInvoiceList(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> AddRepeatInvoiceNoteLog(IVRepeatInvoiceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateRepeatInvoiceExpectedInfo(IVRepeatInvoiceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<IVRepeatInvoiceListModel>> GetRepeatInvoiceList(IVInvoiceListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> AddInvoiceNoteLog(IVInvoiceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<IVInvoiceSummaryModel> GetInvoiceSummaryModel(string type, DateTime startDate, DateTime endDate, string accessToken = null);

		[OperationContract]
		MActionResult<IVContactInvoiceSummaryModel> GetInvoiceSummaryModelByContact(string contactId, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<IVInvoiceListModel>> GetInvoiceList(IVInvoiceListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<List<NameValueModel>> GetFPInvoiceSummary(IVInvoiceListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVInvoiceModel>> GetInvoiceListForPrint(IVInvoiceListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVInvoiceModel>> GetInvoiceListForExport(IVInvoiceListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateInvoice(IVInvoiceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateInvoiceStatus(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UnApproveInvoice(string invoiceId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ApproveInvoice(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateInvoiceExpectedInfo(IVInvoiceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<IVInvoiceModel> GetInvoiceEditModel(string pkID, string bizType, string accessToken = null);

		[OperationContract]
		MActionResult<IVInvoiceModel> GetInvoiceCopyModel(string pkID, bool isCopyCredit, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteInvoiceList(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> PayToInvoice(IVMakePaymentModel model, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetChartStackedDictionary(string Type, DateTime startDate, DateTime endDate, string contactId = null, string accessToken = null);

		[OperationContract]
		MActionResult<List<ChartPie2DModel>> GetChartPieDictionary(string Type, DateTime startDate, DateTime endDate, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVStatementsModel>> GetStatementData(IVStatementListFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVViewStatementModel>> GetViewStatementData(string contactID, string type, DateTime BeginDate, DateTime EndDate, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVViewStatementModel>> GetViewStatementOpeningBalanceDate(string statementType, string contactId, DateTime beginDate, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetOverPastDictionary(string contactID, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVInvoiceSendModel>> GetSendInvoiceList(IVInvoiceSendParam param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SendInvoiceList(IVInvoiceEmailSendModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVEmailTmplPreviewModel>> PreviewEmailTmpl(IVInvoiceEmailSendModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportInvoiceList(List<IVInvoiceModel> list, bool isFromApi = false, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(string invoiceType, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDCheckInactiveModel>> GetInactiveList(string accessToken = null);

		[OperationContract]
		MActionResult<IVInvoiceModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);
	}
}
