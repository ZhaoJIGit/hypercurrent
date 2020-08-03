using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASTimezone
	{
		[OperationContract]
		MActionResult<List<BASTimezoneModel>> GetList(string accessToken = null);
	}
}
