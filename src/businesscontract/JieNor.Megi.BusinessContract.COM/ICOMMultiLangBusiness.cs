using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.COM
{
	public interface ICOMMultiLangBusiness
	{
		void UpdateLang(MContext ctx, LangModule module, string key, string defaultValue, int type);

		List<KeyValuePair<string, Hashtable>> GetStringKeyLangTableList(MContext ctx, string[] localeIds = null);

		List<KeyValuePair<string, Hashtable>> GetIntKeyLangTableList(MContext ctx, string[] localeIds = null);

		string GetText(MContext ctx, LangModule module, string localeId, string key);

		string GetTextByStringKey(MContext ctx, string localeId, LangModule module, string key, string defaultValue);
	}
}
