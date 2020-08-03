using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASMyHomeService : ServiceT<BASMyHomeModel>, IBASMyHome
	{
		private IBASMyHomeBusiness biz = new BASMyHomeBusiness();

		public MContext GetMContextByOrgID(string accessToken = null)
		{
			return MContextManager.GetMContextByAccessToken(accessToken, "System");
		}

		public MActionResult<List<BASMyHomeModel>> GetOrgInfoListByUserID(string accessToken = null)
		{
			IBASMyHomeBusiness iBASMyHomeBusiness = biz;
			return base.RunFunc(iBASMyHomeBusiness.GetOrgInfoListByUserID, accessToken);
		}

		public MActionResult<DataGridJson<BASMyHomeModel>> GetOrgInfoPageListByUserID(BDOrganistationListFilter filter, string accessToken = null)
		{
			IBASMyHomeBusiness iBASMyHomeBusiness = biz;
			return base.RunFunc(iBASMyHomeBusiness.GetOrgInfoPageListByUserID, filter, accessToken);
		}

		public MActionResult<OperationResult> OrgRegisterForTry(BASOrganisationModel model, string accessToken = null)
		{
			IBASMyHomeBusiness iBASMyHomeBusiness = biz;
			return base.RunFunc(iBASMyHomeBusiness.OrgRegisterForTry, model, accessToken);
		}

		public MActionResult<int> DeleteOrgById(string orgId, string accessToken = null)
		{
			IBASMyHomeBusiness iBASMyHomeBusiness = biz;
			return base.RunFunc(iBASMyHomeBusiness.DeleteOrgById, orgId, accessToken);
		}
	}
}
