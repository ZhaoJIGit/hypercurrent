using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDBankRuleRepository : DataServiceT<BDBankRuleModel>
	{
		public OperationResult UpdateBankRule(MContext ctx, BDBankRuleModel model)
		{
			model.MOrgID = ctx.MOrgID;
			model.MChkAmount = true;
			OperationResult opertion = new OperationResult();
			if (string.IsNullOrWhiteSpace(model.MItemID))
			{
				opertion = IsMBankIdExist(opertion, ctx, model);
				if (!opertion.Success)
				{
					return opertion;
				}
			}
			else
			{
				BDBankRuleModel dataModel = GetDataModel(ctx, model.MItemID, false);
				if (dataModel.MBankID != model.MBankID)
				{
					opertion = IsMBankIdExist(opertion, ctx, model);
					if (!opertion.Success)
					{
						return opertion;
					}
				}
			}
			return base.InsertOrUpdate(ctx, model, null);
		}

		public OperationResult IsMBankIdExist(OperationResult opertion, MContext ctx, BDBankRuleModel model)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MBankID", model.MBankID);
			if (ExistsByFilter(ctx, sqlWhere))
			{
				opertion.Success = false;
				opertion.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "existsbankrule", "目标银行账户已存在银行规则!");
			}
			return opertion;
		}

		public void AddDefaultBankRule(MContext ctx)
		{
			List<BDBankRuleModel> modelList = base.GetModelList(ctx, null, false);
			if (modelList == null || modelList.Count <= 0)
			{
				BDBankRuleModel bDBankRuleModel = new BDBankRuleModel();
				bDBankRuleModel.MultiLanguage = new List<MultiLanguageFieldList>
				{
					new MultiLanguageFieldList
					{
						MFieldName = "MName",
						MMultiLanguageField = new List<MultiLanguageField>
						{
							new MultiLanguageField
							{
								MLocaleID = "0x0009",
								MValue = "Default"
							},
							new MultiLanguageField
							{
								MLocaleID = "0x7804",
								MValue = "默认"
							},
							new MultiLanguageField
							{
								MLocaleID = "0x7C04",
								MValue = "默認"
							}
						}
					}
				};
				bDBankRuleModel.MOrgID = ctx.MOrgID;
				bDBankRuleModel.MBankID = "all";
				bDBankRuleModel.MChkAmount = true;
				bDBankRuleModel.MChkPayee = true;
				base.InsertOrUpdate(ctx, bDBankRuleModel, null);
			}
		}

		public DataGridJson<BDBankRuleListModel> GetBankRuleList(MContext ctx, BDBankRuleListFilterModel filter)
		{
			AddDefaultBankRule(ctx);
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = "SELECT c.MName as MBanKName,a.MItemID, b.MName,MChkAmount,MChkPayee,MChkRef,MChkTransDate \r\n                                   FROM T_BD_BankRule a\r\n                                   LEFT JOIN T_BD_BankRule_L b ON a.MItemID=b.MParentID AND  b.MOrgID=@MOrgID AND b.MLocaleID=@MLocaleID  AND b.MIsDelete=0\r\n                                   LEFT JOIN T_BD_BankAccount_L c ON  a.MBankID=c.MParentID AND c.MOrgID=@MOrgID AND  c.MLocaleID=@MLocaleID AND c.MIsDelete=0\r\n                                   where a.MOrgID=@MOrgID AND  a.MIsDelete=0 \r\n                                 ";
			if (string.IsNullOrEmpty(filter.Sort))
			{
				sqlQuery.OrderBy("a.MCreateDate DESC");
			}
			else if (filter.Sort == "MName")
			{
				sqlQuery.OrderBy("b.MName " + filter.Order);
			}
			else
			{
				sqlQuery.OrderBy($"a.{filter.Sort} {filter.Order}");
			}
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MorgID", ctx.MOrgID)
			};
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<BDBankRuleListModel>(ctx, sqlQuery);
		}

		public static BDBankRuleModel GetBankRuleModel(string bankId, MContext ctx)
		{
			string text = "SELECT * FROM T_BD_BankRule\r\n                            WHERE MOrgID=@MOrgID and MIsDelete = 0  AND (MBankID=@MBankID or MBankID='all')";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBankID", bankId)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(text.ToString(), cmdParms).Tables[0];
			List<BDBankRuleModel> list = ModelInfoManager.DataTableToList<BDBankRuleModel>(dt);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			List<BDBankRuleModel> list2 = (from t in list
			where t.MBankID != "all"
			select t).ToList();
			if (list2 != null && list2.Count > 0)
			{
				return list2[0];
			}
			return list[0];
		}
	}
}
