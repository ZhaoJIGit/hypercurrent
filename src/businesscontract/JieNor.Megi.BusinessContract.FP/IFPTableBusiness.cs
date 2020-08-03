using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FP
{
	public interface IFPTableBusiness : IDataContract<FPTableModel>
	{
		FPTableViewModel GetTableViewModel(MContext ctx, string tableId = null, string invoiceIds = null, int invoiceType = 0);

		DataGridJson<FPTableViewModel> GetTableViewModelPageList(MContext ctx, FPTableViewFilterModel filter);

		List<FPFapiaoModel> GetFapiaoListByTableInvoice(MContext ctx, string tableId, string invoiceIds);

		OperationResult DeleteTableByInvoiceIds(MContext ctx, string invoiceIds);

		OperationResult DeleteTableByTableIds(MContext ctx, string tableIds);

		OperationResult SaveTable(MContext ctx, FPTableViewModel table);

		List<NameValueModel> GetTableHomeData(MContext ctx, int invoiceType, DateTime date);

		FPTableViewModel GetTableViewModelByInvoiceID(MContext ctx, string invoiceId);

		OperationResult FPAddLog(MContext ctx, FPFapiaoModel invoiceId);

		string GetChartStackedDictionary(MContext ctx, int fapiaoType, DateTime startDate, DateTime endDate);

		string GetDashboardTableData(MContext ctx);
	}
}
