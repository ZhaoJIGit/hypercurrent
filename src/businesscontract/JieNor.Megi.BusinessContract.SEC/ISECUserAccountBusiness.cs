using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.SEC;

namespace JieNor.Megi.BusinessContract.SEC
{
	public interface ISECUserAccountBusiness
	{
		SECUserModel GetUserModel(string email, string password);

		SECLoginResultModel Login(MContext ctx, SECLoginModel model);

		MContext GetMContextByOrgID(MContext MContext);
	}
}
