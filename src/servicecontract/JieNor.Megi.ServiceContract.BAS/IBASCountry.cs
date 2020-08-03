using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASCountry
	{
		[OperationContract]
		MActionResult<List<BASCountryModel>> GetCountryList(string accessToken = null);

		[OperationContract]
		MActionResult<List<BASProvinceModel>> GetProvinceList(string countryId, string accessToken = null);
	}
}
