using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.PA
{
	public interface IPAPaySettingBll
	{
		OperationResult InsertOrUpdate(MContext ctx, PAPaySettingModel model);

		PAPaySettingModel GetModel(MContext ctx);
	}
}
