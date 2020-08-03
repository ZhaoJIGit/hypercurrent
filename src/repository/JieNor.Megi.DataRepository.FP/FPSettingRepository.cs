using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace JieNor.Megi.DataRepository.FP
{
	public class FPSettingRepository : DataServiceT<FPImportTypeConfigModel>
	{
		public List<FPSettingForAutoUploadModel> GetFapiaoSettingList(Dictionary<string, DateTime> dicList, string connectionString)
		{
			List<FPSettingForAutoUploadModel> list = new List<FPSettingForAutoUploadModel>();
			string sQLString = string.Format("select \n                                            t2.MOrgID, t2.MTaxNo, t3.MType, t3.MFPTypes, t3.MMonthAgo\n                                        from\n                                            t_reg_financial t2\n                                                left join\n                                            (select MOrgID,MType,MMonthAgo,(case MImportType when 2 then '0,1' else '' end) as MFPTypes from t_fp_importtypeconfig \n                                                where MIsDelete = 0 and MType=0\n                                             union \n                                             select MOrgID,MType,MMonthAgo,MFPType as MFPTypes from t_fp_importtypeconfig \n                                                where MIsDelete = 0 and MType=1 and MFPType=1) t3 ON t3.MOrgID = t2.MOrgID\n                                        where\n                                            t2.MIsDelete = 0\n                                                and t2.MOrgID in ('{0}')", string.Join("','", dicList.Keys));
			DataSet dataSet = DbHelperMySQL.Query(connectionString, sQLString);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					string value = Convert.ToString(row["MType"]);
					string value2 = Convert.ToString(row["MMonthAgo"]);
					string text = Convert.ToString(row["MOrgID"]);
					list.Add(new FPSettingForAutoUploadModel
					{
						MOrgID = text,
						MTaxNo = Convert.ToString(row["MTaxNo"]),
						MFPTypes = Convert.ToString(row["MFPTypes"]),
						MMonthAgo = ((!string.IsNullOrWhiteSpace(value2)) ? Convert.ToInt32(value2) : 0),
						MType = ((!string.IsNullOrWhiteSpace(value)) ? Convert.ToInt32(value) : 0),
						MGLBeginDate = dicList[text]
					});
				}
			}
			return list;
		}

		public List<FPImportTypeConfigModel> GetFPImportTypeConfigModel(MContext ctx)
		{
			return null;
		}

		public OperationResult SaveImportTypeConfig(MContext ctx, FPImportTypeConfigModel model)
		{
			OperationResult operationResult = new OperationResult();
			string sql = " UPDATE T_FP_ImportTypeConfig \r\n                           SET \r\n                           MAccountNo=@MAccountNo,MPassword=@MPassword,MMonthAgo=@MMonthAgo,MModifierID=@MModifierID,MModifyDate=@MModifyDate,MImportType=2\r\n                           WHERE MItemID=@MItemID ";
			MySqlParameter[] cmdParms = new MySqlParameter[6]
			{
				new MySqlParameter("@MAccountNo", model.MAccountNo),
				new MySqlParameter("@MPassword", model.MPassword),
				new MySqlParameter("@MMonthAgo", model.MMonthAgo),
				new MySqlParameter("@MModifierID", ctx.MUserID),
				new MySqlParameter("@MModifyDate", DateTime.Now),
				new MySqlParameter("@MItemID", model.MItemID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			if (dynamicDbHelperMySQL.ExecuteSql(sql, cmdParms) > 0)
			{
				operationResult.Success = true;
			}
			return operationResult;
		}

		public OperationResult SaveFaPiaoSetting(MContext ctx, FPConfigSettingSaveModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> saveSettingCommandInfos = GetSaveSettingCommandInfos(ctx, model);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(saveSettingCommandInfos) > 0);
			return operationResult;
		}

		public List<CommandInfo> GetSaveSettingCommandInfos(MContext ctx, FPConfigSettingSaveModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(GetImportTypesCommandInfos(ctx, model));
			list.AddRange(GetFaPiaoFieldCommandInfos(ctx, model));
			return list;
		}

		public List<CommandInfo> GetImportTypesCommandInfos(MContext ctx, FPConfigSettingSaveModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string commandText = " UPDATE t_fp_importtypeconfig\r\n                         SET \r\n                         MImportType=@MImportType,MModifierID=@MModifierID,MModifyDate=@MModifyDate \r\n                         WHERE MItemID=@MItemID ";
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = commandText;
			MySqlParameter[] parameters = new MySqlParameter[4]
			{
				new MySqlParameter("@MImportType", model.SpecialTypeModel.MImportType),
				new MySqlParameter("@MModifierID", ctx.MUserID),
				new MySqlParameter("@MModifyDate", DateTime.Now),
				new MySqlParameter("@MItemID", model.SpecialTypeModel.MItemID)
			};
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			if (model.MType == 1)
			{
				CommandInfo commandInfo2 = new CommandInfo();
				commandInfo2.CommandText = commandText;
				MySqlParameter[] parameters2 = new MySqlParameter[4]
				{
					new MySqlParameter("@MImportType", model.ProfessionalType.MImportType),
					new MySqlParameter("@MModifierID", ctx.MUserID),
					new MySqlParameter("@MModifyDate", DateTime.Now),
					new MySqlParameter("@MItemID", model.ProfessionalType.MItemID)
				};
				array = (commandInfo2.Parameters = parameters2);
				list.Add(commandInfo2);
			}
			return list;
		}

		public List<CommandInfo> GetFaPiaoFieldCommandInfos(MContext ctx, FPConfigSettingSaveModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string commandText = " UPDATE t_fp_configsetting \r\n                         SET \r\n                         MValue=@MValue,MModifierID=@MModifierID,MModifyDate=@MModifyDate \r\n                         WHERE MOrgID=@MOrgID AND MIsKey=@MIsKey   ";
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = commandText;
			MySqlParameter[] parameters = new MySqlParameter[5]
			{
				new MySqlParameter("@MValue", model.IsInfoAll),
				new MySqlParameter("@MModifierID", ctx.MUserID),
				new MySqlParameter("@MModifyDate", DateTime.Now),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MIsKey", true)
			};
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = commandText;
			MySqlParameter[] parameters2 = new MySqlParameter[5]
			{
				new MySqlParameter("@MValue", model.IsInfoAll),
				new MySqlParameter("@MModifierID", ctx.MUserID),
				new MySqlParameter("@MModifyDate", DateTime.Now),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MIsKey", false)
			};
			array = (commandInfo2.Parameters = parameters2);
			list.Add(commandInfo2);
			return list;
		}

		public List<CommandInfo> GetInitImportCofigCommandInfos(MContext ctx)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			FPImportTypeConfigModel modelData = new FPImportTypeConfigModel
			{
				MAccountNo = "",
				MPassword = "",
				MOrgID = ctx.MOrgID,
				MType = 1,
				MFPType = 0,
				MImportType = 2,
				MMonthAgo = 1
			};
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPImportTypeConfigModel>(ctx, modelData, null, true));
			FPImportTypeConfigModel modelData2 = new FPImportTypeConfigModel
			{
				MAccountNo = "",
				MPassword = "",
				MOrgID = ctx.MOrgID,
				MType = 1,
				MFPType = 1,
				MImportType = 2,
				MMonthAgo = 1
			};
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPImportTypeConfigModel>(ctx, modelData2, null, true));
			FPImportTypeConfigModel modelData3 = new FPImportTypeConfigModel
			{
				MAccountNo = "",
				MPassword = "",
				MOrgID = ctx.MOrgID,
				MType = 0,
				MFPType = 0,
				MImportType = 2,
				MMonthAgo = 1
			};
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPImportTypeConfigModel>(ctx, modelData3, null, true));
			return list;
		}

		public List<IOConfigModel> GetConfigList(MContext ctx, int importType)
		{
			SqlWhere filter = new SqlWhere().Equal("MTypeID", importType);
			return ModelInfoManager.GetDataModelList<IOConfigModel>(ctx, filter, false, false);
		}

		public List<FPConfigSettingModel> GetConfigSettingList(MContext ctx, int importType)
		{
			SqlWhere filter = new SqlWhere().Equal("MType", importType);
			return ModelInfoManager.GetDataModelList<FPConfigSettingModel>(ctx, filter, false, false);
		}
	}
}
