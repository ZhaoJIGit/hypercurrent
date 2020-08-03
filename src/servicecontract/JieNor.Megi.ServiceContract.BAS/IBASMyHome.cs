using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASMyHome
	{
		[OperationContract]
		MActionResult<List<BASMyHomeModel>> GetOrgInfoListByUserID(string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BASMyHomeModel>> GetOrgInfoPageListByUserID(BDOrganistationListFilter filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> OrgRegisterForTry(BASOrganisationModel model, string accessToken = null);

		[OperationContract]
		MActionResult<int> DeleteOrgById(string orgId, string accessToken = null);
	}
}
