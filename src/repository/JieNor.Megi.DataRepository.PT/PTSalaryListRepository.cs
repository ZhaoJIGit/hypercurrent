using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.PT
{
	public class PTSalaryListRepository : DataServiceT<PAPrintSettingModel>
	{
		public List<PAPrintSettingModel> GetList(MContext ctx)
		{
			List<PAPrintSettingModel> dataModelList = ModelInfoManager.GetDataModelList<PAPrintSettingModel>(ctx, new SqlWhere(), false, true);
			return (from item in dataModelList
			orderby item.MSeq, item.MCreateDate
			select item).ToList();
		}

		public OperationResult Sort(MContext ctx, string ids)
		{
			OperationResult operationResult = new OperationResult();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<CommandInfo> list = new List<CommandInfo>();
			int num = 0;
			string commandText = "update t_pa_printsetting set MSeq=@MSeq where MItemID=@MItemID AND MIsDelete=0 ";
			try
			{
				string[] array = ids.Split(',');
				foreach (string value in array)
				{
					MySqlParameter[] array2 = new MySqlParameter[2]
					{
						new MySqlParameter("@MSeq", MySqlDbType.Int32),
						new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
					};
					array2[0].Value = num;
					array2[1].Value = value;
					List<CommandInfo> list2 = list;
					CommandInfo obj = new CommandInfo
					{
						CommandText = commandText
					};
					DbParameter[] array3 = obj.Parameters = array2;
					list2.Add(obj);
					num++;
				}
				dynamicDbHelperMySQL.ExecuteSqlTran(list);
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

		public override OperationResult Delete(MContext ctx, string pkID)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<PAPrintSettingModel>(ctx, pkID));
				list.AddRange(RPTReportRepository.GetDelReportLayoutCmds(ctx, new List<string>
				{
					pkID
				}));
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				dynamicDbHelperMySQL.ExecuteSqlTran(list);
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

		public OperationResult CheckNameExist(MContext ctx, string itemId, string name)
		{
			OperationResult operationResult = new OperationResult();
			string text = "select Count(*) from t_pa_printsetting a \r\n                            left join t_pa_printsetting_l b \r\n                            on a.MItemID=b.MParentID and MLocaleID=@MLocaleID and b.MIsDelete=0\r\n                            WHERE a.MIsDelete=0 and a.MOrgID=@MOrgID AND b.MName=@MName ";
			if (!string.IsNullOrEmpty(itemId))
			{
				text += " AND a.MItemID <> @MItemID ";
			}
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MName", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			array[2].Value = name;
			array[3].Value = itemId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(text, array);
			if (Convert.ToInt32(single) > 0)
			{
				operationResult.Success = false;
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PrintSettingExist", "A Print Setting with the same name already exists.");
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text2
				});
			}
			return operationResult;
		}

		public OperationResult DeletePrintSetting(MContext ctx, List<string> idList)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<PAPrintSettingModel>(ctx, idList));
				list.AddRange(RPTReportRepository.GetDelReportLayoutCmds(ctx, idList));
				int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
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

		public OperationResult CopySettingWithTemplate(MContext ctx, PAPrintSettingModel copyModel, string srcModelId)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPrintSettingModel>(ctx, copyModel, null, true));
				List<RPTReportLayoutModel> dataModelList = ModelInfoManager.GetDataModelList<RPTReportLayoutModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MPrintSettingID", srcModelId).Equal("MIsDelete", 0), false, false);
				if (dataModelList.Any())
				{
					RPTReportLayoutModel rPTReportLayoutModel = dataModelList[0];
					rPTReportLayoutModel.MID = string.Empty;
					rPTReportLayoutModel.MPrintSettingID = copyModel.MItemID;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<RPTReportLayoutModel>(ctx, rPTReportLayoutModel, null, true));
				}
				int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
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
	}
}
