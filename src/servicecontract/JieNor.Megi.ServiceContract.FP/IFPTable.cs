using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FP
{
	[ServiceContract]
	public interface IFPTable
	{
		[OperationContract]
		MActionResult<FPTableViewModel> GetTableViewModel(string tableId = null, string invoiceIds = null, int invoiceType = 0, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FPTableViewModel>> GetTableViewModelPageList(FPTableViewFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<FPFapiaoModel>> GetFapiaoListByTableInvoice(string tableId, string invoiceIds, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteTableByInvoiceIds(string invoiceIds, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> FPAddLog(FPFapiaoModel Model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteTableByTableIds(string tableIds, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveTable(FPTableViewModel table, string accessToken = null);

		[OperationContract]
		MActionResult<List<NameValueModel>> GetTableHomeData(int invoiceType, DateTime date, string accessToken = null);

		[OperationContract]
		MActionResult<FPTableViewModel> GetTableViewModelByInvoiceID(string invoiceId, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetChartStackedDictionary(int fapiaoType, DateTime startDate, DateTime endDate, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetDashboardTableData(string accessToken = null);
	}
}
