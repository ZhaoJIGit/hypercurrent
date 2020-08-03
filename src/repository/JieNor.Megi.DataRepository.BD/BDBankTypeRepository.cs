using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDBankTypeRepository : DataServiceT<BDBankTypeModel>
	{
		private string multLangFieldSql = "\r\n            ,t2.MName{0} ";

		public string CommonSelect = "SELECT \n                t1.MItemID, \n                t1.MItemID AS MBankTypeID, \n                t2.MName, \n                t1.MIsSys, \n                t1.MModifyDate\n                #_#lang_field0#_#    \n            FROM\n                T_BD_BankType t1\n                    INNER JOIN\n                @_@t_bd_banktype_l@_@ t2 ON t1.MItemID = t2.MParentID\n                    AND t2.MIsDelete = 0\n            WHERE\n                (t1.MIsSys = 1 OR t1.MOrgID = @MOrgID)\n                AND t1.MIsDelete = 0";

		public DataGridJson<BDBankTypeModel> Get(MContext ctx, GetParam param)
		{
			bool multiLang = ctx.MultiLang;
			return new APIDataRepository().Get<BDBankTypeModel>(ctx, param, CommonSelect, multLangFieldSql, false, true, null);
		}

		public List<BDBankTypeViewModel> GetBDBankTypeList(MContext ctx, GetParam param = null)
		{
			string str = "select a.MItemID,b.MName,b.MLocaleID , MIsSys from T_BD_BankType a\r\n                        INNER JOIN T_BD_BankType_L b ON (a.MIsSys=1 OR a.MOrgID = b.MOrgID) AND  a.MItemID = b.MParentID AND b.MLocaleID = @MLocaleID AND b.MIsDelete = 0 \r\n                        WHERE (a.MIsSys=1 OR a.MOrgID=@MOrgID)  AND  a.MIsDelete=0 AND b.MName is not null ";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MModifyDate", param?.ModifiedSince ?? DateTime.MinValue),
				new MySqlParameter("@MItemID", (param == null) ? "" : param.ElementID)
			};
			if (param != null)
			{
				if (param.ModifiedSince > DateTime.MinValue)
				{
					str += " AND a.MModifyDate>@MModifyDate ";
				}
				if (!string.IsNullOrWhiteSpace(param.ElementID))
				{
					str += " AND a.MItemID=@MItemID ";
				}
			}
			str += " ORDER BY MIsSys DESC,MSeq ASC ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(str.ToString(), cmdParms);
			if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
			{
				return ModelInfoManager.DataTableToList<BDBankTypeViewModel>(dataSet.Tables[0]);
			}
			return new List<BDBankTypeViewModel>();
		}

		public BDBankTypeEditModel GetBDBankTypeEditModel(MContext ctx, BDBankTypeEditModel banktype)
		{
			if (string.IsNullOrWhiteSpace(banktype.MItemID))
			{
				return ModelInfoManager.GetEmptyDataEditModel<BDBankTypeEditModel>(ctx);
			}
			return ModelInfoManager.GetDataEditModel<BDBankTypeEditModel>(ctx, banktype.MItemID, false, true);
		}

		public OperationResult SaveBankType(MContext ctx, BDBankTypeEditModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			model.MOrgID = ctx.MOrgID;
			model.IsNew = string.IsNullOrWhiteSpace(model.MItemID);
			BDBankTypeEditModel modelData = CopyModel2EditModel(ctx, model);
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankTypeEditModel>(ctx, modelData, null, true));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			return new OperationResult
			{
				Success = false,
				Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ErrorSaveFailed", "Error, save failed!")
			};
		}

		private BDBankTypeEditModel CopyModel2EditModel(MContext ctx, BDBankTypeModel model)
		{
			return new BDBankTypeEditModel
			{
				MItemID = model.MItemID,
				MOrgID = model.MOrgID,
				MNumber = model.MNumber,
				MultiLanguage = model.MultiLanguage
			};
		}

		public OperationResult DeleteBankType(MContext ctx, BDBankTypeModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDBankTypeEditModel>(ctx, model.MItemID));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			return new OperationResult
			{
				Success = false,
				Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ErrorDeleteFailed", "Error, delete failed!")
			};
		}
	}
}
