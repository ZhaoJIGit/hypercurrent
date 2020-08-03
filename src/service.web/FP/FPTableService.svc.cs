using JieNor.Megi.BusinessContract.FP;
using JieNor.Megi.BusinessService.FP;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FP;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FP
{
	public class FPTableService : ServiceT<FPTableModel>, IFPTable
	{
		private readonly IFPTableBusiness biz = new FPTableBusiness();

		public MActionResult<FPTableViewModel> GetTableViewModel(string tableId = null, string invoiceIds = null, int invoiceType = 0, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.GetTableViewModel, tableId, invoiceIds, invoiceType, accessToken);
		}

		public MActionResult<DataGridJson<FPTableViewModel>> GetTableViewModelPageList(FPTableViewFilterModel filter, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.GetTableViewModelPageList, filter, accessToken);
		}

		public MActionResult<List<FPFapiaoModel>> GetFapiaoListByTableInvoice(string tableId, string invoiceIds, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.GetFapiaoListByTableInvoice, tableId, invoiceIds, accessToken);
		}

		public MActionResult<OperationResult> DeleteTableByInvoiceIds(string invoiceIds, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.DeleteTableByInvoiceIds, invoiceIds, accessToken);
		}

		public MActionResult<OperationResult> FPAddLog(FPFapiaoModel invoiceIds, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.FPAddLog, invoiceIds, accessToken);
		}

		public MActionResult<OperationResult> DeleteTableByTableIds(string tableIds, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.DeleteTableByTableIds, tableIds, accessToken);
		}

		public MActionResult<OperationResult> SaveTable(FPTableViewModel table, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.SaveTable, table, accessToken);
		}

		public MActionResult<List<NameValueModel>> GetTableHomeData(int invoiceType, DateTime date, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.GetTableHomeData, invoiceType, date, accessToken);
		}

		public MActionResult<FPTableViewModel> GetTableViewModelByInvoiceID(string invoiceId, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.GetTableViewModelByInvoiceID, invoiceId, accessToken);
		}

		public MActionResult<string> GetChartStackedDictionary(int fapiaoType, DateTime startDate, DateTime endDate, string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.GetChartStackedDictionary, fapiaoType, startDate, endDate, accessToken);
		}

		public MActionResult<string> GetDashboardTableData(string accessToken = null)
		{
			IFPTableBusiness iFPTableBusiness = biz;
			return base.RunFunc(iFPTableBusiness.GetDashboardTableData, accessToken);
		}
	}
}
