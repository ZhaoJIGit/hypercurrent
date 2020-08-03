using JieNor.Megi.BusinessContract.COM;
using JieNor.Megi.BusinessService.COM;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.ServiceContract.COM;
using System.Collections;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.COM
{
	public class COMMultiLangService : ServiceT<BASLangModel>, ICOMMultiLang
	{
		private ICOMMultiLangBusiness biz = new COMMultiLangBusiness();

		public MActionResult<List<KeyValuePair<string, Hashtable>>> GetIntKeyLangTableList(string[] localeIds = null, string accessToken = null)
		{
			ICOMMultiLangBusiness iCOMMultiLangBusiness = biz;
			return base.RunFunc(iCOMMultiLangBusiness.GetIntKeyLangTableList, localeIds, accessToken);
		}

		public MActionResult<List<KeyValuePair<string, Hashtable>>> GetStringKeyLangTableList(string[] localeIds = null, string accessToken = null)
		{
			ICOMMultiLangBusiness iCOMMultiLangBusiness = biz;
			return base.RunFunc(iCOMMultiLangBusiness.GetStringKeyLangTableList, localeIds, accessToken);
		}

		public MActionResult<string> GetText(LangModule module, string localeId, string key, string accessToken = null)
		{
			ICOMMultiLangBusiness iCOMMultiLangBusiness = biz;
			return base.RunFunc(iCOMMultiLangBusiness.GetText, module, localeId, key, accessToken);
		}

		public MActionResult<string> GetTextByStringKey(string localeId, LangModule module, string key, string defaultValue, string accessToken = null)
		{
			ICOMMultiLangBusiness iCOMMultiLangBusiness = biz;
			return base.RunFunc(iCOMMultiLangBusiness.GetTextByStringKey, localeId, module, key, defaultValue, accessToken);
		}
	}
}
