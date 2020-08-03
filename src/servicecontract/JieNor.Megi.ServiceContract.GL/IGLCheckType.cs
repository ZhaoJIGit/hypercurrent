using JieNor.Megi.Core;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.GL
{
	[ServiceContract]
	public interface IGLCheckType
	{
		[OperationContract]
		MActionResult<GLCheckTypeDataModel> GetCheckTypeDataByType(int type, bool includeDisabled = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLCheckTypeModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null);
	}
}
