using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASSearch
	{
		[OperationContract]
		MActionResult<DataGridJson<BASSearchModel>> GetSearchResult(BASSearchFilterModel filter, string accessToken = null);
	}
}
