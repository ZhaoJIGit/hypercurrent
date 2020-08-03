using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.SEC
{
	public interface ISECSendLinkInfoBusiness
	{
		bool IsValidLink(MContext ctx, string itemid);

		void InsertLink(MContext ctx, SECSendLinkInfoModel linkModel);

		void DeleteLink(MContext ctx, string itemid);

		SECSendLinkInfoModel GetModel(MContext ctx, string itemId);
	}
}
