using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDTrackEntry
	{
		[OperationContract]
		MActionResult<BDTrackEntryModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDTrackEntryModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null);
	}
}
