using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.REG
{
	public class REGTaxRateRepository : DataServiceT<REGTaxRateModel>
	{
		private string multLangFieldSql = "\r\n            ,t2.MName{0} ";

		public string CommonSelect = "select \r\n                t1.MItemID,\r\n                t1.MItemID as MTaxRateID,\r\n                t1.MEffectiveTaxRate,\r\n                t3.MNumber as MSaleTaxAccountCode,\r\n                t4.MNumber as MPurchaseAccountCode,\r\n                t5.MNumber as MPayDebitAccountCode,\r\n                t1.MIsSysData,\r\n                t1.MIsActive,\r\n                t1.MModifyDate,\r\n                t2.MName\r\n                #_#lang_field0#_# \r\n            from\r\n                T_REG_TaxRate t1\r\n                    left join\r\n                @_@t_reg_taxrate_l@_@ t2 ON t2.MParentID = t1.MItemID\r\n                    and t2.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_account t3 ON t3.MOrgID = t1.MOrgID\r\n                    AND t3.MCode = t1.MSaleTaxAccountCode\r\n                    AND t3.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_account t4 ON t4.MOrgID = t1.MOrgID\r\n                    AND t4.MCode = t1.MPurchaseAccountCode\r\n                    AND t4.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_account t5 ON t5.MOrgID = t1.MOrgID\r\n                    AND t5.MCode = t1.MPayDebitAccountCode\r\n                    AND t5.MIsDelete = 0\r\n            where\r\n                t1.MOrgID = @MOrgID\r\n                    and t1.MIsDelete = 0 ";

		public DataGridJson<REGTaxRateModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<REGTaxRateModel>(ctx, param, CommonSelect, multLangFieldSql, false, true, null);
		}

		public List<CommandInfo> GetUpdateCmdList(MContext ctx, REGTaxRateModel model, List<string> fields = null)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			model.ValidationErrors = (model.ValidationErrors ?? new List<ValidationError>());
			if (fields != null && !fields.Contains("MEffectiveTaxRate"))
			{
				return ModelInfoManager.GetInsertOrUpdateCmd<REGTaxRateModel>(ctx, model, fields, true);
			}
			if (model.TaxRateDetail == null || model.TaxRateDetail.Count == 0)
			{
				model.ValidationErrors.Add(new ValidationError
				{
					Message = string.Empty
				});
				return result;
			}
			decimal effTaxRate = GetEffTaxRate(model);
			model.MAppID = ctx.MAppID;
			model.MTaxRate = model.TaxRateDetail.Sum((REGTaxRateEntryModel t) => t.MTaxRate);
			model.MEffectiveTaxRate = effTaxRate + model.MTaxRate;
			foreach (REGTaxRateEntryModel item in model.TaxRateDetail)
			{
				item.MOrgID = model.MOrgID;
			}
			return ModelInfoManager.GetInsertOrUpdateCmd<REGTaxRateModel>(ctx, model, fields, true);
		}

		public override OperationResult InsertOrUpdate(MContext ctx, REGTaxRateModel modelData, string fields = null)
		{
			if (modelData.TaxRateDetail == null || modelData.TaxRateDetail.Count == 0)
			{
				return new OperationResult
				{
					Success = false
				};
			}
			if (ModelInfoManager.IsLangColumnValueExists<REGTaxRateModel>(ctx, "MName", modelData.MultiLanguage, modelData.MItemID, "", "", false))
			{
				return new OperationResult
				{
					Success = false,
					HaveError = true,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxRateNameUsed", "税率名称已存在，请录入其它名称。")
				};
			}
			decimal effTaxRate = GetEffTaxRate(modelData);
			modelData.MAppID = ctx.MAppID;
			modelData.MTaxRate = modelData.TaxRateDetail.Sum((REGTaxRateEntryModel t) => t.MTaxRate);
			modelData.MEffectiveTaxRate = effTaxRate + modelData.MTaxRate;
			foreach (REGTaxRateEntryModel item in modelData.TaxRateDetail)
			{
				item.MOrgID = modelData.MOrgID;
			}
			return base.InsertOrUpdate(ctx, modelData, fields);
		}

		private decimal GetEffTaxRate(REGTaxRateModel paramModel)
		{
			decimal d = decimal.One;
			if (paramModel.TaxRateDetail.Count((REGTaxRateEntryModel t) => t.MIsCompound) > 0)
			{
				foreach (REGTaxRateEntryModel item in paramModel.TaxRateDetail)
				{
					d *= item.MTaxRate;
				}
				d /= 100m;
			}
			else
			{
				d = default(decimal);
			}
			return d;
		}

		public bool HaveTaxRateName(MContext ctx, REGTaxRateModel modelData)
		{
			List<MultiLanguageFieldList> multiLanguage = modelData.MultiLanguage;
			if (multiLanguage != null && multiLanguage.Count > 0)
			{
				List<string> list = new List<string>();
				List<string> list2 = (from m in multiLanguage[0].MMultiLanguageField
				select m.MValue).ToList();
				List<string> list3 = list;
				list2.ForEach(list3.Add);
				string text = string.Format(" SELECT MPKID FROM t_reg_taxrate_l WHERE MIsActive=1 AND MIsDelete=0 AND MOrgID = @MOrgID AND (  ");
				if (list.Count == 0)
				{
					return false;
				}
				int num = (modelData.MItemID == null) ? (list.Count + 1) : (list.Count + 2);
				MySqlParameter[] array = new MySqlParameter[num];
				array[0] = new MySqlParameter("@MOrgID", ctx.MOrgID);
				for (int i = 0; i < list.Count; i++)
				{
					if (i == 0)
					{
						text += " MName=@MName ";
						array[i + 1] = new MySqlParameter("@MName", list[i]);
					}
					else
					{
						text = text + "  OR  MName=@MName" + i + " ";
						array[i + 1] = new MySqlParameter(("@MName" + i) ?? "", list[i]);
					}
				}
				text += " ) ";
				if (modelData.MItemID != null)
				{
					text += " AND MParentID !=@MItemID ";
					array[num - 1] = new MySqlParameter("@MItemID", modelData.MItemID);
				}
				List<REGTaxRateModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<REGTaxRateModel>(ctx, text, array);
				if (dataModelBySql != null && dataModelBySql.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		public List<REGTaxRateModel> GetListIgnoreLocale(MContext ctx, bool includeDisabled)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select TRIM(b.MName) as MName, a.MItemID, a.MTaxRate, a.MEffectiveTaxRate, a.MIsActive from T_REG_TaxRate a ");
			stringBuilder.AppendLine(" join T_REG_TaxRate_l b on a.MItemID=b.MParentID");
			stringBuilder.AppendLine(" where a.MOrgID=@MOrgID and a.MIsDelete=0 ");
			if (!includeDisabled)
			{
				stringBuilder.AppendLine(" and a.MIsActive=1 ");
			}
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			return ModelInfoManager.GetDataModelBySql<REGTaxRateModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		public List<REGTaxRateModel> GetList(MContext ctx, SqlWhere filter = null, bool ignoreLocale = false)
		{
			if (ignoreLocale)
			{
				return GetListIgnoreLocale(ctx, false);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT a.MItemID,b.MName,a.MorgID,a.MAppID,a.MTaxRate,a.MEffectiveTaxRate,a.MPurchaseAccountCode,a.MPayDebitAccountCode,a.MSaleTaxAccountCode,\r\n                                a.MIsActive,a.MIsDelete,a.MCreatorID,a.MCreateDate,a.MModifierID,a.MModifyDate,a.MIsSysData \r\n                            FROM T_REG_TaxRate a \r\n                            INNER JOIN T_REG_TaxRate_L b ON a.MItemID= b.MParentID AND b.MLocaleID=@MLocaleID and a.MIsActive=1 and a.MIsDelete = 0 AND a.MOrgID=@MOrgID ");
			if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
			}
			stringBuilder.AppendLine(" GROUP BY a.MItemID ");
			if (filter != null && string.IsNullOrEmpty(filter.OrderBySqlString))
			{
				stringBuilder.AppendLine(filter.OrderBySqlString);
			}
			else
			{
				stringBuilder.AppendLine(" ORDER BY MName ");
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 6));
			arrayList.Add(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36));
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] array = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			return ModelInfoManager.GetDataModelBySql<REGTaxRateModel>(ctx, stringBuilder.ToString(), array);
		}

		public DataGridJson<REGTaxRateModel> GetTaxRateListByPage(MContext ctx, REGTaxTateListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT a.MItemID,b.MName,a.MorgID,a.MAppID,a.MTaxRate,a.MEffectiveTaxRate,\r\n                                a.MIsActive,a.MIsDelete,a.MCreatorID,a.MCreateDate,a.MModifierID,a.MModifyDate,a.MIsSysData \r\n                            FROM T_REG_TaxRate a \r\n                            LEFT JOIN T_REG_TaxRate_L b ON a.MItemID= b.MParentID \r\n                            WHERE a.MIsActive=@MIsActive and a.MIsDelete = 0 AND a.MOrgID=@MOrgID AND b.MLocaleID=@MLocaleID \r\n                            GROUP BY a.MItemID ");
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@searchFilter", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MIsActive", MySqlDbType.Bit)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = filter.SearchFilter;
			array[3].Value = filter.MIsActive;
			sqlQuery.SqlWhere = filter;
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				sqlQuery.SqlWhere.OrderBy($" a.MIsSysData desc, b.{filter.Sort} {filter.Order}");
			}
			sqlQuery.SelectString = stringBuilder.ToString();
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<REGTaxRateModel>(ctx, sqlQuery);
		}

		public OperationResult ArchiveTaxRate(MContext ctx, List<string> keyIDs, bool isActive)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MIsActive", MySqlDbType.Bit)
			};
			list2[0].Value = ctx.MOrgID;
			list2[1].Value = isActive;
			string inFilterQuery = GLUtility.GetInFilterQuery(keyIDs, ref list2, "M_ID");
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_reg_taxrate_l set MIsActive=@MIsActive where MOrgID=@MOrgID and MParentID " + inFilterQuery;
			DbParameter[] array = commandInfo.Parameters = list2.ToArray();
			list.Add(commandInfo);
			List<CommandInfo> archiveFlagCmd = ModelInfoManager.GetArchiveFlagCmd<REGTaxRateModel>(ctx, keyIDs, isActive);
			list.AddRange(archiveFlagCmd);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return new OperationResult
			{
				Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0)
			};
		}

		public void DeleteUnUsedTaxRate(MContext ctx)
		{
			List<REGTaxRateModel> dataModelList = ModelInfoManager.GetDataModelList<REGTaxRateModel>(ctx, new SqlWhere(), false, false);
			List<string> itemIds = (from f in dataModelList
			select f.MItemID).ToList();
			List<string> list = new List<string>();
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "TaxRate", itemIds, out list);
			if (list.Any())
			{
				ModelInfoManager.DeleteFlag<REGTaxRateModel>(ctx, list);
			}
		}

		public List<CommandInfo> UpdateTaxrateMapAccount(MContext ctx, string oldCode, string newCode)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@oldCode", MySqlDbType.VarChar, 36)
				{
					Value = oldCode
				},
				new MySqlParameter("@newCode", MySqlDbType.VarChar, 36)
				{
					Value = newCode
				}
			};
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_reg_taxrate set MSaleTaxAccountCode=@newCode where MSaleTaxAccountCode=@oldCode and MOrgID=@MOrgID";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = "update t_reg_taxrate set MPurchaseAccountCode=@newCode where MPurchaseAccountCode=@oldCode and MOrgID=@MOrgID";
			array = (commandInfo2.Parameters = parameters);
			list.Add(commandInfo2);
			CommandInfo commandInfo3 = new CommandInfo();
			commandInfo3.CommandText = "update t_reg_taxrate set MPayDebitAccountCode=@newCode where MPayDebitAccountCode=@oldCode and MOrgID=@MOrgID";
			array = (commandInfo3.Parameters = parameters);
			list.Add(commandInfo3);
			return list;
		}
	}
}
