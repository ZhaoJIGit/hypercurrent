using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLInitBalanceBusiness : IDataContract<GLInitBalanceModel>
	{
		GLInitBalanceModel GetBankInitBalance(MContext ctx, string accountId, string bankName);

		OperationResult ClearInitBalance(MContext ctx, string initBalanceId = null);

		List<GLInitBalanceModel> GetInitBalanceList(MContext ctx, GLInitBalanceListFilterModel filter);

		OperationResult SaveInitBalance(MContext ctx, List<GLInitBalanceModel> initBalanceList);

		List<ImportTemplateModel> GetImportTemplateModel(MContext ctx);

		List<GLInitBalanceModel> GetCompleteInitBalanceList(MContext ctx, GLInitBalanceListFilterModel filter);

		OperationResult ImportInitBalanceList(MContext ctx, List<GLInitBalanceModel> list);

		OperationResult CheckAutoCreateBillHadVerifiyRecord(MContext ctx);

		OperationResult ValidateData(MContext ctx, List<GLInitBalanceModel> list);
	}
}
