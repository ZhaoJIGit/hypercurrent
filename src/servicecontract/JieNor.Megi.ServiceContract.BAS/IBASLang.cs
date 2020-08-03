using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASLang
	{
		[OperationContract]
		MActionResult<List<BASLangModel>> GetOrgLangList(string accessToken = null);

		[OperationContract]
		MActionResult<List<BASLangModel>> GetSysLangList(string accessToken = null);

		[OperationContract]
		MActionResult<List<string>> GetOrgLangIDList(string accessToken = null);

		[OperationContract]
		MActionResult<string> GetClientGlobalInfo(string accessToken = null);

		[OperationContract]
		MActionResult<Dictionary<LangModule, Dictionary<string, string>>> GetStringKeyLangList(string langId, string accessToken = null);

		[OperationContract]
		MActionResult<Dictionary<LangModule, Dictionary<int, string>>> GetIntKeyLangList(string langId, string accessToken = null);

		[OperationContract]
		MActionResult<string> UpdateLang(LangModule module, string key, string defaultValue, int type, string accessToken = null);

		[OperationContract]
		MActionResult<string> UpdateScriptLang(LangModule langModule, string key, string defaultValue, string accessToken = null);

		[OperationContract]
		MActionResult<object> GetText(LangModule module, string key, string accessToken = null);
	}
}
