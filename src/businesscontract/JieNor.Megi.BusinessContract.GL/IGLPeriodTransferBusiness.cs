using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLPeriodTransferBusiness : IDataContract<GLPeriodTransferModel>
	{
		GLPeriodTransferModel CalculatePeriodTransfer(MContext ctx, GLPeriodTransferModel model);

		GLPeriodTransferModel GetPeriodTransfer(MContext ctx, GLPeriodTransferModel model, bool test = true);

		List<GLPeriodTransferModel> GetExsitsAndCalculatedPeriodTransfer(MContext ctx, GLPeriodTransferModel model);
	}
}
