using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.FP
{
	public class FPImportRepository : DataServiceT<FPImpportModel>
	{
		public List<CommandInfo> GetSaveFPImportCmds(MContext ctx, List<FPImpportModel> fpImportlist)
		{
			return ModelInfoManager.GetInsertOrUpdateCmds(ctx, fpImportlist, null, true);
		}

		public int GetImportListCountByFilter(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			string fPImportListByFilterQuerySql = GetFPImportListByFilterQuerySql(ctx, filter, out list, true);
			return int.Parse(new DynamicDbHelperMySQL(ctx).GetSingle(fPImportListByFilterQuerySql, list.ToArray()).ToString());
		}

		public List<FPImpportModel> GetImportListByFilter(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<FPImpportModel> list = new List<FPImpportModel>();
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			string fPImportListByFilterQuerySql = GetFPImportListByFilterQuerySql(ctx, filter, out list2, false);
			List<FPImpportModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<FPImpportModel>(ctx, fPImportListByFilterQuerySql, list2.ToArray());
			foreach (FPImpportModel item in dataModelBySql)
			{
				item.MFileName = FileHelper.GetOriginalFileName(item.MFileName);
			}
			return dataModelBySql;
		}

		public OperationResult DeleteFPImportByIds(MContext ctx, List<string> idList)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			idList = idList.Distinct().ToList();
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(idList, ref list2, "M_ID");
			string commandText = "UPDATE t_fp_fapiaoentry e INNER JOIN t_fp_fapiao f ON e.mid = f.mid \r\n                                      INNER JOIN t_fp_import im ON f.MImportID = im.MID \r\n                                      SET e.MIsDelete = 1 \r\n                                      WHERE e.MOrgID = @MOrgID AND im.MOrgID=@MOrgID AND e.MIsDelete = 0 AND im.MIsDelete = 0 \r\n                                      AND im.mid " + inFilterQuery;
			List<CommandInfo> list3 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list3.Add(obj);
			string commandText2 = "UPDATE t_fp_fapiao f INNER JOIN t_fp_import im ON f.MImportID = im.MID \r\n                                SET f.MIsDelete = 1 \r\n                                WHERE f.MOrgID = @MOrgID AND im.MOrgID=@MOrgID AND f.MIsDelete = 0 AND im.MIsDelete = 0 \r\n                                AND im.mid " + inFilterQuery;
			List<CommandInfo> list4 = list;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = commandText2
			};
			array = (obj2.Parameters = list2.ToArray());
			list4.Add(obj2);
			string text = "UPDATE t_fp_import SET MIsDelete = 1 WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND mid " + inFilterQuery;
			List<CommandInfo> list5 = list;
			CommandInfo obj3 = new CommandInfo
			{
				CommandText = text.ToString()
			};
			array = (obj3.Parameters = list2.ToArray());
			list5.Add(obj3);
			operationResult = new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) > 0)
			};
			if (operationResult.Success)
			{
				OptLogTemplate template = OptLogTemplate.Sale_FaPiao_Import_Deleted;
				foreach (string id in idList)
				{
					OptLog.AddLog(template, ctx, id, id);
				}
			}
			return operationResult;
		}

		public List<FPImpportModel> GetFPImportDataByImportIds(MContext ctx, List<string> idList)
		{
			List<FPImpportModel> list = new List<FPImpportModel>();
			idList = idList.Distinct().ToList();
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(idList, ref list2, "M_ID");
			string sql = "SELECT MID FROM t_fp_import WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MID " + inFilterQuery;
			return ModelInfoManager.GetDataModelBySql<FPImpportModel>(ctx, sql, list2.ToArray());
		}

		public bool FlagExistAutoData(MContext ctx, List<string> importIdList)
		{
			//bool flag = false;
			importIdList = importIdList.Distinct().ToList();
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(importIdList, ref list, "mimportid");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT COUNT(MID) FROM t_fp_fapiao WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MSource=" + 1);
			stringBuilder.Append(" AND mimportid" + inFilterQuery);
			int num = Convert.ToInt32(new DynamicDbHelperMySQL(ctx).GetSingle(stringBuilder.ToString(), list.ToArray()));
			return num > 0;
		}

		public bool FlagExisCheckData(MContext ctx, List<string> importIdList)
		{
			//bool flag = false;
			importIdList = importIdList.Distinct().ToList();
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(importIdList, ref list, "mimportid");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT COUNT(MID) FROM t_fp_fapiao WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MReconcileStatus=1");
			stringBuilder.Append(" AND mimportid" + inFilterQuery);
			int num = Convert.ToInt32(new DynamicDbHelperMySQL(ctx).GetSingle(stringBuilder.ToString(), list.ToArray()));
			return num > 0;
		}

		public bool FlagExisVoucherData(MContext ctx, List<string> importIdList)
		{
			//bool flag = false;
			importIdList = importIdList.Distinct().ToList();
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(importIdList, ref list, "mimportid");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT COUNT(MID) FROM t_fp_fapiao WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MCodingStatus=1");
			stringBuilder.Append(" AND mimportid" + inFilterQuery);
			int num = Convert.ToInt32(new DynamicDbHelperMySQL(ctx).GetSingle(stringBuilder.ToString(), list.ToArray()));
			return num > 0;
		}

		internal CommandInfo DeleteImportByDeleteFapiaoIds(MContext ctx, List<FPFapiaoModel> fapiaoList)
		{
			List<string> list = new List<string>();
			fapiaoList = (from a in fapiaoList
			where !string.IsNullOrEmpty(a.MImportID)
			select a).ToList();
			IEnumerable<IGrouping<string, FPFapiaoModel>> enumerable = from a in fapiaoList
			group a by a.MImportID;
			List<string> list2 = (from a in enumerable
			select a.Key).ToList();
			if (!list2.Any())
			{
				return null;
			}
			List<MySqlParameter> list3 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(list2, ref list3, "mimportid");
			string sql = "SELECT mid,mimportid FROM t_fp_fapiao WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND mimportid " + inFilterQuery;
			List<FPFapiaoModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<FPFapiaoModel>(ctx, sql, list3.ToArray());
			foreach (IGrouping<string, FPFapiaoModel> item in enumerable)
			{
				string importId = item.Key;
				int num = item.Count();
				int num2 = (from a in dataModelBySql
				where a.MImportID == importId
				select a).Count();
				if (num == num2)
				{
					list.Add(importId);
				}
			}
			if (!list.Any())
			{
				return null;
			}
			List<MySqlParameter> list4 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery2 = GLUtility.GetInFilterQuery(list, ref list4, "M_ID");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE t_fp_import SET MIsDelete=1 ");
			stringBuilder.Append("WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MID " + inFilterQuery2);
			return new CommandInfo(stringBuilder.ToString(), list4.ToArray());
		}

		public string GetFPImportListByFilterQuerySql(MContext ctx, FPFapiaoFilterModel filter, out List<MySqlParameter> parameters, bool count = false)
		{
			parameters = ctx.GetParameters((MySqlParameter)null).ToList();
			string text = " WHERE t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 ";
			if (filter.MStartDate > new DateTime(1900, 1, 1))
			{
				text += " and t1.MDate >= @MStartDate ";
				parameters.Add(new MySqlParameter("@MStartDate", filter.MStartDate));
			}
			if (filter.MFapiaoCategory >= 0)
			{
				text += " and t1.MFapiaoCategory = @MFapiaoCategory ";
				parameters.Add(new MySqlParameter("@MFapiaoCategory", filter.MFapiaoCategory));
			}
			if (filter.MEndDate > new DateTime(1900, 1, 1))
			{
				text += " and t1.MDate <= @MEndDate ";
				parameters.Add(new MySqlParameter("@MEndDate", filter.MEndDate));
			}
			if (count)
			{
				return "SELECT COUNT(t1.MID) AS MCount FROM t_fp_import t1 " + text;
			}
			string text2 = "SELECT t1.MID,t1.MOrgID,t1.MDate,t1.MStartDate,t1.MEndDate,t1.MFileName,t1.MSource,COUNT(fp.mid) MCount,SUM(fp.MTotalAmount) MTotalAmount,\n                            F_GETUSERNAME(max(t.MFristName),max(t.MLastName)) as MOperator \n                            FROM t_fp_import t1 LEFT JOIN T_FP_Fapiao fp ON (t1.mid=fp.mimportid AND t1.morgid = fp.morgid AND fp.MIsDelete = 0)\n                            LEFT JOIN t_sec_user_l t ON t1.MCreatorID = t.MParentID \n                            AND t.MIsDelete = 0 AND t.MLocaleID=@MLocaleID" + text + "GROUP BY t1.mid,t1.MOrgID,t1.mdate,t1.mstartdate,t1.menddate,t1.mfilename,t1.MSource,t1.MCreatorID\n                            ORDER BY t1.MDate DESC ";
			if (filter.rows > 0)
			{
				text2 = text2 + " limit " + (filter.page - 1) * filter.rows + "," + filter.rows;
			}
			return text2;
		}
	}
}
