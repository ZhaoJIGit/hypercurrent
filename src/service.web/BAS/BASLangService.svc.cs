using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASLangService : ServiceT<BASLangModel>, IBASLang
	{
		private readonly IBASLangBusiness biz = new BASLangBusiness();

		public MActionResult<List<BASLangModel>> GetOrgLangList(string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunFunc(iBASLangBusiness.GetOrgLangList, accessToken);
		}

		public MActionResult<List<BASLangModel>> GetSysLangList(string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunFunc(iBASLangBusiness.GetSysLangList, accessToken);
		}

		public MActionResult<List<string>> GetOrgLangIDList(string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunFunc(iBASLangBusiness.GetOrgLangIDList, accessToken);
		}

		public MActionResult<string> GetClientGlobalInfo(string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunFunc(iBASLangBusiness.GetClientGlobalInfo, accessToken);
		}

		public MActionResult<Dictionary<LangModule, Dictionary<string, string>>> GetStringKeyLangList(string langId, string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunFunc(iBASLangBusiness.GetStringKeyLangList, langId, accessToken);
		}

		public MActionResult<Dictionary<LangModule, Dictionary<int, string>>> GetIntKeyLangList(string langId, string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunFunc(iBASLangBusiness.GetIntKeyLangList, langId, accessToken);
		}

		public MActionResult<string> UpdateLang(LangModule module, string key, string defaultValue, int type, string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunAction<string, LangModule, string, string, int>(iBASLangBusiness.UpdateLang, module, key, defaultValue, type, accessToken);
		}

		public MActionResult<string> UpdateScriptLang(LangModule langModule, string key, string defaultValue, string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunAction<string, LangModule, string, string>(iBASLangBusiness.UpdateScriptLang, langModule, key, defaultValue, accessToken);
		}

		public MActionResult<object> GetText(LangModule module, string key, string accessToken = null)
		{
			IBASLangBusiness iBASLangBusiness = biz;
			return base.RunFunc((Func<MContext, LangModule, string, object>)iBASLangBusiness.GetText, module, key, accessToken);
		}
	}
}
