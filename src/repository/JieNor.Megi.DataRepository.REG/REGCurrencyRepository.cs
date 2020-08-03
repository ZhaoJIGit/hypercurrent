using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.REG
{
	public class REGCurrencyRepository : DataServiceT<REGCurrencyModel>
	{
		private string multLangFieldSql = "\r\n            ,t2.MName{0} ";

		private string CommonSelect = "select \r\n                t1.MItemID, \r\n                t1.MCurrencyID, \r\n                t1.MModifyDate, \r\n                t2.MName\r\n                #_#lang_field0#_#  \r\n            from\r\n                (select \r\n                    MItemID, MOrgID, MCurrencyID, MModifyDate, MIsDelete\r\n                from\r\n                    t_reg_financial union all select \r\n                    MItemID, MOrgID, MCurrencyID, MModifyDate, MIsDelete\r\n                from\r\n                    T_REG_Currency) t1\r\n                    left join\r\n                @_@t_bas_currency_l@_@ t2 ON t1.MCurrencyID = t2.MParentID\r\n                    and t2.MIsDelete = 0\r\n            where\r\n                t1.MOrgID = @MOrgID\r\n                    and t1.MIsDelete = 0";

		public DataGridJson<REGCurrencyViewModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<REGCurrencyViewModel>(ctx, param, CommonSelect, multLangFieldSql, false, true, null);
		}

		public List<REGCurrencyModel> GetList(MContext context)
		{
			return new List<REGCurrencyModel>();
		}

		public List<REGCurrencyViewModel> GetCurrencyViewList(MContext context, DateTime? endDate, bool isIncludeBase = false, GetParam param = null)
		{
			List<REGCurrencyViewModel> viewList = GetViewList(context, endDate, null, false, param);
			if (isIncludeBase)
			{
				BASCurrencyViewModel @base = GetBase(context, false, null, null);
				if (@base != null)
				{
					REGCurrencyViewModel rEGCurrencyViewModel = new REGCurrencyViewModel();
					rEGCurrencyViewModel.MCurrencyID = @base.MCurrencyID;
					rEGCurrencyViewModel.MName = @base.MLocalName;
					rEGCurrencyViewModel.MNumber = @base.MCurrencyID;
					rEGCurrencyViewModel.MUserRate = "1";
					viewList.Insert(0, rEGCurrencyViewModel);
				}
			}
			return viewList;
		}

		public BASCurrencyViewModel GetBase(MContext context, bool ignoreLocale = false, List<BASCurrencyViewModel> baseList = null, string currencyNames = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select distinct t1.MItemID MCurrencyID,t1.MItemID AS MNumber, L.MName as MLocalName from T_Bas_Currency t1");
			stringBuilder.Append(" join T_Bas_Currency_L L on t1.MItemID = L.MParentID");
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MLocaleID", context.MLCID),
				new MySqlParameter("@MOrgID", context.MOrgID)
			};
			stringBuilder.AppendFormat(" and ({0})", GetCurrencyNameParam(currencyNames, list, ignoreLocale));
			stringBuilder.AppendLine(" join T_REG_Financial  t3 on t3.MCurrencyID = t1.MItemID");
			stringBuilder.Append(" where t3.MIsDelete = 0 and t3.MOrgID= @MOrgID");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			DataTable dt = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list.ToArray()).Tables[0];
			List<BASCurrencyViewModel> list2 = ModelInfoManager.DataTableToList<BASCurrencyViewModel>(dt);
			if (list2.Count > 0)
			{
				baseList?.AddRange(list2);
				return list2[0];
			}
			if (!string.IsNullOrWhiteSpace(currencyNames))
			{
				BASCurrencyViewModel @base = GetBase(context, false, null, null);
				baseList?.Add(@base);
				return @base;
			}
			return null;
		}

		public Dictionary<string, decimal> GetBaseCurrencyRate(MContext ctx, DateTime? endDate)
		{
			List<REGCurrencyViewModel> viewList = new REGCurrencyRepository().GetViewList(ctx, endDate, null, false, null);
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			for (int i = 0; i < viewList.Count; i++)
			{
				dictionary.Add(viewList[i].MCurrencyID, string.IsNullOrEmpty(viewList[i].MUserRate) ? decimal.Zero : decimal.Parse(viewList[i].MUserRate));
			}
			return dictionary;
		}

		public List<REGCurrencyViewModel> GetAllCurrencyList(MContext context, bool ignoreLocale = false)
		{
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", context.MOrgID),
				new MySqlParameter("@MLocaleID", context.MLCID),
				new MySqlParameter("@endDate", DateTime.MaxValue.ToString("yyyy/MM/dd 23:59:59"))
			};
			string currencyNameParam = GetCurrencyNameParam(null, list, ignoreLocale);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select \r\n\t            T.MItemID,\r\n\t            T.MCurrencyID, \r\n\t            S.MRateDate,\r\n\t            S.MUserRate,\r\n                S.MRate,\r\n\t            L.MName\r\n\t            from \r\n\t            T_REG_Currency T\r\n\t            join  T_Bas_Currency_L L \r\n\t\t            on T.MCurrencyID = L.MParentID \r\n\t\t            and {0}\r\n\t            left join T_BD_ExchangeRate S \r\n\t\t            on S.MTargetCurrencyID = T.MCurrencyID \r\n\t\t            and S.MOrgID = T.MOrgID \r\n\t\t            and S.MIsDelete = 0\r\n\t            where 1=1\r\n                and T.MOrgID = @MOrgID\r\n                and T.MIsDelete = 0 \r\n                and S.MUserRate is null", currencyNameParam);
			stringBuilder.AppendLine("  union all ");
			stringBuilder.AppendFormat("select \r\n\t            T.MItemID,\r\n\t            T.MCurrencyID, \r\n\t            S.MRateDate,\r\n\t            S.MUserRate,\r\n                S.MRate,\r\n\t            L.MName\r\n\t            from \r\n\t            T_REG_Currency T\r\n\t            join  T_Bas_Currency_L L \r\n\t\t            on T.MCurrencyID = L.MParentID \r\n\t\t            and {0}\r\n\t            join T_BD_ExchangeRate S \r\n\t\t            on S.MTargetCurrencyID = T.MCurrencyID \r\n\t\t            and S.MOrgID = T.MOrgID \r\n\t\t            and S.MIsDelete = 0\r\n\t            where 1=1\r\n               and  T.MOrgID = @MOrgID\r\n                and T.MIsDelete = 0 \r\n                and S.MRateDate <= @endDate", currencyNameParam);
			stringBuilder.AppendLine(" union all");
			stringBuilder.AppendFormat("\r\n                select \r\n\t            T.MItemID,\r\n\t            T.MCurrencyID, \r\n\t            S.MRateDate,\r\n\t            S.MUserRate,\r\n                S.MRate,\r\n\t            L.MName\r\n\t            from \r\n\t            T_REG_Currency T\r\n\t            join  T_Bas_Currency_L L \r\n\t\t            on T.MCurrencyID = L.MParentID \r\n\t\t            and {0}\r\n\t            join T_BD_ExchangeRate S \r\n\t\t            on S.MTargetCurrencyID = T.MCurrencyID \r\n\t\t            and S.MOrgID = T.MOrgID \r\n\t\t            and S.MIsDelete = 0\r\n\t            where 1=1\r\n               and  T.MOrgID = @MOrgID\r\n                and T.MIsDelete = 0 \r\n                and S.MRateDate > @endDate", currencyNameParam);
			return ModelInfoManager.GetDataModelBySql<REGCurrencyViewModel>(context, stringBuilder.ToString(), list.ToArray());
		}

		public List<REGCurrencyViewModel> GetViewList(MContext context, DateTime? endDate, string currencyNames = null, bool ignoreLocale = false, GetParam param = null)
		{
			endDate = (endDate.HasValue ? endDate : new DateTime?(context.DateNow));
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", context.MOrgID),
				new MySqlParameter("@MLocaleID", context.MLCID),
				new MySqlParameter("@endDate", endDate.Value.ToDayLastSecond()),
				new MySqlParameter("@MModifyDate", param?.ModifiedSince ?? DateTime.MinValue)
			};
			string currencyNameParam = GetCurrencyNameParam(currencyNames, list, ignoreLocale);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select \r\n\t                                    T.MItemID,\r\n\t                                    T.MCurrencyID, \r\n\t                                    T.MCurrencyID AS MNumber, \r\n\t                                    S.MRateDate,\r\n\t                                    S.MUserRate,\r\n                                        S.MRate,\r\n\t                                    L.MName\r\n\t                                    from \r\n\t                                    T_REG_Currency T\r\n\t                                    join  T_Bas_Currency_L L \r\n\t\t                                    on T.MCurrencyID = L.MParentID \r\n\t\t                                    and ({0})\r\n\t                                    left join T_BD_ExchangeRate S \r\n\t\t                                    on S.MTargetCurrencyID = T.MCurrencyID \r\n\t\t                                    and S.MOrgID = T.MOrgID \r\n\t\t                                    and S.MIsDelete = 0 and S.MRateDate <= @endDate\r\n\t                                    where 1=1\r\n                                        and T.MOrgID = @MOrgID\r\n                                        and T.MIsDelete = 0 \r\n                                        and S.MUserRate is null", currencyNameParam);
			if (param != null && param.ModifiedSince > DateTime.MinValue)
			{
				stringBuilder.Append(" AND T.MModifyDate > @MModifyDate ");
			}
			stringBuilder.AppendLine("  union all ");
			stringBuilder.AppendFormat(" SELECT \n                                        y.MItemID,\n                                        y.MCurrencyID,\r\n\t                                    y.MCurrencyID AS MNumber, \n                                        x.MRateDate,\n                                        x.MUserRate,\n                                        x.MRate,\n                                        L.MName\n                                    FROM\n                                        T_BD_ExchangeRate x\n                                            INNER JOIN\n                                        (SELECT \n                                            t.MItemID,\n                                            t.MCurrencyID,\n                                                MAX(CONCAT(DATE_FORMAT(t.MRateDate, '%Y-%m-%d'), DATE_FORMAT(t.MModifyDate, '%Y-%m-%d %H:%i:%s'))) MRateModifyDateDate\n                                        FROM\n                                            (SELECT \n                                                T.MItemID,\n                                                T.MCurrencyID,\n                                                S.MRateDate,\n                                                S.MModifyDate,\n                                                S.MUserRate,\n                                                S.MRate\n                                        FROM\n                                            T_REG_Currency T\n                                        JOIN T_BD_ExchangeRate S ON S.MTargetCurrencyID = T.MCurrencyID\n                                            AND S.MOrgID = T.MOrgID\n                                            AND S.MIsDelete = 0\n                                        WHERE\n                                            1 = 1 AND T.MOrgID = @MOrgID\n                                                AND T.MIsDelete = 0\n                                                AND S.MRateDate <= @endDate\n                                        ORDER BY MCurrencyID ASC , MRateDate DESC , MModifyDate DESC) t\n                                        GROUP BY t.MItemID, t.MCurrencyID) y ON x.MTargetCurrencyID = y.MCurrencyID\n                                            AND CONCAT(DATE_FORMAT(x.MRateDate, '%Y-%m-%d'),\n                                                DATE_FORMAT(x.MModifyDate, '%Y-%m-%d %H:%i:%s')) = y.MRateModifyDateDate\n                                            AND x.MIsDelete = 0\n                                            AND x.MOrgID = @MOrgID\n                                            JOIN\n                                        T_Bas_Currency_L L ON x.MTargetCurrencyID = L.MParentID\n                                            and ({0}) ", currencyNameParam);
			stringBuilder.Append(" Order by MName ");
			List<REGCurrencyViewModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<REGCurrencyViewModel>(context, stringBuilder.ToString(), list.ToArray());
			dataModelBySql.ForEach(delegate(REGCurrencyViewModel x)
			{
				DateTime mRateDate2 = x.MRateDate;
				DateTime mRateDate = x.MRateDate;
				DateTime? t = endDate;
				if ((DateTime?)mRateDate > t)
				{
					x.MUserRate = null;
					x.MRateDate = default(DateTime);
				}
			});
			return dataModelBySql;
		}

		public List<REGCurrencyViewModel> GetBillViewList(MContext context, DateTime? endDate, string currencyNames = null, bool ignoreLocale = false)
		{
			endDate = (endDate.HasValue ? endDate : new DateTime?(context.DateNow));
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", context.MOrgID),
				new MySqlParameter("@MLocaleID", context.MLCID),
				new MySqlParameter("@endDate", endDate.Value.ToDayLastSecond())
			};
			string currencyNameParam = GetCurrencyNameParam(currencyNames, list, ignoreLocale);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" select \r\n\t                                    T.MItemID,\r\n\t                                    T.MCurrencyID, \r\n\t                                    S.MRateDate,\r\n\t                                    S.MUserRate,\r\n                                        S.MRate,\r\n\t                                    L.MName,\r\n                                        S.MItemID AS MExchangeRateID\r\n\t                                    from \r\n\t                                    T_REG_Currency T\r\n\t                                    join  T_Bas_Currency_L L \r\n\t\t                                    on T.MCurrencyID = L.MParentID \r\n\t\t                                    and ({0})\r\n\t                                    left join T_BD_ExchangeRate S \r\n\t\t                                    on S.MTargetCurrencyID = T.MCurrencyID \r\n\t\t                                    and S.MOrgID = T.MOrgID \r\n\t\t                                    and S.MIsDelete = 0 and S.MRateDate <= @endDate\r\n\t                                    where 1=1\r\n                                        and T.MOrgID = @MOrgID\r\n                                        and T.MIsDelete = 0 \r\n                                        and S.MUserRate is null", currencyNameParam);
			stringBuilder.AppendLine("  union all ");
			stringBuilder.AppendFormat("  SELECT t3.MItemID,  t3.MCurrencyID, t1.MRateDate, t1.MUserRate, t1.MRate, L.MName, t1.MItemID AS MExchangeRateID \n                                         from T_BD_ExchangeRate t1\n                                         INNER JOIN \n                                         (\n\t                                        SELECT MTargetCurrencyID,MAX(CONCAT(DATE_FORMAT(MRateDate, '%Y-%m-%d'), DATE_FORMAT(MModifyDate, '%Y-%m-%d %H:%i:%s'))) MRateModifyDateDate  \n                                            FROM T_BD_ExchangeRate\n                                            WHERE MOrgID = @MOrgID  AND MIsDelete = 0 AND MRateDate <= @endDate\n                                            GROUP BY MTargetCurrencyID\n                                         ) t2 ON t1.MTargetCurrencyID=t2.MTargetCurrencyID AND CONCAT(DATE_FORMAT(t1.MRateDate, '%Y-%m-%d'), DATE_FORMAT(t1.MModifyDate, '%Y-%m-%d %H:%i:%s'))=t2.MRateModifyDateDate\n                                         INNER JOIN T_REG_Currency t3 ON t1.MTargetCurrencyID = t3.MCurrencyID AND t3.MOrgID = t1.MOrgID AND t3.MIsDelete = 0\n                                         inner join T_Bas_Currency_L L ON t3.MCurrencyID = L.MParentID and ({0})\n                                         WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0 AND t1.MRateDate <= @endDate\n                                            ", currencyNameParam);
			stringBuilder.Append(" Order by MName ");
			List<REGCurrencyViewModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<REGCurrencyViewModel>(context, stringBuilder.ToString(), list.ToArray());
			dataModelBySql.ForEach(delegate(REGCurrencyViewModel x)
			{
				DateTime mRateDate2 = x.MRateDate;
				DateTime mRateDate = x.MRateDate;
				DateTime? t = endDate;
				if ((DateTime?)mRateDate > t)
				{
					x.MUserRate = null;
					x.MRateDate = default(DateTime);
				}
			});
			return dataModelBySql;
		}

		public REGCurrencyModel GetModel(MContext context)
		{
			List<REGCurrencyModel> list = GetList(context);
			if (list != null && list.Count > 0)
			{
				return list[0];
			}
			return null;
		}

		public bool ExistsByCurrencyID(string MCurrencyID, string MOrgID, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select count(1) from T_REG_Currency");
			stringBuilder.Append(" where ");
			stringBuilder.Append(" MCurrencyID = @MCurrencyID AND MOrgID=@MOrgID ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = MCurrencyID;
			array[1].Value = MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(stringBuilder.ToString(), array);
		}

		public OperationResult InsertCurrency(MContext ctx, REGCurrencyModel model)
		{
			string text = " SELECT COUNT(1) FROM T_REG_Currency WHERE  MCurrencyID = @MCurrencyID AND MOrgID=@MOrgID AND MIsDelete=0 ";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MCurrencyID;
			array[1].Value = model.MOrgID;
			array[2].Value = model.MItemID;
			if (model.MItemID != null)
			{
				text += " AND MItemID !=@MItemID ";
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			if (dynamicDbHelperMySQL.Exists(text, array))
			{
				return new OperationResult
				{
					Success = false,
					HaveError = true,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "CurrencyExists", "货币已存在！")
				};
			}
			return InsertOrUpdate(ctx, model, null);
		}

		public OperationResult DeleteCurrency(MContext ctx, REGCurrencyModel model)
		{
			string sql = "Update T_REG_Currency set MIsDelete = 1\r\n                            where MItemID = @MItemID";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MItemID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return new OperationResult
			{
				Success = (dynamicDbHelperMySQL.ExecuteSql(sql, array) == 1)
			};
		}

		public OperationResult SetCurrencyUndeleted(MContext ctx, REGCurrencyModel model)
		{
			string sql = "Update T_REG_Currency set MIsDelete = 0\r\n                            WHERE  MItemID = @MItemID ";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MItemID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return new OperationResult
			{
				Success = (dynamicDbHelperMySQL.ExecuteSql(sql, array) == 1)
			};
		}

		public static List<GlobalCurrencyModel> GetOrgCurrencyModel(MContext ctx)
		{
			if (ctx == null || string.IsNullOrWhiteSpace(ctx.MOrgID))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT MCurrencyID,MPricePrecision,MAmountPrecision,MCurrencySymbol FROM T_REG_Currency");
			stringBuilder.AppendLine("where MIsDelete=0 AND MOrgID=@MOrgID");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			return ModelInfoManager.GetDataModelBySql<GlobalCurrencyModel>(ctx, stringBuilder.ToString(), array);
		}

		private static string GetCurrencyNameParam(string currencyNames, List<MySqlParameter> parameters, bool ignoreLocale = false)
		{
			if (string.IsNullOrWhiteSpace(currencyNames))
			{
				return ignoreLocale ? " 1=1" : " L.MLocaleID = @MLocaleID";
			}
			int num = 1;
			string empty = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = currencyNames.Split(',').ToList();
			foreach (string item in list)
			{
				if (num > 1 && num <= list.Count())
				{
					stringBuilder.Append(" or ");
				}
				empty = $"@MName{num}";
				stringBuilder.AppendFormat(" locate(L.MName, {0}) > 0", empty);
				parameters.Add(new MySqlParameter(empty, item));
				num++;
			}
			foreach (string item2 in list)
			{
				stringBuilder.Append(" or ");
				empty = $"@MCurrencyID{num}";
				stringBuilder.AppendFormat(" locate(L.MParentID, {0}) > 0", empty);
				parameters.Add(new MySqlParameter(empty, item2.ToUpper()));
				num++;
			}
			return string.Join(",", stringBuilder);
		}

		public static OperationResult CheckCurrency(MContext ctx, string currencyId, DateTime endDate)
		{
			OperationResult operationResult = new OperationResult();
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			BASCurrencyViewModel @base = rEGCurrencyRepository.GetBase(ctx, false, null, null);
			if (@base.MCurrencyID == currencyId)
			{
				return operationResult;
			}
			List<GlobalCurrencyModel> orgCurrencyModel = GetOrgCurrencyModel(ctx);
			if (!orgCurrencyModel.Any((GlobalCurrencyModel t) => t.MCurrencyID == currencyId))
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "CurrencyNotExists", "Currency doesn't exists!");
				operationResult.Success = false;
			}
			return operationResult;
		}

		public static decimal GetExchangeRate(MContext ctx, string currencyId, DateTime endDate, out OperationResult result)
		{
			result = new OperationResult();
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			BASCurrencyViewModel @base = rEGCurrencyRepository.GetBase(ctx, false, null, null);
			if (@base.MCurrencyID == currencyId)
			{
				result.Success = true;
				return decimal.One;
			}
			result.Success = false;
			List<REGCurrencyViewModel> viewList = rEGCurrencyRepository.GetViewList(ctx, endDate, null, false, null);
			if (viewList == null || viewList.Count == 0)
			{
				result.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "CurrencyNotExists", "Currency doesn't exists!");
				return decimal.One;
			}
			result.Message = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Bank, "CantFindExchangeRate", "Can't find the exchange rate before {0}!", endDate.ToOrgZoneDateFormat(null));
			REGCurrencyViewModel rEGCurrencyViewModel = (from t in viewList
			where t.MCurrencyID == currencyId
			select t).FirstOrDefault();
			if (rEGCurrencyViewModel == null || string.IsNullOrEmpty(rEGCurrencyViewModel.MRate))
			{
				return decimal.One;
			}
			decimal one = decimal.One;
			if (!decimal.TryParse(rEGCurrencyViewModel.MRate, out one))
			{
				return decimal.One;
			}
			result.Success = true;
			result.Message = "";
			return one;
		}
	}
}
