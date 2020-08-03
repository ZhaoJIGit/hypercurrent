using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace JieNor.Megi.DataRepository.PT
{
	public class PTUtility
	{
		private string _tableName = string.Empty;

		public PTUtility(string tableName)
		{
			_tableName = tableName;
		}

		public OperationResult Sort(MContext ctx, string ids)
		{
			OperationResult operationResult = new OperationResult();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<CommandInfo> list = new List<CommandInfo>();
			int num = 0;
			string commandText = $"update {_tableName} set MSeq=@MSeq where MItemID=@MItemID and MOrgID = @MOrgID and MIsDelete = 0 ";
			try
			{
				string[] array = ids.Split(',');
				foreach (string value in array)
				{
					MySqlParameter[] parameters = new MySqlParameter[3]
					{
						new MySqlParameter("@MSeq", num),
						new MySqlParameter("@MItemID", value),
						new MySqlParameter("@MOrgID", ctx.MOrgID)
					};
					List<CommandInfo> list2 = list;
					CommandInfo obj = new CommandInfo
					{
						CommandText = commandText
					};
					DbParameter[] array2 = obj.Parameters = parameters;
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

		public OperationResult Delete<T>(MContext ctx, string pkID) where T : BDModel
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetDeleteFlagCmd<T>(ctx, pkID));
				list.AddRange((IEnumerable<CommandInfo>)RPTReportRepository.GetDelReportLayoutCmds(ctx, new List<string>
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
			string text = string.Format("select Count(*) from {0} a \r\n                            left join {0}_l b \r\n                            on a.MItemID=b.MParentID and MLocaleID=@MLocaleID and a.MOrgID = b.MOrgID and b.MIsDelete = 0 \r\n                            WHERE a.MIsDelete=0 and a.MOrgID=@MOrgID AND b.MName=@MName ", _tableName);
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
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PrintTemplateExist", "模板名称已存在！");
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
