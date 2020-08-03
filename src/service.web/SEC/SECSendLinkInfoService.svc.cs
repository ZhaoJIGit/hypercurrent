using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.BusinessService.SEC;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.SEC;

namespace JieNor.Megi.Service.Web.SEC
{
	public class SECSendLinkInfoService : ServiceT<SECSendLinkInfoModel>, ISECSendLinkInfo
	{
		private ISECSendLinkInfoBusiness biz = new SECSendLinkInfoBusiness();

		public MActionResult<bool> IsValidLink(string itemid, string accessToken = null)
		{
			ISECSendLinkInfoBusiness iSECSendLinkInfoBusiness = biz;
			return base.RunFunc(iSECSendLinkInfoBusiness.IsValidLink, itemid, accessToken);
		}

		public MActionResult<string> InsertLink(SECSendLinkInfoModel linkModel, string accessToken = null)
		{
			ISECSendLinkInfoBusiness iSECSendLinkInfoBusiness = biz;
			return base.RunAction<string, SECSendLinkInfoModel>(iSECSendLinkInfoBusiness.InsertLink, linkModel, accessToken);
		}

		public MActionResult<string> DeleteLink(string itemid, string accessToken = null)
		{
			ISECSendLinkInfoBusiness iSECSendLinkInfoBusiness = biz;
			return base.RunAction<string, string>(iSECSendLinkInfoBusiness.DeleteLink, itemid, accessToken);
		}

		public MActionResult<SECSendLinkInfoModel> GetModel(string itemId, string accessToken = null)
		{
			ISECSendLinkInfoBusiness iSECSendLinkInfoBusiness = biz;
			return base.RunFunc(iSECSendLinkInfoBusiness.GetModel, itemId, accessToken);
		}
	}
}
