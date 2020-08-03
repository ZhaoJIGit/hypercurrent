using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.PT;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.PT
{
	public class PTVoucherRepository : DataServiceT<PTVoucherModel>
	{
		private string _tableName = string.Empty;

		public string TableName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_tableName))
				{
					_tableName = new PTVoucherModel().TableName;
				}
				return _tableName;
			}
		}

		public List<PTVoucherModel> GetList(MContext ctx)
		{
			string sql = "select a.*,b.MName\n                from t_pt_voucher a\n                left join t_pt_voucher_l b on a.MItemID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MOrgID = a.MOrgID and b.MIsDelete = 0 \n                where a.MOrgID=@MOrgID and  a.MIsDelete = 0 \r\n                order by a.MIsDefault desc,a.MSeq,a.MCreateDate";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			return ModelInfoManager.GetDataModelBySql<PTVoucherModel>(ctx, sql, array);
		}

		public PTVoucherModel GetModel(MContext ctx, string itemID)
		{
			string sql = "select a.*,b.MName\n                from t_pt_voucher a\n                left join t_pt_voucher_l b on a.MItemID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MOrgID = a.MOrgID and b.MIsDelete = 0 \n                where a.MOrgID=@MOrgID and a.MItemID=@MItemID and  a.MIsDelete = 0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MItemID", itemID),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			List<PTVoucherModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<PTVoucherModel>(ctx, sql, cmdParms);
			if (dataModelBySql.Count > 0)
			{
				return dataModelBySql[0];
			}
			return null;
		}

		public OperationResult Sort(MContext ctx, string ids)
		{
			PTUtility pTUtility = new PTUtility(TableName);
			return pTUtility.Sort(ctx, ids);
		}

		public override OperationResult Delete(MContext ctx, string pkID)
		{
			PTUtility pTUtility = new PTUtility(TableName);
			return pTUtility.Delete<PTVoucherModel>(ctx, pkID);
		}

		public OperationResult CheckNameExist(MContext ctx, string itemId, string name)
		{
			PTUtility pTUtility = new PTUtility(TableName);
			return pTUtility.CheckNameExist(ctx, itemId, name);
		}
	}
}
