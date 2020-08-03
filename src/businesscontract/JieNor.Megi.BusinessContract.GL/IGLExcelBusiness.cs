using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Formula;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLExcelBusiness : IDataContract<GLExcelModel>
	{
		List<GLVoucherModel> GetVoucherListByFilter(MContext ctx, GLBalanceListFilterModel filter);

		List<GLBalanceModel> GetBalanceListByFilter(MContext ctx, GLBalanceListFilterModel filter);

		List<GLBalanceModel> GetBalanceListWithTrackByFilter(MContext ctx, GLBalanceListFilterModel filter);

		List<GLVoucherModel> GetTransferVoucherList(MContext ctx, GLBalanceListFilterModel filter);

		List<GLInitBalanceModel> GetInitBalanceList(MContext ctx, GLBalanceListFilterModel filter);

		List<BDAccountModel> GetAccountList(MContext ctx);

		DateTime GetGLBeginDate(MContext ctx);

		List<BatchFormaluModel> RefreshFormula(MContext ctx, List<BatchFormaluModel> formaluList);
	}
}
