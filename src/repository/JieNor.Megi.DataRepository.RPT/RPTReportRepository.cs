using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTReportRepository
	{
		public static void AddReport(MContext ctx, RPTReportModel model)
		{
			ModelInfoManager.InsertOrUpdate<RPTReportModel>(ctx, model, null);
		}

		public static string AddEmptyReport(MContext ctx, List<RPTReportModel> modelList)
		{
			string text = string.Empty;
			int num = 0;
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (RPTReportModel model in modelList)
			{
				model.MParentID = text;
				model.MStatus = Convert.ToInt32(RPTReportStatus.Draft);
				model.MAuthor = "";
				model.MOrgID = ctx.MOrgID;
				model.MIsActive = false;
				List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<RPTReportModel>(ctx, model, null, true);
				list.AddRange(insertOrUpdateCmd);
				if (num == 0)
				{
					text = model.MID;
				}
				num++;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return text;
		}

		public static string AddReport(MContext ctx, List<BizReportModel> modelList)
		{
			string text = string.Empty;
			int num = 0;
			List<CommandInfo> list = new List<CommandInfo>();
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			foreach (BizReportModel model in modelList)
			{
				RPTReportModel rPTReportModel = new RPTReportModel();
				rPTReportModel.MParentID = text;
				rPTReportModel.MType = model.Type;
				rPTReportModel.MStatus = Convert.ToInt32(RPTReportStatus.Draft);
				rPTReportModel.MTitle = model.Title1;
				rPTReportModel.MSubtitle = model.Title2;
				rPTReportModel.MAuthor = "";
				rPTReportModel.MSheetName = model.Title1;
				rPTReportModel.MOrgID = ctx.MOrgID;
				rPTReportModel.MIsActive = false;
				rPTReportModel.MIsShow = model.IsShow;
				List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<RPTReportModel>(ctx, rPTReportModel, null, true);
				model.ReportID = rPTReportModel.MID;
				model.Filter.MReportID = rPTReportModel.MID;
				list.AddRange(insertOrUpdateCmd);
				if (num == 0)
				{
					text = rPTReportModel.MID;
				}
				num++;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return text;
		}

		public static OperationResult UpdateInActiveReport(MContext ctx, RPTReportModel model)
		{
			model.MIsActive = false;
			return ModelInfoManager.InsertOrUpdate<RPTReportModel>(ctx, model, null);
		}

		public static bool UpdateReportInfo(MContext ctx, RPTReportModel model)
		{
			List<RPTReportSheetModel> reportSheetList = GetReportSheetList(model.MID, ctx);
			model.MIsActive = true;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_RPT_Report set ");
			stringBuilder.Append(" MModifierID = @MModifierID , ");
			stringBuilder.Append(" MModifyDate = @MModifyDate , ");
			stringBuilder.Append(" MTitle = IFNULL(@MTitle,MTitle) , ");
			stringBuilder.Append(" MSubtitle = @MSubtitle , ");
			stringBuilder.Append(" MAuthor = @MAuthor , ");
			stringBuilder.Append(" MReportDate =@MReportDate,  ");
			stringBuilder.Append(" MStatus = @MStatus,  ");
			stringBuilder.Append(" MSheetName = IFNULL(@MSheetName,MSheetName), ");
			stringBuilder.Append(" MIsActive =1, ");
			stringBuilder.Append(" MPublishDate =@MPublishDate ");
			stringBuilder.Append(" where MID=@MID  ");
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter("@MID", model.MID));
			list.Add(new MySqlParameter("@MModifierID", ctx.MUserID));
			list.Add(new MySqlParameter("@MModifyDate", ctx.DateNow));
			list.Add(new MySqlParameter("@MTitle", model.MTitle));
			list.Add(new MySqlParameter("@MSubtitle", model.MSubtitle));
			list.Add(new MySqlParameter("@MAuthor", model.MAuthor));
			list.Add(new MySqlParameter("@MReportDate", model.MReportDate));
			list.Add(new MySqlParameter("@MStatus", model.MStatus));
			list.Add(new MySqlParameter("@MSheetName", model.MSheetName));
			list.Add(new MySqlParameter("@MPublishDate", ctx.DateNow));
			if (model.MStatus == Convert.ToInt32(RPTReportStatus.Published))
			{
				for (int i = 0; i < reportSheetList.Count; i++)
				{
					stringBuilder.AppendFormat(" OR MID=@MID{0} ", i);
					list.Add(new MySqlParameter($"@MID{i}", reportSheetList[i].MID));
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSql(stringBuilder.ToString(), list.ToArray());
			if (num > 0)
			{
				return true;
			}
			return false;
		}

		public static bool UpdateReportContent(MContext ctx, RPTReportModel model, bool isActive = true)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_RPT_Report set ");
			stringBuilder.Append(" MTitle = IFNULL(@MTitle,MTitle) , ");
			stringBuilder.Append(" MSubtitle = IFNULL(@MSubtitle,MSubtitle) , ");
			stringBuilder.Append(" MContent = IFNULL(@MContent,MContent) , ");
			stringBuilder.Append(" MModifierID = IFNULL(@MModifierID,MModifierID) , ");
			stringBuilder.Append(" MModifyDate = IFNULL(@MModifyDate,MModifyDate),  ");
			if (isActive)
			{
				stringBuilder.Append(" MIsActive = 1 ");
			}
			else
			{
				stringBuilder.Append(" MIsActive = MIsActive ");
			}
			stringBuilder.Append(" where MID=@MID  ");
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContent", MySqlDbType.LongText),
				new MySqlParameter("@MModifierID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MModifyDate", MySqlDbType.DateTime),
				new MySqlParameter("@MTitle", model.MTitle),
				new MySqlParameter("@MSubtitle", model.MSubtitle)
			};
			array[0].Value = model.MID;
			array[1].Value = model.MContent;
			array[2].Value = ctx.MUserID;
			array[3].Value = model.MModifyDate;
			array[4].Value = model.MTitle;
			array[5].Value = model.MSubtitle;
			List<CommandInfo> list2 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			};
			DbParameter[] array2 = obj.Parameters = array;
			list2.Add(obj);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0)
			{
				return true;
			}
			return false;
		}

		public static RPTReportModel GetReportModel(string reportId, MContext ctx)
		{
			string sql = "select MID, MIsShowCoverPage, MIsShowTableContent, MPDFBorderColor, MFooterText, r.MIsActive, r.MIsDelete, r.MCreatorID, r.MCreateDate, r.MModifierID, r.MModifyDate, r.MOrgID, MType, MStatus, MTitle, MSubtitle, MReportDate, \r\n                            CASE WHEN IFNULL(r.MAuthor,'')='' THEN  F_GetUserName(u.MFristName,u.MLastName) ELSE r.MAuthor END  MAuthor,\r\n                            r.MContent ,r.MPublishDate,r.MParentID,r.MSheetName,r.MIsActive\r\n                            from T_RPT_Report r\r\n                            LEFT JOIN  T_Sec_User_L u ON r.MCreatorID=u.MParentID\r\n                            WHERE r.MID=@MID and r.MIsDelete=0 ";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = reportId;
			array[1].Value = ctx.MLCID;
			List<RPTReportModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<RPTReportModel>(ctx, sql, array);
			return (dataModelBySql == null || dataModelBySql.Count == 0) ? null : dataModelBySql[0];
		}

		public static RPTReportModel GetReportModel(string parentReportId, BizReportType type, MContext ctx)
		{
			string sql = "SELECT MID, MIsShowCoverPage, MIsShowTableContent, MPDFBorderColor, MFooterText, r.MIsActive, r.MIsDelete, r.MCreatorID, r.MCreateDate, r.MModifierID, r.MModifyDate, r.MOrgID, MType, MStatus, MTitle, MSubtitle, MReportDate, \r\n                            CASE WHEN IFNULL(r.MAuthor,'')='' THEN  F_GetUserName(u.MFristName,u.MLastName) ELSE r.MAuthor END  MAuthor,\r\n                            r.MContent ,r.MPublishDate,r.MParentID,r.MSheetName \r\n                            FROM ( SELECT *\r\n                            FROM T_RPT_Report WHERE MParentID=@MID AND MType=@MType AND MIsDelete=0 AND MOrgID=@MOrgID )r\r\n                            LEFT JOIN  T_Sec_User_L u ON r.MCreatorID=u.MParentID\r\n                            ";
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.Int32),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 100)
			};
			array[0].Value = parentReportId;
			array[1].Value = ctx.MLCID;
			array[2].Value = Convert.ToInt32(type);
			array[3].Value = ctx.MOrgID;
			List<RPTReportModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<RPTReportModel>(ctx, sql, array);
			return (dataModelBySql == null || dataModelBySql.Count == 0) ? null : dataModelBySql[0];
		}

		public static RPTReportModel GetMainReportModel(string reportId, MContext context)
		{
			RPTReportModel reportModel = GetReportModel(reportId, context);
			if (reportModel != null && !string.IsNullOrEmpty(reportModel.MParentID))
			{
				return GetReportModel(reportModel.MParentID, context);
			}
			return reportModel;
		}

		public static RPTReportModel GetCurrentReportModel(string reportId, MContext context)
		{
			return GetReportModel(reportId, context);
		}

		public static List<RPTReportSheetModel> GetReportSheetList(string reportId, MContext context)
		{
			List<RPTReportSheetModel> list = new List<RPTReportSheetModel>();
			RPTReportModel mainReportModel = GetMainReportModel(reportId, context);
			if (mainReportModel == null)
			{
				return list;
			}
			string sql = "SELECT MID,MType,MSheetName,MCreateDate FROM T_RPT_Report\r\n                            WHERE (IFNULL(MParentID,'')='' AND MID=@MID) OR (MParentID=@MID AND (MIsActive=1 OR MIsShow=1)) AND MIsDelete=0\r\n                            ORDER BY MCreateDate";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = mainReportModel.MID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			DataTable dataTable = dataSet.Tables[0];
			foreach (DataRow row in dataTable.Rows)
			{
				RPTReportSheetModel rPTReportSheetModel = new RPTReportSheetModel();
				rPTReportSheetModel.MID = row["MID"].ToString();
				rPTReportSheetModel.MType = Convert.ToInt32(row["MType"]);
				rPTReportSheetModel.MSheetName = row["MSheetName"].ToString();
				list.Add(rPTReportSheetModel);
			}
			return list;
		}

		public static List<RPTReportModel> GetDraftReportList(MContext ctx, RPTReportQueryParam param)
		{
			List<int> list = new List<int>();
			bool flag = SECPermissionRepository.HavePermission(ctx, "Sale_Reports", "View", "");
			if (flag)
			{
				int[] source = new int[3]
				{
					6,
					9,
					21
				};
				list.AddRange(source.ToList());
			}
			bool flag2 = SECPermissionRepository.HavePermission(ctx, "Purchase_Reports", "View", "");
			if (flag2)
			{
				int[] source2 = new int[1]
				{
					19
				};
				list.AddRange(source2.ToList());
			}
			bool flag3 = SECPermissionRepository.HavePermission(ctx, "Bank_Reports", "View", "");
			if (flag3)
			{
				int[] source3 = new int[4]
				{
					12,
					13,
					11,
					8
				};
				list.AddRange(source3.ToList());
			}
			if (SECPermissionRepository.HavePermission(ctx, "Expense_Reports", "View", ""))
			{
				int[] source4 = new int[1]
				{
					38
				};
				list.AddRange(source4.ToList());
			}
			bool flag4 = SECPermissionRepository.HavePermission(ctx, "Other_Reports", "View", "");
			if (flag4)
			{
				int[] source5 = new int[6]
				{
					41,
					33,
					42,
					23,
					39,
					40
				};
				list.AddRange(source5.ToList());
			}
			bool flag5 = SECPermissionRepository.HavePermission(ctx, "PayRun_Reports", "View", "");
			if (flag5)
			{
				int[] source6 = new int[0];
				list.AddRange(source6.ToList());
			}
			bool flag6 = SECPermissionRepository.HavePermission(ctx, "Fixed_Assets_Reports", "View", "");
			if (flag6)
			{
				int[] source7 = new int[4]
				{
					501,
					502,
					503,
					504
				};
				list.AddRange(source7.ToList());
			}
			string str = string.Join(",", list);
			string str2 = " and r.MType in (" + str + ") ";
			if (flag & flag2 & flag3 & flag5 & flag4 & flag6)
			{
				str2 = string.Empty;
			}
			string arg = "select distinct MID, MIsShowCoverPage, MIsShowTableContent, MPDFBorderColor, MFooterText, r.MIsActive, r.MIsDelete, r.MCreatorID, r.MCreateDate, r.MModifierID, r.MModifyDate, r.MOrgID, MType, MStatus, MTitle, MSubtitle, MReportDate, \r\n                            CASE WHEN r.MAuthor='' THEN F_GetUserName(u.MFristName,u.MLastName) ELSE r.MAuthor END  MAuthor,\r\n                            '' AS MContent ,r.MPublishDate,r.MParentID,r.MSheetName\r\n                            from T_RPT_Report r\r\n                            LEFT JOIN  T_Sec_User_L u ON r.MCreatorID=u.MParentID  and u.MLocaleID = @MLCID and u.MIsDelete = 0 and u.MIsActive = 1\r\n                            WHERE r.MOrgID=@MOrgID AND r.MStatus=@MStatus AND r.MIsActive=1 AND r.MIsDelete=0 AND IFNULL(r.MParentID,'')='' " + str2 + "\r\n                             ORDER BY r.MCreateDate DESC";
			arg = $" {arg} {param.PageSqlString} ";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStatus", MySqlDbType.Int32, 11),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = Convert.ToInt32(param.Status);
			array[2].Value = ctx.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(arg, array);
			return ModelInfoManager.DataTableToList<RPTReportModel>(dataSet.Tables[0]);
		}

		public static List<RPTReportModel> GetReportList(MContext ctx, RPTReportQueryParam param)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select MID, MIsShowCoverPage, MIsShowTableContent, MPDFBorderColor, MFooterText, MIsActive, MIsDelete, MCreatorID, MCreateDate, MModifierID, MModifyDate, MOrgID, MType, MStatus, MTitle, MSubtitle, MReportDate, MAuthor, '' AS MContent,MPublishDate,MParentID,MSheetName  ");
			stringBuilder.Append("  from T_RPT_Report ");
			stringBuilder.Append(" where MOrgID=@MOrgID AND MStatus=@MStatus  AND MIsActive=1 AND MIsDelete=0  AND IFNULL(r.MParentID,'')='' ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStatus", MySqlDbType.Int32, 11)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = Convert.ToInt32(param.Status);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return ModelInfoManager.DataTableToList<RPTReportModel>(dataSet.Tables[0]);
		}

		public static OperationResult DeleteReport(MContext ctx, List<string> mids)
		{
			return ModelInfoManager.Delete<RPTReportModel>(ctx, mids);
		}

		public static List<RPTReportLayoutModel> GetReportLayoutList(MContext context, string printSettingId = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select * from T_RPT_ReportLayout where MIsActive=1 and MIsDelete=0 and MOrgID=@MOrgID");
			if (!string.IsNullOrWhiteSpace(printSettingId))
			{
				stringBuilder.AppendLine(" and MPrintSettingID=@MPrintSettingID ");
			}
			stringBuilder.AppendLine(" order by MCreateDate desc ");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", context.MOrgID),
				new MySqlParameter("@MPrintSettingID", printSettingId)
			};
			return ModelInfoManager.GetDataModelBySql<RPTReportLayoutModel>(context, stringBuilder.ToString(), cmdParms);
		}

		public static RPTReportLayoutModel GetReportLayoutModel(BizReportType reportType, string printSettingID, MContext context)
		{
			string sql = "select * from T_RPT_ReportLayout where MIsActive=1 and MIsDelete=0 and MReportType=@MReportType and MOrgID=@MOrgID order by MCreateDate desc";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MReportType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = reportType;
			array[1].Value = context.MOrgID;
			List<RPTReportLayoutModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<RPTReportLayoutModel>(context, sql, array);
			if (!string.IsNullOrWhiteSpace(printSettingID))
			{
				return dataModelBySql.FirstOrDefault((RPTReportLayoutModel f) => f.MPrintSettingID == printSettingID);
			}
			return dataModelBySql.FirstOrDefault();
		}

		public static OperationResult UpdateReportLayout(MContext ctx, RPTReportLayoutModel model)
		{
			if (string.IsNullOrWhiteSpace(model.MID))
			{
				model.IsNew = true;
			}
			else
			{
				model.IsUpdate = true;
			}
			return ModelInfoManager.InsertOrUpdate<RPTReportLayoutModel>(ctx, model, null);
		}

		public static List<CommandInfo> GetDelReportLayoutCmds(MContext ctx, List<string> printSettingIdList)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			if (printSettingIdList == null || printSettingIdList.Count == 0)
			{
				return result;
			}
			List<RPTReportLayoutModel> dataModelList = ModelInfoManager.GetDataModelList<RPTReportLayoutModel>(ctx, new SqlWhere().In("MPrintSettingID", printSettingIdList), false, false);
			if (dataModelList.Any())
			{
				result = ModelInfoManager.GetDeleteFlagCmd<RPTReportLayoutModel>(ctx, (from f in dataModelList
				select f.MID).ToList());
			}
			return result;
		}

		public static OperationResult RestoreReport(string mid, MContext context)
		{
			return ModelInfoManager.DeleteFlag<RPTReportLayoutModel>(context, mid);
		}

		public static string GetMultiLangKey(MContext ctx)
		{
			string empty = string.Empty;
			if (ctx.MLCID == LangCodeEnum.ZH_CN)
			{
				return "zh-cn";
			}
			if (ctx.MLCID == LangCodeEnum.ZH_TW)
			{
				return "zh-tw";
			}
			return "en-us";
		}
	}
}
