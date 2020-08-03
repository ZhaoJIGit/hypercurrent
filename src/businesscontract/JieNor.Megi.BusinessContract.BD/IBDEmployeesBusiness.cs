using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDEmployeesBusiness
	{
		List<BDEmployeesListModel> GetBDEmployeesList(MContext ctx, string filterString, bool includeDisable = false);

		List<BDEmployeesModel> GetEmployeeList(MContext ctx, bool includeDisable = true);

		DataGridJson<BDEmployeesListModel> GetBDEmployeesPageList(MContext ctx, BDEmployeesListFilterModel filter);

		List<BDEmployeesListModel> GetEmployeeListForExport(MContext ctx, BDEmployeesListFilterModel filter);

		BDEmployeesModel GetEmployeesEditInfo(MContext ctx, string employeeId);

		BDEmployeesModel GetEmployeeInfo(MContext ctx, string employeeId);

		OperationResult EmployeesUpdate(MContext ctx, BDEmployeesModel info);

		List<BDEmployeesModel> GetOrgUserList(MContext ctx);

		BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param);

		OperationResult DeleteEmployee(MContext ctx, ParamBase param);

		OperationResult ArchiveEmployee(MContext ctx, ParamBase param);

		OperationResult IsImportEmployeeNamesExist(MContext ctx, List<BDEmployeesModel> list);

		OperationResult ImportEmployeeList(MContext ctx, List<BDEmployeesModel> list);

		ImportTemplateModel GetImportTemplateModel(MContext ctx);

		OperationResult RestoreEmployee(MContext ctx, ParamBase param);
	}
}
