using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASLangBusiness : IBASLangBusiness
	{
		private readonly BASNationalLanguageRepository dal = new BASNationalLanguageRepository();

		public List<BASLangModel> GetOrgLangList(MContext context)
		{
			return BASLangRepository.GetOrgLangList(context);
		}

		[NoAuthorization]
		public List<BASLangModel> GetSysLangList(MContext context)
		{
			return BASLangRepository.GetSysLangList(context);
		}

		public List<string> GetOrgLangIDList(MContext context)
		{
			List<BASLangModel> orgLangList = GetOrgLangList(context);
			return (from t in orgLangList
			select t.LangID).ToList();
		}

		public string GetClientGlobalInfo(MContext ctx)
		{
			REGGlobalizationRepository rEGGlobalizationRepository = new REGGlobalizationRepository();
			return rEGGlobalizationRepository.GetClientGlobalInfo(ctx);
		}

		[NoAuthorization]
		public Dictionary<LangModule, Dictionary<int, string>> GetIntKeyLangList(MContext ctx, string langId)
		{
			return BASLangRepository.GetIntKeyLangList(ctx, langId);
		}

		[NoAuthorization]
		public Dictionary<LangModule, Dictionary<string, string>> GetStringKeyLangList(MContext ctx, string langId)
		{
			return BASLangRepository.GetStringKeyLangList(ctx, langId);
		}

		[NoAuthorization]
		public object GetText(MContext ctx, LangModule module, string key)
		{
			return BASLangRepository.GetText(ctx, module, key);
		}

		[NoAuthorization]
		public void UpdateLang(MContext ctx, LangModule module, string key, string defaultValue, int type)
		{
			BASLangRepository.UpdateLang(ctx, module, key, defaultValue, type);
		}

		[NoAuthorization]
		public void UpdateScriptLang(MContext ctx, LangModule langModule, string key, string defaultValue)
		{
			BASLangRepository.UpdateLang(ctx, langModule, key, defaultValue, 2);
		}

		[NoAuthorization]
		public string GetText(MContext ctx, LangModule langModule, string key, string defaultValue)
		{
			return COMMultiLangRepository.GetText(ctx.MLCID, langModule, key, defaultValue);
		}
	}
}
