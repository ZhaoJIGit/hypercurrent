using JieNor.Megi.BusinessContract.REG;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;

namespace JieNor.Megi.BusinessService.REG
{
	public class REGGlobalizationBusiness : IREGGlobalizationBusiness
	{
		private REGGlobalizationRepository global = new REGGlobalizationRepository();

		public OperationResult GlobalizationUpdate(MContext ctx, REGGlobalizationModel model)
		{
			OperationResult operationResult = new OperationResult();
			bool isSys = ctx.IsSys;
			ctx.IsSys = true;
			BASOrganisationModel dataEditModel = ModelInfoManager.GetDataEditModel<BASOrganisationModel>(ctx, ctx.MOrgID, false, true);
			if (dataEditModel.MOriRegProgress < 5)
			{
				REGGlobalizationRepository.ClearCache(ctx);
				BASLangRepository.ClearCache(ctx);
				operationResult = global.GlobalizationUpdate(ctx, model);
				if (operationResult.Success)
				{
					SetGlobalizationContext(ctx, model);
				}
			}
			ctx.IsSys = isSys;
			ContextHelper.MContext = ctx;
			return operationResult;
		}

		public REGGlobalizationModel GetOrgGlobalizationDetail(MContext ctx, string orgid)
		{
			return global.GetOrgGlobalizationDetail(ctx, orgid);
		}

		public void SetGlobalizationContext(MContext mContext, REGGlobalizationModel globalModel)
		{
			mContext.MZoneFormat = ((globalModel == null) ? "China Standard Time" : globalModel.MSystemZone);
			mContext.MDateFormat = ((globalModel == null) ? "yyyy-MM-dd" : globalModel.MSystemDate);
			mContext.MTimeFormat = ((globalModel == null) ? "HH:mm:ss" : globalModel.MSystemTime);
			mContext.MDigitGrpFormat = ((globalModel == null) ? "," : globalModel.MSystemDigitGroupingSymbol);
			mContext.MIsShowCSymbol = (globalModel?.MIsShowCSymbol ?? false);
			if (globalModel != null && !string.IsNullOrWhiteSpace(globalModel.MSystemLanguage) && globalModel.MSystemLanguage.IndexOf(mContext.MLCID) <= 0)
			{
				try
				{
					mContext.MLCID = globalModel.MSystemLanguage.Split(',')[0];
				}
				catch
				{
					mContext.MLCID = LangCodeEnum.EN_US;
				}
			}
			ContextHelper.MContext = mContext;
		}
	}
}
