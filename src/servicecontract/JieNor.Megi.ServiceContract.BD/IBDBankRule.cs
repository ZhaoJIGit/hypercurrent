using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDBankRule
	{
		[OperationContract]
		MActionResult<BDBankRuleModel> GetBDBankRuleModel(string pkId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateBankRule(BDBankRuleModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteBankRule(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BDBankRuleListModel>> GetBankRuleList(BDBankRuleListFilterModel filter, string accessToken = null);
	}
}
