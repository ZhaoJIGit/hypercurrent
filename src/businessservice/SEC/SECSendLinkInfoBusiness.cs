using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.SEC
{
	public class SECSendLinkInfoBusiness : ISECSendLinkInfoBusiness
	{
		[NoAuthorization]
		public bool IsValidLink(MContext ctx, string itemid)
		{
			return SECSendLinkInfoRepository.IsValidLink(itemid);
		}

		[NoAuthorization]
		public void InsertLink(MContext ctx, SECSendLinkInfoModel linkModel)
		{
			SECSendLinkInfoRepository.InsertLink(linkModel.MItemID, linkModel.MEmail, linkModel.MSendDate, linkModel.MPhone, linkModel.MFirstName, linkModel.MLastName, linkModel.MLinkType, linkModel.MInvitationEmail, linkModel.MInvitationOrgID,linkModel.PlanCode);
		}

		[NoAuthorization]
		public void DeleteLink(MContext ctx, string itemid)
		{
			SECSendLinkInfoRepository.DeleteLink(itemid);
		}

		[NoAuthorization]
		public SECSendLinkInfoModel GetModel(MContext ctx, string itemId)
		{
			return SECSendLinkInfoRepository.GetModel(itemId);
		}
	}
}
