using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASLangBusiness
	{
		[NoAuthorization]
		List<BASLangModel> GetOrgLangList(MContext context);

		[NoAuthorization]
		List<BASLangModel> GetSysLangList(MContext context);

		[NoAuthorization]
		List<string> GetOrgLangIDList(MContext context);

		[NoAuthorization]
		string GetClientGlobalInfo(MContext ctx);

		[NoAuthorization]
		Dictionary<LangModule, Dictionary<string, string>> GetStringKeyLangList(MContext ctx, string langId);

		[NoAuthorization]
		Dictionary<LangModule, Dictionary<int, string>> GetIntKeyLangList(MContext ctx, string langId);

		[NoAuthorization]
		void UpdateLang(MContext ctx, LangModule module, string key, string defaultValue, int type);

		[NoAuthorization]
		void UpdateScriptLang(MContext ctx, LangModule langModule, string key, string defaultValue);

		[NoAuthorization]
		object GetText(MContext ctx, LangModule module, string key);

		[NoAuthorization]
		string GetText(MContext ctx, LangModule langModule, string key, string defaultValue);
	}
}
