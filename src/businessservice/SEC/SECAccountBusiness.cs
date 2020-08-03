using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.SEC
{
	public class SECAccountBusiness : ISECAccountBusiness
	{
		private BASAccountRepository dal = new BASAccountRepository();

		public OperationResult UpdateAccountData(MContext ctx, SECUserModel model, int type)
		{
			return dal.UpdateAccountData(ctx, model, type);
		}

		public SECUserModel GetUserModelByKey(MContext ctx, string MItemID)
		{
			return dal.GetUserModelByKey(MItemID);
		}

		[NoAuthorization]
		public OperationResult LoginForUpdateEmail(MContext ctx, string[] token)
		{
			return dal.LoginForUpdateEmail(token);
		}
	}
}
