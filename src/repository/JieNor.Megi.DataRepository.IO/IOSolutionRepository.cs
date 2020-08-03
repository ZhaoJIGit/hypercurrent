using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.IO
{
	public class IOSolutionRepository : DataServiceT<IOSolutionModel>
	{
		public OperationResult ImportData(MContext ctx, List<BaseModel> modelList)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.InsertOrUpdate<BaseModel>(ctx, modelList, (List<string>)null);
		}

		public List<IOSolutionModel> GetSolutionList(MContext ctx, ImportTypeEnum importType)
		{
			return ModelInfoManager.GetDataModelList<IOSolutionModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MTypeID", (int)importType), false, false);
		}

		public OperationResult UpdateSolution(MContext ctx, IOSolutionModel model)
		{
			model.MOrgID = ctx.MOrgID;
			return base.InsertOrUpdate(ctx, model, null);
		}

		public IOSolutionModel GetSolutionModel(MContext ctx, string solutionId)
		{
			if (solutionId == 999999.ToString())
			{
				return GetHanTianSolutionModel(ctx);
			}
			string sql = "select a.MItemID,a.MOrgID,MTypeID,MHeaderRowIndex,MDataRowIndex,a.MName\n                            from T_IO_Solution a\n                            left join T_IO_Solution_l l\n                            on a.MItemID=l.MParentID and l.MLocaleID=@MLocaleID and l.MIsDelete=0 \n                            where a.MItemID=@MItemID AND a.MOrgID=@MOrgID and a.MIsDelete=0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MItemID", solutionId),
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			IOSolutionModel dataModel = ModelInfoManager.GetDataModel<IOSolutionModel>(ctx, sql, cmdParms);
			if (!string.IsNullOrEmpty(dataModel.MItemID))
			{
				dataModel.MConfig = GetSolutionConfigList(ctx, dataModel.MItemID, dataModel.MTypeID);
			}
			return dataModel;
		}

		public List<IOSolutionConfigModel> GetSolutionConfigList(MContext ctx, string solutionId, int typeId)
		{
			string sql = $"SELECT a.MItemID AS MConfigID, a.MName AS MConfigStandardName, b.MName AS MConfigName, a.MIsRequired,a.MIsDataRequired,a.MIsKey,a.MExpression,a.MDataType,\r\n                            c.MItemID, '{ctx.MOrgID}' AS MOrgID, c.MTypeID,c.MParentID,c.MConfigID, c.MColumnName FROM T_IO_Config a\r\n                            LEFT JOIN T_IO_Config_L b on a.MItemID=b.MParentID and b.MLocaleID=@MLocaleID and b.MIsDelete=0 \n                            LEFT JOIN T_IO_SolutionConfig c ON a.MItemID=c.MConfigID AND c.MParentID=@MSolutionID and c.MIsDelete=0 \n                            WHERE a.MTypeID=@MTypeID and a.MIsDelete=0\r\n                            ORDER BY MSequence, MConfigName";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MSolutionID", solutionId),
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MTypeID", typeId)
			};
			return ModelInfoManager.GetDataModelBySql<IOSolutionConfigModel>(ctx, sql, cmdParms);
		}

		public List<IOSolutionConfigModel> GetHangTianConfigList(MContext ctx)
		{
			string sql = string.Format(" SELECT a.MItemID AS MConfigID, a.MName AS MConfigStandardName, b.MName AS MConfigName, a.MIsRequired,a.MIsDataRequired,a.MIsKey,a.MExpression,a.MDataType,\r\n                            '{1}' AS MItemID, '{0}' AS MOrgID FROM T_IO_Config a\r\n                            LEFT JOIN T_IO_Config_L b on a.MItemID=b.MParentID and b.MLocaleID=@MLocaleID and b.MIsDelete=0 \r\n                            WHERE a.MTypeID=@MTypeID and a.MIsDelete=0\r\n                            ORDER BY MSequence, MConfigName ", ctx.MOrgID, 999999.ToString());
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MTypeID", ImportTypeEnum.OutFaPiao)
			};
			return ModelInfoManager.GetDataModelBySql<IOSolutionConfigModel>(ctx, sql, cmdParms);
		}

		public IOSolutionModel GetHanTianSolutionModel(MContext ctx)
		{
			string sql = " SELECT MItemID,MOrgID,MTypeID,MHeaderRowIndex,MDataRowIndex,MName\n                            FROM T_IO_Solution \n                            WHERE MItemID='999999'  AND MIsDelete=0 ";
			IOSolutionModel dataModel = ModelInfoManager.GetDataModel<IOSolutionModel>(ctx, sql, new MySqlParameter[0]);
			dataModel.MOrgID = ctx.MOrgID;
			int num = 999999;
			List<IOSolutionConfigModel> list = dataModel.MConfig = GetSolutionConfigList(ctx, num.ToString(), 8);
			return dataModel;
		}

		public List<IOConfigModel> GetConfigList(MContext ctx, int importType)
		{
			SqlWhere filter = new SqlWhere().Equal("MTypeID", importType);
			return ModelInfoManager.GetDataModelList<IOConfigModel>(ctx, filter, false, false);
		}

		public OperationResult CheckNameExist(MContext ctx, int type, string id, string name)
		{
			OperationResult operationResult = new OperationResult();
			string text = "select Count(*) from t_io_solution a \r\n                            WHERE MIsDelete=0 and MOrgID=@MOrgID AND MTypeID=@MTypeID AND a.MName=@MName ";
			if (!string.IsNullOrEmpty(id))
			{
				text += " AND a.MItemID <> @MItemID ";
			}
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MTypeID", type),
				new MySqlParameter("@MName", name),
				new MySqlParameter("@MItemID", id)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(text, cmdParms);
			if (Convert.ToInt32(single) > 0)
			{
				operationResult.Success = false;
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ImportTemplateExist", "An import template with the same name already exists.");
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text2
				});
			}
			return operationResult;
		}
	}
}
