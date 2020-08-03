using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IO.Import.PA;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.PA
{
	public interface IPASalaryPaymentBusiness
	{
		List<PAPayRunListModel> GetPayRunList(MContext ctx, PAPayRunListFilterModel filter);

		DataGridJson<PAPayRunListModel> GetPayRunListPage(MContext ctx, PAPayRunListFilterModel filter);

		string GetChartStackedDictionary(MContext ctx, string payRunListData);

		List<ChartPie2DModel> GetChartPieDictionary(MContext ctx, DateTime startDate, DateTime endDate);

		PASalaryPaymentModel GetSalaryPaymentEditModel(MContext ctx, string mid);

		DataGridJson<PASalaryPaymentListModel> GetSalaryPaymentList(MContext ctx, PASalaryPaymentListFilterModel filter);

		List<PASalaryPaymentTreeModel> GetSalaryPaymentPersonDetails(MContext ctx, string salaryPayId);

		OperationResult SalaryPaymentUpdate(MContext ctx, PASalaryPaymentModel spModel);

		PAPayRunModel GetPayRunModel(MContext ctx, string id);

		string GetSalaryPaymentListByVerificationId(MContext ctx, string id);

		OperationResult ValidatePayRunAction(MContext ctx, string yearMonth, PayRunSourceEnum source);

		OperationResult PayRunUpdate(MContext ctx, PAPayRunModel model, List<string> updateFields = null);

		OperationResult PayRunNew(MContext ctx, string yearMonth);

		OperationResult PayRunCopy(MContext ctx, string yearMonth);

		OperationResult SalaryPaymentListUpdate(MContext ctx, string runId, string employeeIds);

		List<PAEmployeesListModel> GetUnPayEmployeeList(MContext ctx, string runId);

		OperationResult SalaryPaymentDelete(MContext ctx, string salaryPaymentIds);

		List<PASalaryListModel> GetSalaryListForPrint(MContext ctx, PASalaryListFilterModel filter);

		ImportTemplateModel GetImportTemplateModel(MContext ctx, DateTime period);

		OperationResult ImportSalaryList(MContext ctx, ImportSalaryModel model);

		PASalaryPaymentSummaryModel GetSalaryPaymentSummaryModelByStatus(MContext ctx);

		PASalaryPaymentSummaryModel GetSalaryPaymentSummaryModel(MContext ctx, string runId);

		OperationResult UnApproveSalaryPayment(MContext ctx, string ids);

		OperationResult PaySalary(MContext ctx, IVMakePaymentModel model);

		List<PAPITThresholdModel> GetPITThresholdList(MContext ctx, PAPITThresholdFilterModel filter);

		List<NameValueModel> GetSalaryHeader(MContext ctx);
	}
}
