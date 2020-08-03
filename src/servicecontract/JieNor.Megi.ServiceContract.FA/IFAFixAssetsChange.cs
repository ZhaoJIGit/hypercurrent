using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FA
{
	[ServiceContract]
	public interface IFAFixAssetsChange
	{
		[OperationContract]
		MActionResult<List<FAFixAssetsChangeModel>> GetFixAssetsChangeLog(FAFixAssetsChangeFilterModel filter = null, string accessToken = null);
	}
}
