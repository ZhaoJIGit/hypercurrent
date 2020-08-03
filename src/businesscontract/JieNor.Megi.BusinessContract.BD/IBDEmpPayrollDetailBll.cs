using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDEmpPayrollDetailBll
	{
		OperationResult InsertOrUpdate(MContext ctx, BDPayrollDetailModel model);

		BDPayrollDetailModel GetModel(MContext ctx, string employeeID);

		List<BDPayrollDetailModel> GetList(MContext ctx, string employeeIds);
	}
}
