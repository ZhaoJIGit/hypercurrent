using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDEmployees
	{
		[OperationContract]
		MActionResult<List<BDEmployeesListModel>> GetBDEmployeesList(string filterString, bool includeDisable = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDEmployeesModel>> GetEmployeeList(bool includeDisable = true, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BDEmployeesListModel>> GetBDEmployeesPageList(BDEmployeesListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDEmployeesListModel>> GetEmployeeListForExport(BDEmployeesListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<BDEmployeesModel> GetEmployeesEditInfo(string employeeId, string accessToken = null);

		[OperationContract]
		MActionResult<BDEmployeesModel> GetEmployeeInfo(string employeeId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> EmployeesUpdate(BDEmployeesModel info, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDEmployeesModel>> GetOrgUserList(string accessToken = null);

		[OperationContract]
		MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteEmployee(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ArchiveEmployee(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> IsImportEmployeeNamesExist(List<BDEmployeesModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportEmployeeList(List<BDEmployeesModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> RestoreEmployee(ParamBase param, string accessToken = null);
	}
}
