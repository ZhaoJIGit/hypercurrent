using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDEmployeesService : ServiceT<BDAccountModel>, IBDEmployees
	{
		private readonly IBDEmployeesBusiness biz = new BDEmployeesBusiness();

		public MActionResult<List<BDEmployeesListModel>> GetBDEmployeesList(string filterString, bool includeDisable = false, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.GetBDEmployeesList, filterString, includeDisable, accessToken);
		}

		public MActionResult<List<BDEmployeesModel>> GetEmployeeList(bool includeDisable = true, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.GetEmployeeList, includeDisable, accessToken);
		}

		public MActionResult<DataGridJson<BDEmployeesListModel>> GetBDEmployeesPageList(BDEmployeesListFilterModel filter, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.GetBDEmployeesPageList, filter, accessToken);
		}

		public MActionResult<List<BDEmployeesListModel>> GetEmployeeListForExport(BDEmployeesListFilterModel filter, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.GetEmployeeListForExport, filter, accessToken);
		}

		public MActionResult<BDEmployeesModel> GetEmployeesEditInfo(string employeeId, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.GetEmployeesEditInfo, employeeId, accessToken);
		}

		public MActionResult<BDEmployeesModel> GetEmployeeInfo(string employeeId, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.GetEmployeeInfo, employeeId, accessToken);
		}

		public MActionResult<OperationResult> EmployeesUpdate(BDEmployeesModel info, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.EmployeesUpdate, info, accessToken);
		}

		public MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.IsCanDeleteOrInactive, param, accessToken);
		}

		public MActionResult<OperationResult> DeleteEmployee(ParamBase param, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.DeleteEmployee, param, accessToken);
		}

		public MActionResult<OperationResult> ArchiveEmployee(ParamBase param, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.ArchiveEmployee, param, accessToken);
		}

		public MActionResult<List<BDEmployeesModel>> GetOrgUserList(string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.GetOrgUserList, accessToken);
		}

		public MActionResult<OperationResult> IsImportEmployeeNamesExist(List<BDEmployeesModel> list, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.IsImportEmployeeNamesExist, list, accessToken);
		}

		public MActionResult<OperationResult> ImportEmployeeList(List<BDEmployeesModel> list, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.ImportEmployeeList, list, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.GetImportTemplateModel, accessToken);
		}

		public MActionResult<OperationResult> RestoreEmployee(ParamBase param, string accessToken = null)
		{
			IBDEmployeesBusiness iBDEmployeesBusiness = biz;
			return base.RunFunc(iBDEmployeesBusiness.RestoreEmployee, param, accessToken);
		}
	}
}
