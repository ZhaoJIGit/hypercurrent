using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASDataDictionaryBusiness
	{
		List<BASDataDictionaryModel> GetDictListByValues(MContext context, string dictType, List<string> filterValues);

		List<BASDataDictionaryModel> GetDictList(MContext context, string dictType);
	}
}
