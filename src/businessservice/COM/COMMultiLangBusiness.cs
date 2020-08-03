using JieNor.Megi.BusinessContract.COM;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.COM
{
	public class COMMultiLangBusiness : ICOMMultiLangBusiness
	{
		[NoAuthorization]
		public void UpdateLang(MContext ctx, LangModule module, string key, string defaultValue, int type)
		{
			COMMultiLangRepository.UpdateLang(module, key, defaultValue, type);
		}

		[NoAuthorization]
		public List<KeyValuePair<string, Hashtable>> GetStringKeyLangTableList(MContext ctx, string[] localeIds = null)
		{
			return COMMultiLangRepository.GetStringKeyLangTableList(localeIds);
		}

		[NoAuthorization]
		public List<KeyValuePair<string, Hashtable>> GetIntKeyLangTableList(MContext ctx, string[] localeIds = null)
		{
			return COMMultiLangRepository.GetIntKeyLangTableList(localeIds);
		}

		[NoAuthorization]
		public string GetText(MContext ctx, LangModule module, string localeId, string key)
		{
			return COMMultiLangRepository.GetText(module, localeId, key);
		}

		[NoAuthorization]
		public string GetTextByStringKey(MContext ctx, string localeId, LangModule module, string key, string defaultValue)
		{
			return COMMultiLangRepository.GetText(localeId, module, key, defaultValue);
		}
	}
}
