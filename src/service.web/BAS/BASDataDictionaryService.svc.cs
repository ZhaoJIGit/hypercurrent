using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASDataDictionaryService : ServiceT<BASDataDictionaryModel>, IBASDataDictionary
	{
		public readonly IBASDataDictionaryBusiness biz = new BASDataDictionaryBusiness();

		public MActionResult<List<BASDataDictionaryModel>> GetDictListByValues(string dictType, List<string> filterValues, string accessToken = null)
		{
			IBASDataDictionaryBusiness iBASDataDictionaryBusiness = biz;
			return base.RunFunc(iBASDataDictionaryBusiness.GetDictListByValues, dictType, filterValues, accessToken);
		}

		public MActionResult<List<BASDataDictionaryModel>> GetDictList(string dictType, string accessToken = null)
		{
			IBASDataDictionaryBusiness iBASDataDictionaryBusiness = biz;
			return base.RunFunc(iBASDataDictionaryBusiness.GetDictList, dictType, accessToken);
		}
	}
}
