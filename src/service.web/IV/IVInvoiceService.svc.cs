using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Invoice;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.IV
{
	public class IVInvoiceService : ServiceT<IVInvoiceModel>, IIVInvoice
	{
		private readonly IIVInvoiceBusiness biz = new IVInvoiceBusiness();

		public MActionResult<OperationResult> UpdateRepeatInvoiceMessage(IVInvoiceEmailSendModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateRepeatInvoiceMessage, model, accessToken);
		}

		public MActionResult<IVRepeatInvoiceModel> GetRepeatInvoiceEditModel(string pkID, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetRepeatInvoiceEditModel, pkID, accessToken);
		}

		public MActionResult<IVRepeatInvoiceModel> GetRepeatInvoiceCopyModel(string pkID, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetRepeatInvoiceCopyModel, pkID, accessToken);
		}

		public MActionResult<OperationResult> UpdateRepeatInvoice(IVRepeatInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateRepeatInvoice, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateRepeatInvoiceStatus(ParamBase param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateRepeatInvoiceStatus, param, accessToken);
		}

		public MActionResult<OperationResult> DeleteRepeatInvoiceList(ParamBase param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.DeleteRepeatInvoiceList, param, accessToken);
		}

		public MActionResult<OperationResult> AddRepeatInvoiceNoteLog(IVRepeatInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.AddRepeatInvoiceNoteLog, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateRepeatInvoiceExpectedInfo(IVRepeatInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateRepeatInvoiceExpectedInfo, model, accessToken);
		}

		public MActionResult<DataGridJson<IVRepeatInvoiceListModel>> GetRepeatInvoiceList(IVInvoiceListFilterModel param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetRepeatInvoiceList, param, accessToken);
		}

		public MActionResult<IVGoldenTaxInvoiceModel> GetGoldenTaxInvoiceEditModel(string pkID, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetGoldenTaxInvoiceEditModel, pkID, accessToken);
		}

		public MActionResult<OperationResult> UpdateGoldenTaxInvoice(IVGoldenTaxInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateGoldenTaxInvoice, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateGoldenTaxCourierInfo(IVGoldenTaxInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateGoldenTaxCourierInfo, model, accessToken);
		}

		public MActionResult<OperationResult> DeleteGoldenTaxInvoiceList(ParamBase param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.DeleteGoldenTaxInvoiceList, param, accessToken);
		}

		public MActionResult<OperationResult> ArchiveGoldenTaxInvoiceList(ParamBase param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.ArchiveGoldenTaxInvoiceList, param, accessToken);
		}

		public MActionResult<OperationResult> UpdateGoldenTaxInvoicePrintStatusList(ParamBase param, bool isPrint, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateGoldenTaxInvoicePrintStatusList, param, isPrint, accessToken);
		}

		public MActionResult<OperationResult> AddGoldenTaxInvoiceNoteLog(IVGoldenTaxInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.AddGoldenTaxInvoiceNoteLog, model, accessToken);
		}

		public MActionResult<DataGridJson<IVGoldenTaxInvoiceListModel>> GetGoldenTaxInvoiceList(IVGoldenTaxInvoiceListFilterModel filter, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetGoldenTaxInvoiceList, filter, accessToken);
		}

		public MActionResult<OperationResult> AddInvoiceNoteLog(IVInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.AddInvoiceNoteLog, model, accessToken);
		}

		public MActionResult<IVInvoiceSummaryModel> GetInvoiceSummaryModel(string type, DateTime startDate, DateTime endDate, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetInvoiceSummaryModel, type, startDate, endDate, accessToken);
		}

		public MActionResult<IVContactInvoiceSummaryModel> GetInvoiceSummaryModelByContact(string contactId, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetInvoiceSummaryModelByContact, contactId, accessToken);
		}

		public MActionResult<DataGridJson<IVInvoiceListModel>> GetInvoiceList(IVInvoiceListFilterModel filter, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetInvoiceList, filter, accessToken);
		}

		public MActionResult<List<NameValueModel>> GetFPInvoiceSummary(IVInvoiceListFilterModel filter, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetFPInvoiceSummary, filter, accessToken);
		}

		public MActionResult<List<IVInvoiceModel>> GetInvoiceListForPrint(IVInvoiceListFilterModel filter, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetInvoiceListForPrint, filter, accessToken);
		}

		public MActionResult<List<IVInvoiceModel>> GetInvoiceListForExport(IVInvoiceListFilterModel filter, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetInvoiceListForExport, filter, accessToken);
		}

		public MActionResult<OperationResult> UpdateInvoice(IVInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateInvoice, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateInvoiceStatus(ParamBase param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateInvoiceStatus, param, accessToken);
		}

		public MActionResult<OperationResult> UnApproveInvoice(string invoiceId, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UnApproveInvoice, invoiceId, accessToken);
		}

		public MActionResult<OperationResult> ApproveInvoice(ParamBase param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.ApproveInvoice, param, accessToken);
		}

		public MActionResult<OperationResult> UpdateInvoiceExpectedInfo(IVInvoiceModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.UpdateInvoiceExpectedInfo, model, accessToken);
		}

		public MActionResult<IVInvoiceModel> GetInvoiceEditModel(string pkID, string bizType, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetInvoiceEditModel, pkID, bizType, accessToken);
		}

		public MActionResult<IVInvoiceModel> GetInvoiceCopyModel(string pkID, bool isCopyCredit, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetInvoiceCopyModel, pkID, isCopyCredit, accessToken);
		}

		public MActionResult<OperationResult> DeleteInvoiceList(ParamBase param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.DeleteInvoiceList, param, accessToken);
		}

		public MActionResult<OperationResult> PayToInvoice(IVMakePaymentModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.PayToInvoice, model, accessToken);
		}

		public MActionResult<string> GetChartStackedDictionary(string Type, DateTime startDate, DateTime endDate, string contactId = null, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetChartStackedDictionary, Type, startDate, endDate, contactId, accessToken);
		}

		public MActionResult<List<ChartPie2DModel>> GetChartPieDictionary(string Type, DateTime startDate, DateTime endDate, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetChartPieDictionary, Type, startDate, endDate, accessToken);
		}

		public MActionResult<List<IVStatementsModel>> GetStatementData(IVStatementListFilterModel param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetStatementData, param, accessToken);
		}

		public MActionResult<List<IVViewStatementModel>> GetViewStatementData(string contactID, string type, DateTime BeginDate, DateTime EndDate, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetViewStatementData, contactID, type, BeginDate, EndDate, accessToken);
		}

		public MActionResult<List<IVViewStatementModel>> GetViewStatementOpeningBalanceDate(string statementType, string contactId, DateTime beginDate, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetViewStatementOpeningBalanceDate, statementType, contactId, beginDate, null);
		}

		public MActionResult<string> GetOverPastDictionary(string contactID, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetOverPastDictionary, contactID, accessToken);
		}

		public MActionResult<List<IVInvoiceSendModel>> GetSendInvoiceList(IVInvoiceSendParam param, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetSendInvoiceList, param, accessToken);
		}

		public MActionResult<OperationResult> SendInvoiceList(IVInvoiceEmailSendModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.SendInvoiceList, model, accessToken);
		}

		public MActionResult<List<IVEmailTmplPreviewModel>> PreviewEmailTmpl(IVInvoiceEmailSendModel model, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.PreviewEmailTmpl, model, accessToken);
		}

		public MActionResult<OperationResult> ImportInvoiceList(List<IVInvoiceModel> list, bool isFromApi = false, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.ImportInvoiceList, list, isFromApi, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(string invoiceType, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetImportTemplateModel, invoiceType, accessToken);
		}

		public MActionResult<List<BDCheckInactiveModel>> GetInactiveList(string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetInactiveList, accessToken);
		}

		public MActionResult<IVInvoiceModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IIVInvoiceBusiness iIVInvoiceBusiness = biz;
			return base.RunFunc(iIVInvoiceBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}
	}
}
