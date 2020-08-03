using JieNor.Megi.BusinessContract.PA;
using JieNor.Megi.BusinessService.PA;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IO.Import.PA;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ServiceContract.PA;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.PA
{
	public class PASalaryPaymentService : ServiceT<PAPayRunListModel>, IPASalaryPayment
	{
		private readonly IPASalaryPaymentBusiness biz = new PASalaryPaymentBusiness();

		public MActionResult<List<PAPayRunListModel>> GetPayRunList(PAPayRunListFilterModel filter, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetPayRunList, filter, accessToken);
		}

		public MActionResult<DataGridJson<PAPayRunListModel>> GetPayRunListPage(PAPayRunListFilterModel filter, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetPayRunListPage, filter, accessToken);
		}

		public MActionResult<string> GetChartStackedDictionary(string payRunListData, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetChartStackedDictionary, payRunListData, accessToken);
		}

		public MActionResult<List<ChartPie2DModel>> GetChartPieDictionary(DateTime startDate, DateTime endDate, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetChartPieDictionary, startDate, endDate, accessToken);
		}

		public MActionResult<PASalaryPaymentModel> GetSalaryPaymentEditModel(string mid, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetSalaryPaymentEditModel, mid, accessToken);
		}

		public MActionResult<DataGridJson<PASalaryPaymentListModel>> GetSalaryPaymentList(PASalaryPaymentListFilterModel filter, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetSalaryPaymentList, filter, accessToken);
		}

		public MActionResult<PAPayRunModel> GetPayRunModel(string id, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetPayRunModel, id, accessToken);
		}

		public MActionResult<string> GetSalaryPaymentListByVerificationId(string id, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetSalaryPaymentListByVerificationId, id, accessToken);
		}

		public MActionResult<List<PASalaryPaymentTreeModel>> GetSalaryPaymentPersonDetails(string salaryPayId, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetSalaryPaymentPersonDetails, salaryPayId, accessToken);
		}

		public MActionResult<OperationResult> SalaryPaymentUpdate(PASalaryPaymentModel spModel, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.SalaryPaymentUpdate, spModel, accessToken);
		}

		public MActionResult<OperationResult> ValidatePayRunAction(string yearMonth, PayRunSourceEnum source, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.ValidatePayRunAction, yearMonth, source, accessToken);
		}

		public MActionResult<OperationResult> PayRunUpdate(PAPayRunModel model, List<string> updateFields = null, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.PayRunUpdate, model, updateFields, accessToken);
		}

		public MActionResult<OperationResult> PayRunNew(string yearMonth, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.PayRunNew, yearMonth, accessToken);
		}

		public MActionResult<OperationResult> PayRunCopy(string yearMonth, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.PayRunCopy, yearMonth, accessToken);
		}

		public MActionResult<OperationResult> SalaryPaymentListUpdate(string runId, string employeeIds, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.SalaryPaymentListUpdate, runId, employeeIds, accessToken);
		}

		public MActionResult<List<PAEmployeesListModel>> GetUnPayEmployeeList(string runId, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetUnPayEmployeeList, runId, accessToken);
		}

		public MActionResult<OperationResult> SalaryPaymentDelete(string salaryPaymentIds, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.SalaryPaymentDelete, salaryPaymentIds, accessToken);
		}

		public MActionResult<List<PASalaryListModel>> GetSalaryListForPrint(PASalaryListFilterModel filter, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetSalaryListForPrint, filter, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(DateTime period, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetImportTemplateModel, period, accessToken);
		}

		public MActionResult<OperationResult> ImportSalaryList(ImportSalaryModel model, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.ImportSalaryList, model, accessToken);
		}

		public MActionResult<PASalaryPaymentSummaryModel> GetSalaryPaymentSummaryModelByStatus(string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetSalaryPaymentSummaryModelByStatus, accessToken);
		}

		public MActionResult<PASalaryPaymentSummaryModel> GetSalaryPaymentSummaryModel(string runId, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetSalaryPaymentSummaryModel, runId, accessToken);
		}

		public MActionResult<OperationResult> UnApproveSalaryPayment(string ids, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.UnApproveSalaryPayment, ids, accessToken);
		}

		public MActionResult<OperationResult> PaySalary(IVMakePaymentModel model, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.PaySalary, model, accessToken);
		}

		public MActionResult<List<PAPITThresholdModel>> GetPITThresholdList(PAPITThresholdFilterModel filter, string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetPITThresholdList, filter, accessToken);
		}

		public MActionResult<List<NameValueModel>> GetSalaryHeader(string accessToken = null)
		{
			IPASalaryPaymentBusiness iPASalaryPaymentBusiness = biz;
			return base.RunFunc(iPASalaryPaymentBusiness.GetSalaryHeader, accessToken);
		}
	}
}
