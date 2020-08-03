using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASDataDictionary
	{
		[OperationContract]
		MActionResult<List<BASDataDictionaryModel>> GetDictListByValues(string dictType, List<string> filterValues, string accessToken = null);

		[OperationContract]
		MActionResult<List<BASDataDictionaryModel>> GetDictList(string dictType, string accessToken = null);
	}
}
