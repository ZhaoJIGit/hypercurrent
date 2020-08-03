using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.REG
{
	public interface IREGGlobalizationBusiness
	{
		OperationResult GlobalizationUpdate(MContext ctx, REGGlobalizationModel model);

		REGGlobalizationModel GetOrgGlobalizationDetail(MContext ctx, string orgid);
	}
}
