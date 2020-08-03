using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDBankRuleService : ServiceT<BDBankRuleModel>, IBDBankRule
	{
		private readonly IBDBankRuleBusiness biz = new BDBankRuleBusiness();

		public MActionResult<BDBankRuleModel> GetBDBankRuleModel(string pkId, string accessToken = null)
		{
			IBDBankRuleBusiness iBDBankRuleBusiness = biz;
			return base.RunFunc(iBDBankRuleBusiness.GetBDBankRuleModel, pkId, accessToken);
		}

		public MActionResult<OperationResult> UpdateBankRule(BDBankRuleModel model, string accessToken = null)
		{
			IBDBankRuleBusiness iBDBankRuleBusiness = biz;
			return base.RunFunc(iBDBankRuleBusiness.UpdateBankRule, model, accessToken);
		}

		public MActionResult<OperationResult> DeleteBankRule(ParamBase param, string accessToken = null)
		{
			IBDBankRuleBusiness iBDBankRuleBusiness = biz;
			return base.RunFunc(iBDBankRuleBusiness.DeleteBankRule, param, accessToken);
		}

		public MActionResult<DataGridJson<BDBankRuleListModel>> GetBankRuleList(BDBankRuleListFilterModel filter, string accessToken = null)
		{
			IBDBankRuleBusiness iBDBankRuleBusiness = biz;
			return base.RunFunc(iBDBankRuleBusiness.GetBankRuleList, filter, accessToken);
		}
	}
}
