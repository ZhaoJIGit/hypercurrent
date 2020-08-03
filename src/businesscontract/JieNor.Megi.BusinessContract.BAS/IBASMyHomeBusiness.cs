using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASMyHomeBusiness
	{
		MContext GetMContextByOrgID(MContext MContext);

		List<BASMyHomeModel> GetOrgInfoListByUserID(MContext ctx);

		DataGridJson<BASMyHomeModel> GetOrgInfoPageListByUserID(MContext ctx, BDOrganistationListFilter filter);

		OperationResult OrgRegisterForTry(MContext ctx, BASOrganisationModel model);

		int DeleteOrgById(MContext ctx, string orgId);
	}
}
