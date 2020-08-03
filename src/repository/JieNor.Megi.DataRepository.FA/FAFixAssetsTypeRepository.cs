using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.FA
{
	public class FAFixAssetsTypeRepository : DataServiceT<FAFixAssetsTypeModel>
	{
		public GLUtility utility = new GLUtility();

		public BDAccountRepository accountRepository = new BDAccountRepository();

		public List<FAFixAssetsTypeModel> GetFixAssetsTypeList(MContext ctx, string itemID)
		{
			string sql = "SELECT \n                                t1.*,\n                                t2.MName,\n                                t4.MFullName AS MFixAccountFullName,\n                                t6.MFullName AS MDepAccountFullName\n                            FROM\n                                t_fa_fixassetstype t1\n                                    INNER JOIN\n                                t_fa_fixassetstype_l t2 ON t1.MItemID = t2.MParentID\n                                    AND t2.MIsDelete = t1.MIsDelete\n                                    AND t2.MOrgID = t1.MOrgID\n                                    AND t2.MLocaleID = @MLocaleID\n                                    LEFT JOIN\n                                t_bd_account t3 ON t3.MCode = t1.MFixAccountCode\n                                    AND t3.MOrgID = t1.MOrgID\n                                    AND t3.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_account_l t4 ON t3.MItemID = t4.MParentID\n                                    AND t4.MOrgID = t3.MOrgID\n                                    AND t4.MIsDelete = t3.MIsDelete\n                                    AND t4.MLocaleID = t2.MLocaleID\n                                    LEFT JOIN\n                                t_bd_account t5 ON t5.MCode = t1.MDepAccountCode\n                                    AND t5.MOrgID = t1.MOrgID\n                                    AND t5.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_account_l t6 ON t5.MItemID = t6.MParentID\n                                    AND t6.MOrgID = t5.MOrgID\n                                    AND t6.MIsDelete = t5.MIsDelete\n                                    AND t6.MLocaleID = t2.MLocaleID\n                            WHERE\n                                t1.MOrgID = @MOrgID\n                                    AND t1.MIsDelete = 0\n                                order by t1.MNumber asc ;\n                            ";
			return ModelInfoManager.GetDataModelBySql<FAFixAssetsTypeModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
		}

		public OperationResult DeleteFixAssetsType(MContext ctx, List<string> itemIDs)
		{
			ValidateFixAssetsTypeRelated(ctx, itemIDs);
			return ModelInfoManager.DeleteFlag<FAFixAssetsTypeModel>(ctx, itemIDs);
		}

		public OperationResult SaveFixAssetsType(MContext ctx, FAFixAssetsTypeModel model)
		{
			ValidateFixAssetsType(ctx, model);
			return ModelInfoManager.InsertOrUpdate<FAFixAssetsTypeModel>(ctx, model, null);
		}

		public List<CommandInfo> GetInitAssetsTypeCommands(MContext ctx)
		{
			string code = "1601";
			string code2 = "1602";
			List<BDAccountListModel> bDAccountList = accountRepository.GetBDAccountList(ctx, "");
			if (bDAccountList.Count > 0)
			{
				BDAccountListModel leafAccountByCode = accountRepository.GetLeafAccountByCode(bDAccountList, code);
				BDAccountListModel leafAccountByCode2 = accountRepository.GetLeafAccountByCode(bDAccountList, code2);
				code = ((leafAccountByCode == null) ? "1601" : leafAccountByCode.MCode);
				code2 = ((leafAccountByCode2 == null) ? "1602" : leafAccountByCode2.MCode);
			}
			else
			{
				code = "1601";
				code2 = "1602";
			}
			List<FAFixAssetsTypeModel> list = new List<FAFixAssetsTypeModel>();
			FAFixAssetsTypeModel item = new FAFixAssetsTypeModel
			{
				MNumber = "001",
				MDepreciationFromCurrentPeriod = false,
				MIsSys = true,
				MUsefulPeriods = 240,
				MRateOfSalvage = 5m,
				MDepreciationTypeID = "0",
				MFixAccountCode = code,
				MDepAccountCode = code2,
				MultiLanguage = SetMuliLanguage(ctx, "FixAssetsType001", "房屋、建筑物")
			};
			list.Add(item);
			FAFixAssetsTypeModel item2 = new FAFixAssetsTypeModel
			{
				MNumber = "002",
				MDepreciationFromCurrentPeriod = false,
				MIsSys = true,
				MUsefulPeriods = 120,
				MRateOfSalvage = 5m,
				MDepreciationTypeID = "0",
				MFixAccountCode = code,
				MDepAccountCode = code2,
				MultiLanguage = SetMuliLanguage(ctx, "FixAssetsType002", "飞机、火车、轮船、机器、机械和其他生产设备")
			};
			list.Add(item2);
			FAFixAssetsTypeModel item3 = new FAFixAssetsTypeModel
			{
				MNumber = "003",
				MDepreciationFromCurrentPeriod = false,
				MIsSys = true,
				MUsefulPeriods = 60,
				MRateOfSalvage = 5m,
				MDepreciationTypeID = "0",
				MFixAccountCode = code,
				MDepAccountCode = code2,
				MultiLanguage = SetMuliLanguage(ctx, "FixAssetsType003", "与生产经营活动有关的器具、工具、家具等")
			};
			list.Add(item3);
			FAFixAssetsTypeModel item4 = new FAFixAssetsTypeModel
			{
				MNumber = "004",
				MDepreciationFromCurrentPeriod = false,
				MIsSys = true,
				MUsefulPeriods = 48,
				MRateOfSalvage = 5m,
				MDepreciationTypeID = "0",
				MFixAccountCode = code,
				MDepAccountCode = code2,
				MultiLanguage = SetMuliLanguage(ctx, "FixAssetsType004", "飞机、火车、轮船以外的运输工具")
			};
			list.Add(item4);
			FAFixAssetsTypeModel item5 = new FAFixAssetsTypeModel
			{
				MNumber = "005",
				MDepreciationFromCurrentPeriod = false,
				MIsSys = true,
				MUsefulPeriods = 36,
				MRateOfSalvage = 5m,
				MDepreciationTypeID = "0",
				MFixAccountCode = code,
				MDepAccountCode = code2,
				MultiLanguage = SetMuliLanguage(ctx, "FixAssetsType005", "电子设备")
			};
			list.Add(item5);
			FAFixAssetsTypeModel item6 = new FAFixAssetsTypeModel
			{
				MNumber = "006",
				MDepreciationFromCurrentPeriod = false,
				MIsSys = true,
				MUsefulPeriods = 60,
				MRateOfSalvage = 5m,
				MDepreciationTypeID = "0",
				MFixAccountCode = code,
				MDepAccountCode = code2,
				MultiLanguage = SetMuliLanguage(ctx, "FixAssetsType006", "其他")
			};
			list.Add(item6);
			return ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, null, true);
		}

		private List<MultiLanguageFieldList> SetMuliLanguage(MContext ctx, string languageKey, string defaultVaue)
		{
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
			multiLanguageFieldList.MFieldName = "MName";
			List<MultiLanguageField> list2 = new List<MultiLanguageField>();
			List<string> mActiveLocaleIDS = ctx.MActiveLocaleIDS;
			foreach (string item in mActiveLocaleIDS)
			{
				MultiLanguageField multiLanguageField = new MultiLanguageField();
				multiLanguageField.MLocaleID = item;
				multiLanguageField.MValue = COMMultiLangRepository.GetText(item, LangModule.FA, languageKey, defaultVaue);
				list2.Add(multiLanguageField);
			}
			multiLanguageFieldList.MMultiLanguageField = list2;
			list.Add(multiLanguageFieldList);
			return list;
		}

		private void ValidateFixAssetsTypeRelated(MContext ctx, List<string> itemIDs)
		{
			ValidateQueryModel validateFixAssetsTypeRelatedSql = utility.GetValidateFixAssetsTypeRelatedSql(ctx, itemIDs);
			utility.QueryValidateSql(ctx, true, validateFixAssetsTypeRelatedSql);
		}

		private void ValidateFixAssetsType(MContext ctx, FAFixAssetsTypeModel model)
		{
			ValidateQueryModel validateFixAssetsTypeNameSql = utility.GetValidateFixAssetsTypeNameSql(ctx, model);
			ValidateQueryModel validateFixAssetsTypeNumberSql = utility.GetValidateFixAssetsTypeNumberSql(ctx, model);
			List<ValidateQueryModel> second = (model.MCheckGroupValueModel != null) ? utility.GetValidateCheckGroupValueModel(new List<GLCheckGroupValueModel>
			{
				model.MCheckGroupValueModel
			}) : new List<ValidateQueryModel>();
			utility.QueryValidateSql(ctx, true, new List<ValidateQueryModel>
			{
				validateFixAssetsTypeNameSql,
				validateFixAssetsTypeNumberSql
			}.Concat(second).ToArray());
		}
	}
}
