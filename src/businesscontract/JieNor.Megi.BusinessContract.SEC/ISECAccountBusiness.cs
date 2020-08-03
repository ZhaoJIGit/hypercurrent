using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.SEC
{
	public interface ISECAccountBusiness
	{
		OperationResult UpdateAccountData(MContext ctx, SECUserModel model, int type);

		SECUserModel GetUserModelByKey(MContext ctx, string MItemID);

		OperationResult LoginForUpdateEmail(MContext ctx, string[] token);
	}
}
