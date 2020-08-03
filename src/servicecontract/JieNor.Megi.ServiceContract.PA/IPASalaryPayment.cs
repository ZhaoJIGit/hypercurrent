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
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.PA
{
	[ServiceContract]
	public interface IPASalaryPayment
	{
		[OperationContract]
		MActionResult<List<PAPayRunListModel>> GetPayRunList(PAPayRunListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<PAPayRunListModel>> GetPayRunListPage(PAPayRunListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetChartStackedDictionary(string payRunListData, string accessToken = null);

		[OperationContract]
		MActionResult<List<ChartPie2DModel>> GetChartPieDictionary(DateTime startDate, DateTime endDate, string accessToken = null);

		[OperationContract]
		MActionResult<PASalaryPaymentModel> GetSalaryPaymentEditModel(string mid, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<PASalaryPaymentListModel>> GetSalaryPaymentList(PASalaryPaymentListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<PASalaryPaymentTreeModel>> GetSalaryPaymentPersonDetails(string salaryPayId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SalaryPaymentUpdate(PASalaryPaymentModel spModel, string accessToken = null);

		[OperationContract]
		MActionResult<PAPayRunModel> GetPayRunModel(string id, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetSalaryPaymentListByVerificationId(string id, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ValidatePayRunAction(string yearMonth, PayRunSourceEnum source, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> PayRunUpdate(PAPayRunModel model, List<string> updateFields = null, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> PayRunNew(string yearMonth, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> PayRunCopy(string yearMonth, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SalaryPaymentListUpdate(string runId, string employeeIds, string accessToken = null);

		[OperationContract]
		MActionResult<List<PAEmployeesListModel>> GetUnPayEmployeeList(string runId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SalaryPaymentDelete(string salaryPaymentIds, string accessToken = null);

		[OperationContract]
		MActionResult<List<PASalaryListModel>> GetSalaryListForPrint(PASalaryListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(DateTime period, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportSalaryList(ImportSalaryModel model, string accessToken = null);

		[OperationContract]
		MActionResult<PASalaryPaymentSummaryModel> GetSalaryPaymentSummaryModelByStatus(string accessToken = null);

		[OperationContract]
		MActionResult<PASalaryPaymentSummaryModel> GetSalaryPaymentSummaryModel(string runId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UnApproveSalaryPayment(string ids, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> PaySalary(IVMakePaymentModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<PAPITThresholdModel>> GetPITThresholdList(PAPITThresholdFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<NameValueModel>> GetSalaryHeader(string accessToken = null);
	}
}
