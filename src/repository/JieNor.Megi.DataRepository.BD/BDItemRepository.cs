using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDItemRepository : DataServiceT<BDItemModel>
	{
		private string multLangFieldSql = "\r\n            ,t2.MDesc{0}\r\n            ,t4.MName{0} as MSalesDetails_MTaxRate_MName{0}\r\n            ,t6.MName{0} as MPurchaseDetails_MTaxRate_MName{0} ";

		public string CommonSelect = "SELECT \r\n                t1.MItemID,\r\n                t1.MItemID as MProductItemID,\r\n                t1.MNumber as MProductNumber,\r\n                t1.MIsActive,\r\n                t1.MModifyDate,\r\n                t1.MIsExpenseItem,\r\n                t7.MNumber as MIncomeAccountCode,\r\n                (case t1.MIsExpenseItem when 1 then '' else t8.MNumber end) as MCostAccountCode,\r\n                t9.MNumber as MInventoryAccountCode,\r\n                (case t1.MIsExpenseItem when 1 then t8.MNumber else '' end) as MExpenseAccountCode,\r\n                t1.MPurPrice as MPurchaseDetails_MUnitPrice,\r\n                t1.MSalPrice as MSalesDetails_MUnitPrice,\r\n                t1.MSalTaxTypeID as MSalesDetails_MTaxRate_MTaxRateID,\r\n                t1.MPurTaxTypeID as MPurchaseDetails_MTaxRate_MTaxRateID,\r\n                t3.MEffectiveTaxRate as MSalesDetails_MTaxRate_MEffectiveTaxRate,\r\n                t4.MName as MSalesDetails_MTaxRate_MName,\r\n                t5.MEffectiveTaxRate as MPurchaseDetails_MTaxRate_MEffectiveTaxRate,\r\n                t6.MName as MPurchaseDetails_MTaxRate_MName,\r\n                t2.MDesc\r\n                #_#lang_field0#_#\r\n            FROM\r\n                T_BD_Item t1\r\n                    INNER JOIN\r\n                @_@t_bd_item_l@_@ t2 ON t1.MOrgID = t2.MOrgID\r\n                    AND t1.MItemID = t2.MParentID\r\n                    AND t2.MIsDelete = 0                    \r\n                    LEFT JOIN\r\n                t_reg_taxrate t3 ON t3.MOrgID = t1.MOrgID\r\n                    AND t3.MItemID = t1.MSalTaxTypeID\r\n                    AND t3.MIsDelete = 0\r\n                    LEFT JOIN\r\n                @_@t_reg_taxrate_l@_@ t4 ON t4.MOrgID = t1.MOrgID\r\n                    AND t4.MParentID = t3.MItemID\r\n                    AND t4.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_reg_taxrate t5 ON t5.MOrgID = t1.MOrgID\r\n                    AND t5.MItemID = t1.MPurTaxTypeID\r\n                    AND t5.MIsDelete = 0\r\n                    LEFT JOIN\r\n                @_@t_reg_taxrate_l@_@ t6 ON t6.MOrgID = t1.MOrgID\r\n                    AND t6.MParentID = t5.MItemID\r\n                    AND t6.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_account t7 ON t7.MOrgID = t1.MOrgID\r\n                    AND t7.MCode = t1.MIncomeAccountCode\r\n                    AND t7.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_account t8 ON t8.MOrgID = t1.MOrgID\r\n                    AND t8.MCode = t1.MCostAccountCode\r\n                    AND t8.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_account t9 ON t9.MOrgID = t1.MOrgID\r\n                    AND t9.MCode = t1.MInventoryAccountCode\r\n                    AND t9.MIsDelete = 0\r\n            WHERE\r\n                t1.MOrgID = @MOrgID\r\n                    AND t1.MIsDelete = 0";

		public DataGridJson<BDItemModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<BDItemModel>(ctx, param, CommonSelect, multLangFieldSql, false, true, null);
		}

		public List<BDItemModel> GetList(MContext ctx, BDItemModel item)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" SELECT * FROM T_BD_Item WHERE MOrgID=@MOrgID AND MIsActive=1 AND MIsDelete=0 ");
			stringBuilder.Append(" AND MNumber=@MNumber ");
			if (!item.IsNew)
			{
				stringBuilder.Append(" AND MItemID!=@MItemID ");
			}
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MNumber", item.MNumber),
				new MySqlParameter("@MItemID", item.MItemID)
			};
			return ModelInfoManager.GetDataModelBySql<BDItemModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		public List<BDItemModel> GetListByIds(MContext ctx, List<string> idsList)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter(new SqlFilter("MItemID", SqlOperators.In, idsList.ToArray()));
			return ModelInfoManager.GetDataModelList<BDItemModel>(ctx, sqlWhere, false, false);
		}

		public List<BDItemModel> GetListByWhere(MContext ctx, string strWhere)
		{
			StringBuilder listSqlBuilder = GetListSqlBuilder(strWhere);
			listSqlBuilder.AppendLine(" and T1.MIsActive=1 ");
			MySqlParameter[] listParameters = GetListParameters(ctx, strWhere);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(listSqlBuilder.ToString(), listParameters);
			return ModelInfoManager.DataTableToList<BDItemModel>(ds, 0, true);
		}

		public List<BDItemModel> GetItemList(MContext ctx, BDItemListFilterModel filter)
		{
			StringBuilder listSqlBuilder = GetListSqlBuilder(filter.Keyword);
			if (!filter.IncludeDisable)
			{
				listSqlBuilder.AppendLine(" and T1.MIsActive=1");
			}
			MySqlParameter[] listParameters = GetListParameters(ctx, filter.Keyword);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(listSqlBuilder.ToString(), listParameters);
			return ModelInfoManager.DataTableToList<BDItemModel>(ds, 0, true);
		}

		public DataGridJson<BDItemModel> GetPageList(MContext ctx, BDItemListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder listSqlBuilder = GetListSqlBuilder(filter.Keyword);
			if (!filter.IncludeDisable)
			{
				if (filter.IsActive)
				{
					listSqlBuilder.AppendLine(" and T1.MIsActive=1 ");
				}
				else
				{
					listSqlBuilder.AppendLine(" and T1.MIsActive=0 ");
				}
			}
			MySqlParameter[] listParameters = GetListParameters(ctx, filter.Keyword);
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				filter.OrderBy($"{filter.Sort} {filter.Order} ");
			}
			else
			{
				filter.AddOrderBy("MNumber", SqlOrderDir.Asc);
			}
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = listSqlBuilder.ToString();
			MySqlParameter[] array = listParameters;
			foreach (MySqlParameter para in array)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<BDItemModel>(ctx, sqlQuery);
		}

		private StringBuilder GetListSqlBuilder(string keyword)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT T1.MItemID,T1.MNumber,T1.MPurPrice,T1.MSalPrice,T2.MDesc, ");
			stringBuilder.AppendLine("T1.MPurTaxTypeID,T1.MSalTaxTypeID,T1.MInventoryAccountCode,T1.MIncomeAccountCode,T1.MCostAccountCode ,T1.MIsActive ");
			stringBuilder.AppendLine(" FROM T_BD_Item T1");
			stringBuilder.AppendLine(" Inner JOIN T_BD_Item_L T2 ON T1.MOrgID = T2.MOrgID and  T1.MItemID=T2.MParentID And T2.MLocaleID=@MLCID ");
			stringBuilder.AppendLine(" WHERE T1.MIsDelete = 0 And T1.MOrgID=@MOrgID ");
			if (!string.IsNullOrEmpty(keyword))
			{
				stringBuilder.AppendLine(" AND (T1.MNumber LIKE concat('%',@MFilter,'%') or T2.MDesc LIKE concat('%',@MFilter,'%')) ");
			}
			return stringBuilder;
		}

		private MySqlParameter[] GetListParameters(MContext ctx, string filterStr)
		{
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MFilter", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = (string.IsNullOrWhiteSpace(filterStr) ? filterStr : filterStr.Replace("\\", "\\\\"));
			return array;
		}

		public List<ItemRowModel> GetReportList(MContext ctx)
		{
			string sql = "SELECT T1.MItemID,T1.MNumber,T1.MPurPrice,T1.MSalPrice,T2.MDesc, \r\n                            T1.MPurTaxTypeID,T1.MSalTaxTypeID,T1.MInventoryAccountCode,T1.MIncomeAccountCode,T1.MCostAccountCode,T1.MIsExpenseItem \r\n                            FROM T_BD_Item T1\r\n                            LEFT JOIN T_BD_Item_L T2 ON T1.MItemID=T2.MParentID And T2.MLocaleID=@MLCID and T1.MOrgID = T2.MOrgID and T2.MIsDelete = 0 \r\n                            WHERE T1.MIsDelete = 0 And T1.MOrgID=@MOrgID  and T1.MIsActive = 1\r\n                            order by MNumber ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<ItemRowModel>(dynamicDbHelperMySQL.Query(sql, ctx.GetParameters((MySqlParameter)null)).Tables[0]);
		}

		public BDItemModel GetModelByKey(string MItemID, MContext ctx)
		{
			return ModelInfoManager.GetDataEditModel<BDItemModel>(ctx, MItemID, false, true);
		}

		public BDItemModel GetBDModelByKey(string MItemID, MContext ctx)
		{
			return ModelInfoManager.GetDataEditModel<BDItemModel>(ctx, MItemID, false, true);
		}

		public OperationResult IsImportItemsCodeExist(MContext ctx, List<BDItemModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<string> list2 = (from f in list
			select f.MNumber).ToList();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter(new SqlFilter("MNumber", SqlOperators.In, list2.ToArray()));
			List<BDItemModel> dataModelList = ModelInfoManager.GetDataModelList<BDItemModel>(ctx, sqlWhere, false, false);
			if (dataModelList.Any())
			{
				List<string> values = (from f in dataModelList
				select f.MNumber).ToList();
				operationResult.VerificationInfor = new List<BizVerificationInfor>();
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportFailed4ExistingNumber", "The number:{0} already exists, import failed!");
				string message = string.Format(text, string.Join(",", values));
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					CheckItem = "商品导入存在性检查",
					Level = AlertEnum.Error,
					Message = message
				});
				return operationResult;
			}
			return operationResult;
		}

		public OperationResult ImportItemList(MContext ctx, List<BDItemModel> list)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				operationResult = ModelInfoManager.InsertOrUpdate(ctx, list, null);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		public OperationResult UpdateItemModel(MContext ctx, BDItemModel model)
		{
			model.MOrgID = ctx.MOrgID;
			if (IsItemCodeExists(ctx, model.MItemID, model))
			{
				OperationResult operationResult = new OperationResult();
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Existencefailed", "The code has been in existence, save failed!")
				});
				return operationResult;
			}
			return ModelInfoManager.InsertOrUpdate<BDItemModel>(ctx, model, null);
		}

		public OperationResult DeleteList(MContext ctx, ParamBase param)
		{
			List<string> list = new List<string>();
			OperationResult result = BDRepository.IsCanDelete(ctx, "Item", param.KeyIDs, out list);
			if (list.Count > 0)
			{
				ModelInfoManager.Delete<BDItemModel>(ctx, list);
			}
			return result;
		}

		public bool IsItemCodeExists(MContext ctx, string MItemID, BDItemModel model)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MNumber", model.MNumber);
			sqlWhere.Equal("MOrgID", ctx.MOrgID);
			if (!string.IsNullOrWhiteSpace(MItemID))
			{
				sqlWhere.NotEqual("MItemID", MItemID);
			}
			sqlWhere.Equal("MIsDelete", 0);
			return ModelInfoManager.ExistsByFilter<BDItemModel>(ctx, sqlWhere);
		}

		public List<CommandInfo> UpdateItemMapAccount(MContext ctx, string oldCode, string newCode)
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
			commandInfo.CommandText = "update t_bd_item set MInventoryAccountCode=@newCode where MInventoryAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = "update t_bd_item set MIncomeAccountCode=@newCode where MIncomeAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			array = (commandInfo2.Parameters = parameters);
			list.Add(commandInfo2);
			CommandInfo commandInfo3 = new CommandInfo();
			commandInfo3.CommandText = "update t_bd_item set MCostAccountCode=@newCode where MCostAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			array = (commandInfo3.Parameters = parameters);
			list.Add(commandInfo3);
			return list;
		}

		public int ArchiveItem(MContext ctx, string itemIds, bool isRestore)
		{
			if (string.IsNullOrWhiteSpace(itemIds))
			{
				throw new NullReferenceException("itemids is can be null");
			}
			List<string> list = itemIds.Split(',').ToList();
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (string item in list)
			{
				list2.AddRange(GetArchiveItemCmdList(ctx, isRestore, item));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.ExecuteSqlTran(list2);
		}

		public List<CommandInfo> GetArchiveItemCmdList(MContext ctx, bool isRestore, string itemId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
				{
					Value = itemId
				},
				new MySqlParameter("@MIsActive", MySqlDbType.Int32)
				{
					Value = (object)(isRestore ? 1 : 0)
				}
			};
			List<CommandInfo> archiveFlagCmd = ModelInfoManager.GetArchiveFlagCmd<BDItemModel>(ctx, itemId, isRestore);
			list.AddRange(archiveFlagCmd);
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_bd_item_l set MIsActive=@MIsActive where MOrgID=@MOrgID and MParentid = @MItemID and MIsDelete = 0";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}

		public List<BDItemModel> GetItemListIgnoreLocale(MContext ctx, bool includeDisabled = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.*,b.MDesc as MName from t_bd_item a ");
			stringBuilder.AppendLine(" join t_bd_item_l b on a.MItemID=b.MParentID and a.MOrgID = b.MOrgID and b.MIsDelete = 0 ");
			stringBuilder.AppendLine(" where a.MOrgID=@MOrgID and a.MIsDelete=0");
			if (!includeDisabled)
			{
				stringBuilder.AppendLine(" and a.MIsActive=1");
			}
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			return ModelInfoManager.GetDataModelBySql<BDItemModel>(ctx, stringBuilder.ToString(), parameters);
		}
	}
}
