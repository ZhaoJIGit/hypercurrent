using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASCountryRegion
	{
		[OperationContract]
		MActionResult<List<BASCountryRegionModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null);
	}
}
