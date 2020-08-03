using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.RPT.GL;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.RPT.GL
{
	public class RPTFilterSchemeBussiness : IRPTFilterScheme
	{
		private RPTFilterSchemeRepository Dal = new RPTFilterSchemeRepository();

		public List<RPTFilterSchemeModel> GetFilterSchemeList(MContext ctx, RPTFilterSchemeFilterModel filter)
		{
			return Dal.GetFilterSchemeList(ctx, filter);
		}

		public OperationResult InsertOrUpateFilterScheme(MContext ctx, RPTFilterSchemeModel filterSchemeModel)
		{
			OperationResult operationResult = new OperationResult();
			if (filterSchemeModel == null)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "FilterSchemeIsNull", "方案不能为空");
				return operationResult;
			}
			if (string.IsNullOrWhiteSpace(filterSchemeModel.MName))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "FilterSchemeNameIsNull", "方案名称不能为空");
				return operationResult;
			}
			if (string.IsNullOrWhiteSpace(filterSchemeModel.MContent))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "FilterSchemeContentIsNull", "方案内容不能为空");
				return operationResult;
			}
			if (CheckFilterNameIsExist(ctx, filterSchemeModel))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "FilterSchemeNameExisted", "方案名称已经存在！");
				return operationResult;
			}
			return Dal.InsertOrUpdate(ctx, filterSchemeModel);
		}

		private bool CheckFilterNameIsExist(MContext ctx, RPTFilterSchemeModel filterSchemeModel)
		{
			//bool flag = true;
			SqlWhere sqlWhere = new SqlWhere();
			List<MultiLanguageField> multiNameList = new List<MultiLanguageField>();
			if (filterSchemeModel.MultiLanguage != null)
			{
				filterSchemeModel.MultiLanguage.ForEach(delegate(MultiLanguageFieldList x)
				{
					if (x.MMultiLanguageField != null)
					{
						multiNameList.AddRange(x.MMultiLanguageField);
					}
				});
			}
			List<string> nameList = (from x in multiNameList
			where !string.IsNullOrWhiteSpace(x.MValue)
			select x.MValue).ToList();
			return Dal.IsExistFilterSchemeName(ctx, filterSchemeModel.MItemID, nameList);
		}

		public RPTFilterSchemeModel GetFilterScheme(MContext ctx, string id)
		{
			return Dal.GetDataModel(ctx, id, false);
		}

		public OperationResult DeleteFilterScheme(MContext ctx, string id)
		{
			return Dal.DeleteFilterScheme(ctx, id);
		}
	}
}
