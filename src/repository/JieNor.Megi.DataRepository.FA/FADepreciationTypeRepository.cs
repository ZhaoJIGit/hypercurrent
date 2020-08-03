using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.FA
{
	public class FADepreciationTypeRepository : DataServiceT<FADepreciationTypeModel>
	{
		public List<CommandInfo> GetInitDepreciationTypeCommands(MContext ctx)
		{
			FADepreciationTypeModel fADepreciationTypeModel = new FADepreciationTypeModel();
			fADepreciationTypeModel.MItemID = "0";
			fADepreciationTypeModel.IsNew = true;
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
			multiLanguageFieldList.MParentID = "0";
			multiLanguageFieldList.MFieldName = "MName";
			List<MultiLanguageField> list2 = new List<MultiLanguageField>();
			MultiLanguageField multiLanguageField = new MultiLanguageField();
			multiLanguageField.MLocaleID = LangCodeEnum.EN_US;
			multiLanguageField.MValue = COMMultiLangRepository.GetText(LangCodeEnum.EN_US, LangModule.FA, "AverageAgeMethod", "平均年限法");
			multiLanguageField.MPKID = "0_1";
			list2.Add(multiLanguageField);
			MultiLanguageField multiLanguageField2 = new MultiLanguageField();
			multiLanguageField2.MLocaleID = LangCodeEnum.ZH_CN;
			multiLanguageField2.MValue = COMMultiLangRepository.GetText(LangCodeEnum.ZH_CN, LangModule.FA, "AverageAgeMethod", "平均年限法");
			multiLanguageField2.MPKID = "0_2";
			list2.Add(multiLanguageField2);
			MultiLanguageField multiLanguageField3 = new MultiLanguageField();
			multiLanguageField3.MLocaleID = LangCodeEnum.ZH_TW;
			multiLanguageField3.MValue = COMMultiLangRepository.GetText(LangCodeEnum.ZH_TW, LangModule.FA, "AverageAgeMethod", "平均年限法");
			multiLanguageField3.MPKID = "0_3";
			list2.Add(multiLanguageField3);
			multiLanguageFieldList.MMultiLanguageField = list2;
			list.Add(multiLanguageFieldList);
			fADepreciationTypeModel.MultiLanguage = list;
			return ModelInfoManager.GetInsertOrUpdateCmd<FADepreciationTypeModel>(ctx, fADepreciationTypeModel, null, true);
		}
	}
}
