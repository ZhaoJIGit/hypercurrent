using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.MI
{
	public class MigrateLogRepository
	{
		public static List<T> GetMigrateLogList<T>(MContext ctx, MigrateTypeEnum type)
		{
			string sql = string.Empty;
			switch (type)
			{
			case MigrateTypeEnum.BasicData:
				sql = "select *,'' as MMegiCode from t_mi_log where MOrgID=@MOrgID and MIsDelete=0 and MType<>1\r\n                                union all \r\n                            select a.*, b.MCurrencyID as MMegiCode from (select *from t_mi_log where MOrgID=@MOrgID and MIsDelete=0 and MType=1) a\r\n                                left join t_reg_currency b on a.MMegiID=b.MItemID and b.MOrgID=@MOrgID and b.MIsDelete=0";
				break;
			case MigrateTypeEnum.Currency:
				sql = "select a.*, concat(b.MCurrencyID, ' ', c.MName) as MMegiName, b.MCurrencyID as MMegiCode \r\n                                from t_mi_log a \r\n                                inner join t_reg_currency b on a.MMegiID=b.MItemID and b.MOrgID=a.MOrgID and b.MIsDelete=0\r\n                                inner join t_bas_currency_l c on b.MCurrencyID=c.MParentID and c.MIsDelete=0 and c.MLocaleID='0x7804'\r\n                                where a.MType=@MType and a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                group by a.MItemID";
				break;
			case MigrateTypeEnum.Account:
				sql = "select t1.*,t2.MSourceCheckType,t2.MSourceCurrency, t2.MMatchNumber, t2.MNewNumber, t2.MCheckType, t3.MIsCheckForCurrency \r\n                                from (select * from t_mi_log where MType=@MType and MOrgID=@MOrgID and MIsDelete=0) t1 \r\n                                inner join t_bd_accountmatchlog t2 on t2.MSourceBillKey=2 and t1.MMegiID=t2.MMegiID and t2.MOrgID=t1.MOrgID and t2.MIsDelete=0\r\n                                inner join t_bd_account t3 on t3.MOrgID=t1.MOrgID and t3.MItemID=t1.MMegiID and t3.MIsDelete=0\r\n                                group by t1.MItemID\r\n                                order by t2.MNumber asc";
				break;
			case MigrateTypeEnum.Contact:
				sql = string.Format("select a.*, convert(AES_DECRYPT(c.MName,'{0}') using utf8) as MMegiName, (case when b.MIsCustomer=1 then '客户' when b.MIsSupplier=1 then '供应商' else '其它' end) as MMegiCode \r\n                                from t_mi_log a \r\n                                inner join t_bd_contacts b on a.MMegiID=b.MItemID and b.MOrgID=a.MOrgID and b.MIsDelete=0\r\n                                inner join t_bd_contacts_l c on b.MItemID=c.MParentID and c.MIsDelete=0 and c.MLocaleID='0x7804'\r\n                                where a.MType=@MType and a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                group by a.MItemID", "JieNor-001");
				break;
			case MigrateTypeEnum.Employee:
				sql = "select a.*, c.MLastName as MMegiCode, c.MFirstName as MMegiName \r\n                                from t_mi_log a \r\n                                inner join t_bd_employees b on a.MMegiID=b.MItemID and b.MOrgID=a.MOrgID and b.MIsDelete=0\r\n                                inner join t_bd_employees_l c on b.MItemID=c.MParentID and c.MIsDelete=0 and c.MLocaleID='0x7804'\r\n                                where a.MType=@MType and a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                group by a.MItemID";
				break;
			case MigrateTypeEnum.Item:
				sql = "select a.*, b.MNumber as MMegiCode, c.MDesc as MMegiName \r\n                                from t_mi_log a \r\n                                inner join t_bd_item b on a.MMegiID=b.MItemID and b.MOrgID=a.MOrgID and b.MIsDelete=0\r\n                                inner join t_bd_item_l c on b.MItemID=c.MParentID and c.MIsDelete=0 and c.MLocaleID='0x7804'\r\n                                where a.MType=@MType and a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                group by a.MItemID";
				break;
			case MigrateTypeEnum.Tracking:
				sql = "select t.*,(select MName from t_bd_track_l where MParentID = (select MMegiID from t_mi_log where MItemID = t.MParentID) and MLocaleID = '0x7804') as MMegiCode,\n                                    (select MName from t_bd_TrackEntry_L where MParentID = t.MMegiID and MLocaleID = '0x7804') as MMegiName \n                                from t_mi_log t\n                                where MType = @MType and ifnull(MParentID, '') <> '' and MOrgID = @MOrgID and MIsDelete = 0";
				break;
			}
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MType", (int)type)
			};
			return ModelInfoManager.GetDataModelBySql<T>(ctx, sql, cmdParms);
		}
	}
}
