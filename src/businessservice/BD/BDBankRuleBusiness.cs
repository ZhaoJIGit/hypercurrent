using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDBankRuleBusiness : IBDBankRuleBusiness
	{
		private readonly BDBankRuleRepository _bankRule = new BDBankRuleRepository();

		public BDBankRuleModel GetBDBankRuleModel(MContext ctx, string pkId)
		{
			return _bankRule.GetDataModel(ctx, pkId, false);
		}

		public OperationResult UpdateBankRule(MContext ctx, BDBankRuleModel model)
		{
			return _bankRule.UpdateBankRule(ctx, model);
		}

		public OperationResult DeleteBankRule(MContext ctx, ParamBase param)
		{
			if (string.IsNullOrWhiteSpace(param.KeyIDs))
			{
				throw new Exception("Error Paramenter");
			}
			return _bankRule.DeleteModels(ctx, param.KeyIDs.Split(',').ToList());
		}

		public DataGridJson<BDBankRuleListModel> GetBankRuleList(MContext ctx, BDBankRuleListFilterModel filter)
		{
			return _bankRule.GetBankRuleList(ctx, filter);
		}
	}
}
