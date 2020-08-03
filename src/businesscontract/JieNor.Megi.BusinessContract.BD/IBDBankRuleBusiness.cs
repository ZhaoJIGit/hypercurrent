using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDBankRuleBusiness
	{
		BDBankRuleModel GetBDBankRuleModel(MContext ctx, string pkId);

		OperationResult UpdateBankRule(MContext ctx, BDBankRuleModel model);

		OperationResult DeleteBankRule(MContext ctx, ParamBase param);

		DataGridJson<BDBankRuleListModel> GetBankRuleList(MContext ctx, BDBankRuleListFilterModel filter);
	}
}
