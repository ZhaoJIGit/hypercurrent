using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDEmailTemplateRepository : DataServiceT<BDEmailTemplateModel>
	{
		public List<BDEmailTemplateModel> GetList(MContext ctx, EmailSendTypeEnum emailType)
		{
			string arg = string.Format(" and MIsSys={0}", (emailType == EmailSendTypeEnum.Payslip) ? "0" : "1");
			string sql = $"select * from (select a.MItemID,a.MOrgID,MType,MIsSys,MSeq,MName,MSubject,MContent\r\n                from T_BD_EmailTemplate a\r\n                left join T_BD_EmailTemplate_l l\r\n                on a.MItemID=l.MParentID and l.MLocaleID=@MLocaleID and l.MIsDelete = 0 \r\n                where a.MOrgID='0'  and a.MIsDelete=0 and MType=@MType {arg}\r\n                union all\r\n                select a.MItemID,a.MOrgID,MType,MIsSys,MSeq,MName,MSubject,MContent\r\n                from T_BD_EmailTemplate a\r\n                left join T_BD_EmailTemplate_l l\r\n                on a.MItemID=l.MParentID and l.MLocaleID=@MLocaleID  and l.MIsDelete = 0 \r\n                where a.MOrgID=@MOrgID  and a.MIsDelete=0 and MType=@MType and MIsSys=0) u\r\n                order by MType,MSeq";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			array[2].Value = emailType.ToString();
			return ModelInfoManager.GetDataModelBySql<BDEmailTemplateModel>(ctx, sql, array);
		}

		public BDEmailTemplateModel GetModel(MContext ctx, string itemID)
		{
			string sql = "a.MItemID,a.MOrgID,MType,MIsSys,MSeq,MName,MSubject,MContent\r\n                from T_BD_EmailTemplate a\r\n                left join T_BD_EmailTemplate_l l\r\n                on a.MItemID=l.MParentID and l.MLocaleID=@MLocaleID and l.MIsDelete = 0 \r\n                where a.MOrgID=@MOrgID and a.MItemID=@MItemID and a.MIsDelete = 0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MItemID", itemID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			List<BDEmailTemplateModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDEmailTemplateModel>(ctx, sql, cmdParms);
			if (dataModelBySql.Count > 0)
			{
				return dataModelBySql[0];
			}
			return null;
		}

		public override OperationResult Delete(MContext ctx, string pkID)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				MySqlParameter[] array = new MySqlParameter[1]
				{
					new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
				};
				array[0].Value = pkID;
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				dynamicDbHelperMySQL.ExecuteSql("update T_BD_EmailTemplate set MIsDelete=1 where MItemID=@MItemID and MOrgID = @MOrgID", ctx.GetParameters(array));
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

		public OperationResult CheckNameExist(MContext ctx, string type, string itemId, string name)
		{
			OperationResult operationResult = new OperationResult();
			string text = "select Count(*) from T_BD_EmailTemplate a \r\n                            left join T_BD_EmailTemplate_l b \r\n                            on a.MItemID=b.MParentID and MLocaleID=@MLocaleID  and b.MIsDelete=0\r\n                            WHERE a.MIsDelete=0 and a.MOrgID=@MOrgID AND b.MName=@MName and MType=@MType ";
			if (!string.IsNullOrEmpty(itemId))
			{
				text += " AND a.MItemID <> @MItemID ";
			}
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MName", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			array[2].Value = name;
			array[3].Value = itemId;
			array[4].Value = type;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(text, array);
			if (Convert.ToInt32(single) > 0)
			{
				operationResult.Success = false;
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "EmailTemplateExist", "A Email Template with the same name already exists.");
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
