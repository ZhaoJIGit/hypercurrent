using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.COM
{
	[ServiceContract]
	public interface ICOMMultiLang
	{
		[OperationContract]
		MActionResult<List<KeyValuePair<string, Hashtable>>> GetStringKeyLangTableList(string[] localeIds = null, string accessToken = null);

		[OperationContract]
		MActionResult<List<KeyValuePair<string, Hashtable>>> GetIntKeyLangTableList(string[] localeIds = null, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetText(LangModule module, string localeId, string key, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetTextByStringKey(string localeId, LangModule module, string key, string defaultValue, string accessToken = null);
	}
}
