using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDContactsTypeRepository : DataServiceT<BDContactsTypeModel>
	{
		private List<string> multLangFieldSqls = new List<string>
		{
			"\r\n            ,t7.MContacts_MName{0} as MContacts_MName{0}\r\n            ,t2.MName{0} ",
			",CONVERT( AES_DECRYPT(t5.MName{0}, 'JieNor-001') USING UTF8) as MContacts_MName{0} ",
			",t5.MName{0} as MContacts_MName{0} "
		};

		private string CommonSelect = string.Format("SELECT\r\n                t1.MItemID,\r\n                t1.MItemID AS MContactGroupID,\r\n                t1.MModifyDate,\r\n                t2.MName,\r\n                t7.MContacts_MContactID,\r\n                t7.MContacts_MName,\r\n                t7.MContacts_MModifyDate\r\n                #_#lang_field0#_#\r\n            FROM\r\n                T_BD_ContactsType t1\r\n                    INNER JOIN\r\n                @_@t_bd_contactstype_l@_@ t2 ON t2.MOrgID = t1.MOrgID\r\n                    AND t2.MParentID = t1.MItemID\r\n                    AND t2.MIsDelete = 0\r\n                    LEFT JOIN\r\n                (SELECT\r\n                  t6.MTypeID,\r\n                    t4.MItemID as MContacts_MContactID,\r\n                    t5.MName as MContacts_MName,\r\n                    t4.MModifyDate as MContacts_MModifyDate,\r\n                    t4.morgid\r\n                    #_#lang_field1#_#\r\n                FROM\r\n                    t_bd_contacts t4\r\n                INNER JOIN @_@t_bd_contacts_l@_@ t5 ON t5.MOrgID = t4.MOrgID\r\n                    AND t5.MParentID = t4.MItemID\r\n                    AND t5.MIsDelete = 0\r\n                LEFT JOIN t_bd_contactstypelink t6 ON t6.MOrgID = t4.MOrgID\r\n                    AND t6.MContactID = t4.MItemID\r\n                    AND t6.MIsDelete = 0\r\n                WHERE\r\n                    t4.MOrgID = @MOrgID\r\n                        AND t4.MIsDelete = 0) t7 ON t7.morgid=t1.morgid AND t7.MTypeID=t1.MItemID\r\n            WHERE\r\n                (t1.MOrgID = '0'\r\n                    OR t1.MOrgID = @MOrgID)\r\n                    AND t1.MIsDelete = 0\r\n                    AND t1.MItemID not in ('1', '2','3','4') ", "JieNor-001");

		public DataGridJson<BDContactsTypeModel> Get(MContext ctx, GetParam param)
		{
			DataGridJson<BDContactsTypeModel> dataGridJson = new APIDataRepository().Get<BDContactsTypeModel>(ctx, param, CommonSelect, multLangFieldSqls, false, true, null);
			foreach (BDContactsTypeModel row in dataGridJson.rows)
			{
				row.MContacts = ((from a in row.MContacts
				orderby a.MModifyDate
				select a).ToList() ?? new List<BDContactsSimpleModel>());
			}
			return dataGridJson;
		}

		public List<BDContactsTypeModel> GetContactsTypeModels(MContext ctx, GetParam param)
		{
			string text = " SELECT * FROM T_BD_ContactsType t1 INNER JOIN T_BD_ContactsType_l t2  ON t1.MItemID=t2.MParentID AND t1.MOrgID = t2.MOrgID  AND t2.MLocaleID=@MLocaleID AND t2.MIsDelete = 0   WHERE t1.MIsDelete = 0  And t1.MIsSys=0 And t1.MOrgID=@MOrgID ";
			if (param.ModifiedSince > DateTime.MinValue)
			{
				text += " AND t1.MModifyDate>@MModifyDate ";
			}
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MModifyDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = param.ModifiedSince;
			return ModelInfoManager.GetDataModelBySql<BDContactsTypeModel>(ctx, text, array);
		}

		public List<BDContactsTypeModel> GetContactsTypeModelsById(MContext ctx, string id)
		{
			string sql = " SELECT * FROM T_BD_ContactsType t1 INNER JOIN T_BD_ContactsType_l t2  ON t1.MItemID=t2.MParentID AND t1.MOrgID = t2.MOrgID  AND t2.MLocaleID=@MLocaleID AND t2.MIsDelete = 0   WHERE t1.MIsDelete = 0  And t1.MIsSys=0 And t1.MOrgID=@MOrgID AND t1.MItemId=@MItemId ";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MItemId", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = id;
			array[1].Value = ctx.MOrgID;
			array[2].Value = ctx.MLCID;
			return ModelInfoManager.GetDataModelBySql<BDContactsTypeModel>(ctx, sql, array);
		}
	}
}
