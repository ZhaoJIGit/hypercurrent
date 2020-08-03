using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLInitBalanceRepository : DataServiceT<GLInitBalanceModel>
	{
		private readonly BDAccountRepository accountDal = new BDAccountRepository();

		public List<GLInitBalanceModel> GetCurrentAccountInitBalance(MContext ctx, List<string> currentAccountIds)
		{
			string sql = "select * from t_gl_initbalance where MOrgID = @MOrgID and MIsDelete = 0 and MCheckGroupValueID != '0' and MAccountID in ('" + string.Join("','", currentAccountIds) + "')";
			return ModelInfoManager.GetDataModelBySql<GLInitBalanceModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
		}

		public List<GLInitBalanceModel> GetInitBalanceListIncludeCheckGroupValue(MContext ctx, SqlWhere filter)
		{
			string text = " select * from (SELECT \r\n                            t1.MItemID,\r\n                            t1.MAccountID,\r\n                            t1.MCurrencyID,\r\n                            t1.MCheckGroupValueID,\r\n                            t1.MInitBalance,\r\n                            t1.MInitBalanceFor,\r\n                            t1.MYtdDebit,\r\n                            t1.MYtdDebitFor,\r\n                            t1.MYtdCredit,\r\n                            t1.MYtdCreditFor,\r\n                            t1.MBillID,\r\n                            t1.MContactType,\r\n                            if(t5.MID is null , IF(t6.MID is null , IF(t7.MID is null , IF(t8.MID is null , '' , t8.MType) ,t7.MType) , t6.MType) , t5.MType) as MBillType,\r\n                            if(t6.MID is not null , t6.MBankID , IF(t7.MID is not null , t7.MBankID , '')) as MBankID,\r\n                            if(t5.MID is null , IF(t6.MID is null , IF(t7.MID is null , IF(t8.MID is null , '' , 'Employees') ,t7.MContactType) , t6.MContactType) , IF(LOCATE('Invoice_Sale' ,t5.MType) <> 0 ,'Customer','Supplier')) as MContactTypeFromBill,\r\n                            t2.MCheckGroupID,\r\n                            t2.MDC,\r\n                            t3.MContactID as MContact,\r\n                            t3.MEmployeeID as MEmployee,\r\n                            t3.MMerItemID as MMerItem,\r\n                            t3.MExpItemID as MExpItem,\r\n                            t3.MPaItemID as MPaItem,\r\n                            t3.MTrackItem1 as MTrack1,\r\n                            t3.MTrackItem2 as MTrack2,\r\n                            t3.MTrackItem3 as MTrack3,\r\n                            t3.MTrackItem4 as MTrack4,\r\n                            t3.MTrackItem5 as MTrack5,\r\n                            t4.MContactID,\r\n                            t4.MEmployeeID, \r\n                            t4.MMerItemID,\r\n                            t4.MExpItemID,\r\n                            t4.MPaItemID,\r\n                            t4.MTrackItem1,\r\n                            t4.MTrackItem2,\r\n                            t4.MTrackItem3,\r\n                            t4.MTrackItem4,\r\n                            t4.MTrackItem5\r\n                        FROM\r\n                            t_gl_initbalance t1\r\n                                INNER JOIN\r\n                            t_bd_account t2 ON t1.MOrgID = t2.MOrgID\r\n                                AND t1.MAccountId = t2.MItemID\r\n                                AND t1.MisDelete = t2.MIsDelete \r\n                                inner JOIN\r\n                            t_gl_checkgroup t3 ON t3.MItemID = t2.MCheckGroupID\r\n                                AND t3.MIsDelete = t1.MIsDelete \r\n                                left JOIN\r\n                            t_gl_checkgroupvalue t4 ON t4.MOrgID = t1.MOrgID\r\n                                AND t4.MItemID = t1.MCheckGroupValueID\r\n                                AND t4.MIsDelete = t1.MIsDelete \r\n                                left JOIN\r\n                            t_iv_invoice t5 on t5.MOrgID = t1.MOrgID\r\n                                AND t5.MID = t1.MBillID\r\n                                AND t5.MIsDelete = t1.MIsDelete \r\n                                left JOIN\r\n                            t_iv_receive t6 on t6.MOrgID = t1.MOrgID\r\n                                AND t6.MID = t1.MBillID\r\n                                AND t6.MIsDelete = t1.MIsDelete \r\n                                left JOIN\r\n                            t_iv_payment t7 on t7.MOrgID = t1.MOrgID\r\n                                AND t7.MID = t1.MBillID\r\n                                AND t7.MIsDelete = t1.MIsDelete \r\n                                left JOIN\r\n                            t_iv_expense t8 on t8.MOrgID = t1.MOrgID\r\n                                AND t8.MID = t1.MBillID\r\n                                AND t8.MIsDelete = t1.MIsDelete \r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID\r\n                                AND t1.MIsDelete = 0 ) t";
			MySqlParameter[] array = ctx.GetParameters((MySqlParameter)null);
			if (filter != null)
			{
				text += filter.WhereSqlString;
				text += filter.OrderBySqlString;
				array = array.Concat(filter.Parameters ?? new MySqlParameter[0]).ToArray();
			}
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(text, array);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				return BindDataset2InitBalance(ctx, dataSet.Tables[0]);
			}
			return new List<GLInitBalanceModel>();
		}

		public List<GLInitBalanceModel> GetCompleteInitBalanceList(MContext ctx, GLInitBalanceListFilterModel filter)
		{
			string str = " select * from (SELECT \r\n                            t1.MNumber,\r\n                            t17.MName,\r\n                            t1.MCheckGroupID,\r\n                            t1.MDC,\r\n                            t1.MCreateInitBill,\r\n                            t2.MItemID,\r\n                            t2.MAccountID,\r\n                            t2.MCurrencyID,\r\n                            t2.MCheckGroupValueID,\r\n                            t2.MInitBalance,\r\n                            t2.MInitBalanceFor,\r\n                            t2.MYtdDebit,\r\n                            t2.MYtdDebitFor,\r\n                            t2.MYtdCredit,\r\n                            t2.MYtdCreditFor,\r\n                            t2.MBillID,\r\n                            t2.MContactType,\r\n                            t3.MContactID as MContact,\r\n                            t3.MEmployeeID as MEmployee,\r\n                            t3.MMerItemID as MMerItem,\r\n                            t3.MExpItemID as MExpItem,\r\n                            t3.MPaItemID as MPaItem,\r\n                            t3.MTrackItem1 as MTrack1,\r\n                            t3.MTrackItem2 as MTrack2,\r\n                            t3.MTrackItem3 as MTrack3,\r\n                            t3.MTrackItem4 as MTrack4,\r\n                            t3.MTrackItem5 as MTrack5,\r\n                            t4.MContactID,\r\n                            t4.MEmployeeID, \r\n                            t4.MMerItemID,\r\n                            t4.MExpItemID,\r\n                            t4.MPaItemID,\r\n                            t4.MTrackItem1,\r\n                            t4.MTrackItem2,\r\n                            t4.MTrackItem3,\r\n                            t4.MTrackItem4,\r\n                            t4.MTrackItem5 ";
			str += string.Format(" ,convert(AES_DECRYPT(t5.MName,'{0}') using utf8) as MContactName , F_GetUserName(t6.MFirstName,t6.MLastName) as MEmployeeName , \r\n                              t7.MName as MExpItemName ,CONCAT(t16.MNumber ,':', t8.MDesc) as MMerItemName , t9.MName as MPaItemGroupName, t10.MName as MPaItemName,\r\n                              t11.MName as MTrackItem1Name , t12.MName as MTrackItem2Name ,t13.MName as MTrackItem3Name ,t14.MName as MTrackItem4Name ,t15.MName as MTrackItem5Name ", "JieNor-001");
			str += " FROM\r\n                            t_bd_account  t1\r\n                            INNER JOIN t_bd_account_l t17 ON t1.MOrgID = t17.MOrgID\r\n                                AND t1.MItemID = t17.MParentID\r\n                                AND t1.MIsDelete = t17.MIsDelete\r\n                                AND t17.MLocaleID = @MLocaleID And t1.MIsActive=1\r\n                                Left JOIN\r\n                            t_gl_initbalance t2 ON t1.MOrgID = t2.MOrgID\r\n                                AND t2.MAccountID = t1.MItemID\r\n                                AND t2.MisDelete = t1.MIsDelete ";
			if (!filter.IncludeInitBalance)
			{
				str += " AND t2.MOrgID=-1 ";
			}
			str += " inner JOIN\r\n                            t_gl_checkgroup t3 ON t3.MItemID = t1.MCheckGroupID\r\n                                AND t3.MIsDelete = t1.MIsDelete \r\n                                left JOIN\r\n                            t_gl_checkgroupvalue t4 ON t4.MOrgID = t1.MOrgID\r\n                                AND t4.MItemID = t2.MCheckGroupValueID\r\n                                AND t4.MIsDelete = t1.MIsDelete \r\n                           LEFT JOIN t_bd_contacts_l t5 on t5.MParentID = t4.MContactID and t5.MLocaleID=@MLocaleID and t5.MOrgID = t4.MOrgID and t5.MIsDelete=0\r\n                           LEFT JOIN t_bd_employees_l t6 on t6.MParentID = t4.MEmployeeID and t6.MLocaleID=@MLocaleID and t6.MOrgID = t4.MOrgID and t6.MIsDelete=0\r\n                           LEFT JOIN t_bd_expenseitem_l t7 on t7.MParentID = t4.MExpItemID and t7.MLocaleID=@MLocaleID and t7.MOrgID = t4.MOrgID and t7.MIsDelete=0 \r\n                           LEFT JOIN t_bd_item t16 on t16.mitemid = t4.MMerItemID and t16.MOrgID = t4.MOrgID and t16.MIsDelete=0 \r\n                           LEFT JOIN t_bd_item_l t8 on t8.MParentID = t4.MMerItemID and t8.MLocaleID=@MLocaleID and t8.MOrgID = t16.MOrgID and t8.MIsDelete=0 \r\n                           LEFT JOIN t_pa_payitemgroup_l t9 on t9.MParentID = t4.MPaItemID and t9.MLocaleID=@MLocaleID and t9.MOrgID = t4.MOrgID and t9.MIsDelete=0 \r\n                           LEFT JOIN t_pa_payitem_l t10 on t10.MParentID = t4.MPaItemID and t10.MLocaleID=@MLocaleID and t10.MOrgID = t4.MOrgID and t10.MIsDelete=0  \r\n                           LEFT JOIN t_bd_trackentry_l t11 on t11.MParentID = t4.MTrackItem1 and t11.MLocaleID=@MLocaleID and t11.MOrgID = t4.MOrgID and t11.MIsDelete=0 \r\n                           LEFT JOIN t_bd_trackentry_l t12 on t12.MParentID = t4.MTrackItem2 and t12.MLocaleID=@MLocaleID and t12.MOrgID = t4.MOrgID and t12.MIsDelete=0\r\n                           LEFT JOIN t_bd_trackentry_l t13 on t13.MParentID = t4.MTrackItem3 and t13.MLocaleID=@MLocaleID and t13.MOrgID = t4.MOrgID and t13.MIsDelete=0\r\n                           LEFT JOIN t_bd_trackentry_l t14 on t14.MParentID = t4.MTrackItem4 and t14.MLocaleID=@MLocaleID and t14.MOrgID = t4.MOrgID and t14.MIsDelete=0\r\n                           LEFT JOIN t_bd_trackentry_l t15 on t15.MParentID = t4.MTrackItem5 and t15.MLocaleID=@MLocaleID and t15.MOrgID = t4.MOrgID and t15.MIsDelete=0 \r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID\r\n                                AND t1.MIsDelete = 0 ORDER BY t1.MNumber ) t";
			MySqlParameter[] array = ctx.GetParameters((MySqlParameter)null);
			if (filter != null)
			{
				str += filter.WhereSqlString;
				str += filter.OrderBySqlString;
				array = array.Concat(filter.Parameters ?? new MySqlParameter[0]).ToArray();
			}
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(str, array);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				return BindDataset2InitBalance(ctx, dataSet.Tables[0]);
			}
			return new List<GLInitBalanceModel>();
		}

		public List<GLInitBalanceModel> BindDataset2InitBalance(MContext ctx, DataTable dt)
		{
			List<GLInitBalanceModel> list = new List<GLInitBalanceModel>();
			DataColumnCollection columns = dt.Columns;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow row = dt.Rows[i];
				GLInitBalanceModel gLInitBalanceModel = new GLInitBalanceModel
				{
					MOrgID = ctx.MOrgID,
					MItemID = row.MField<string>("MItemID"),
					MAccountID = row.MField<string>("MAccountID"),
					MCurrencyID = row.MField<string>("MCurrencyID"),
					MCheckGroupValueID = row.MField<string>("MCheckGroupValueID"),
					MInitBalance = row.MField<decimal>("MInitBalance"),
					MInitBalanceFor = row.MField<decimal>("MInitBalanceFor"),
					MYtdDebit = row.MField<decimal>("MYtdDebit"),
					MYtdDebitFor = row.MField<decimal>("MYtdDebitFor"),
					MYtdCredit = row.MField<decimal>("MYtdCredit"),
					MYtdCreditFor = row.MField<decimal>("MYtdCreditFor"),
					MDC = row.MField<int>("MDC")
				};
				if (columns.Contains("MBillID"))
				{
					gLInitBalanceModel.MBillID = row.MField<string>("MBillID");
				}
				if (columns.Contains("MBillType"))
				{
					gLInitBalanceModel.MBillType = row.MField<string>("MBillType");
				}
				if (columns.Contains("MBankID"))
				{
					gLInitBalanceModel.MBankID = row.MField<string>("MBankID");
				}
				if (columns.Contains("MContactTypeFromBill"))
				{
					gLInitBalanceModel.MContactTypeFromBill = row.MField<string>("MContactTypeFromBill");
				}
				if (columns.Contains("MContactType"))
				{
					gLInitBalanceModel.MContactType = row.MField<string>("MContactType");
				}
				BDAccountModel bDAccountModel = new BDAccountModel
				{
					MItemID = gLInitBalanceModel.MAccountID,
					MCheckGroupID = row.MField<string>("MCheckGroupID"),
					MDC = row.MField<int>("MDC")
				};
				if (columns.Contains("MNumber"))
				{
					bDAccountModel.MNumber = row.MField<string>("MNumber");
				}
				if (columns.Contains("MName"))
				{
					bDAccountModel.MName = row.MField<string>("MName");
				}
				if (columns.Contains("MCreateInitBill"))
				{
					bDAccountModel.MCreateInitBill = row.MField<bool>("MCreateInitBill");
				}
				GLCheckGroupModel obj = new GLCheckGroupModel
				{
					MContactID = row.MField<int>("MContact"),
					MEmployeeID = row.MField<int>("MEmployee"),
					MMerItemID = row.MField<int>("MMerItem"),
					MExpItemID = row.MField<int>("MExpItem"),
					MPaItemID = row.MField<int>("MPaItem"),
					MTrackItem1 = row.MField<int>("MTrack1"),
					MTrackItem2 = row.MField<int>("MTrack2"),
					MTrackItem3 = row.MField<int>("MTrack3"),
					MTrackItem4 = row.MField<int>("MTrack4"),
					MTrackItem5 = row.MField<int>("MTrack5")
				};
				GLCheckGroupModel gLCheckGroupModel2 = bDAccountModel.MCheckGroupModel = obj;
				GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel
				{
					MItemID = row.MField<string>("MCheckGroupValueID"),
					MContactID = row.MField<string>("MContactID"),
					MEmployeeID = row.MField<string>("MEmployeeID"),
					MMerItemID = row.MField<string>("MMerItemID"),
					MExpItemID = row.MField<string>("MExpItemID"),
					MPaItemID = row.MField<string>("MPaItemID"),
					MTrackItem1 = row.MField<string>("MTrackItem1"),
					MTrackItem2 = row.MField<string>("MTrackItem2"),
					MTrackItem3 = row.MField<string>("MTrackItem3"),
					MTrackItem4 = row.MField<string>("MTrackItem4"),
					MTrackItem5 = row.MField<string>("MTrackItem5")
				};
				if (columns.Contains("MContactName"))
				{
					gLCheckGroupValueModel.MContactName = row.Field<string>("MContactName");
				}
				if (columns.Contains("MEmployeeName"))
				{
					gLCheckGroupValueModel.MEmployeeName = row.Field<string>("MEmployeeName");
				}
				if (columns.Contains("MMerItemName"))
				{
					gLCheckGroupValueModel.MMerItemName = row.Field<string>("MMerItemName");
				}
				if (columns.Contains("MExpItemName"))
				{
					gLCheckGroupValueModel.MExpItemName = row.Field<string>("MExpItemName");
				}
				if (columns.Contains("MPaItemGroupName"))
				{
					gLCheckGroupValueModel.MPaItemGroupName = row.Field<string>("MPaItemGroupName");
				}
				if (columns.Contains("MPaItemName"))
				{
					gLCheckGroupValueModel.MPaItemName = row.Field<string>("MPaItemName");
				}
				if (columns.Contains("MTrackItem1Name"))
				{
					gLCheckGroupValueModel.MTrackItem1Name = row.Field<string>("MTrackItem1Name");
				}
				if (columns.Contains("MTrackItem2Name"))
				{
					gLCheckGroupValueModel.MTrackItem2Name = row.Field<string>("MTrackItem2Name");
				}
				if (columns.Contains("MTrackItem3Name"))
				{
					gLCheckGroupValueModel.MTrackItem3Name = row.Field<string>("MTrackItem3Name");
				}
				if (columns.Contains("MTrackItem4Name"))
				{
					gLCheckGroupValueModel.MTrackItem4Name = row.Field<string>("MTrackItem4Name");
				}
				if (columns.Contains("MTrackItem5Name"))
				{
					gLCheckGroupValueModel.MTrackItem5Name = row.Field<string>("MTrackItem5Name");
				}
				gLInitBalanceModel.MAccountModel = bDAccountModel;
				gLInitBalanceModel.MCheckGroupValueModel = gLCheckGroupValueModel;
				list.Add(gLInitBalanceModel);
			}
			return list;
		}

		public GLInitBalanceModel GetInitBalanceModel(MContext ctx, SqlWhere filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT a.* , b.MDC from t_gl_initbalance a ");
			stringBuilder.AppendLine(" INNER JOIN t_bd_account b on a.MACCOUNTID = b.MItemID and a.MOrgID = b.MOrgID and a.MOrgID=@MOrgID and a.MIsDelete = 0 ");
			if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36));
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] array = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			array[0].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.GetFirstOrDefaultModel<GLInitBalanceModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public List<GLInitBalanceModel> GetInitBalanceModels(MContext ctx, SqlWhere filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT a.* , b.MDC, b.MCode as MAccountCode from t_gl_initbalance a ");
			stringBuilder.AppendLine(" INNER JOIN t_bd_account b on a.MACCOUNTID = b.MItemID and a.MOrgID=b.MOrgID and b.MIsDelete = 0 \r\n                   where a.MOrgID = @MOrgID and a.MIsDelete = 0  ");
			if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(Regex.Replace(filter.WhereSqlString, "where", "and", RegexOptions.IgnoreCase));
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MOrgID", ctx.MOrgID));
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] cmdParms = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<GLInitBalanceModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms).Tables[0]);
		}

		public int UpdateInitBalanceByTran(MContext ctx, List<CommandInfo> cmdInfos)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.ExecuteSqlTran(cmdInfos);
		}

		public GLInitBalanceModel GetBankInitBalance(MContext ctx, string accountId, string bankName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT c.MItemID, a.MItemID as MAccountID,c.MOrgID,a.MCyID as MCURRENCYID,c.MInitBalance ,c.MInitBalanceFor,al.MName from t_bd_bankaccount a");
			stringBuilder.AppendLine("INNER JOIN t_bd_bankaccount_l al on a1.MOrgID = a.MOrgID and al.MIsDelete = 0  and  al.MParentID = a.MItemID and al.MLocaleID=@MLCID");
			stringBuilder.AppendLine("LEFT JOIN t_gl_initbalance c on c.MOrgID = a.MOrgID and  c.MACCOUNTID = a.MItemID and a.MOrgID=@MOrgID and a.MItemID=@accountId and c.MIsDelete = 0  ");
			stringBuilder.AppendLine("where  MName=@bankName and a.MOrgID = @MOrgID and a.MIsDelete = 0  ");
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@accountId", MySqlDbType.VarChar, 36),
				new MySqlParameter("@bankName", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = accountId;
			array[3].Value = bankName;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				return ModelInfoManager.DataTableToList<GLInitBalanceModel>(dataSet.Tables[0])[0];
			}
			return new GLInitBalanceModel();
		}

		private int GetBalanceCoefficient(int billType, string accountCode)
		{
			int result = 1;
			switch (billType)
			{
			case 1:
				if (accountCode.IndexOf("1122") == 0)
				{
					result = 1;
				}
				else if (accountCode.IndexOf("2203") == 0)
				{
					result = -1;
				}
				break;
			case 3:
				if (accountCode.IndexOf("1122") == 0)
				{
					result = -1;
				}
				else if (accountCode.IndexOf("2203") == 0)
				{
					result = 1;
				}
				break;
			case 2:
				if (accountCode.IndexOf("2202") == 0)
				{
					result = 1;
				}
				else if (accountCode.IndexOf("1123") == 0)
				{
					result = -1;
				}
				break;
			case 4:
				if (accountCode.IndexOf("2202") == 0)
				{
					result = -1;
				}
				else if (accountCode.IndexOf("1123") == 0)
				{
					result = 1;
				}
				break;
			}
			return result;
		}

		public List<CommandInfo> UpdateAccountBalanceByRecevie(MContext ctx, IVReceiveModel recevie)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BDContactsRepository bDContactsRepository = new BDContactsRepository();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			BDContactsModel dataModel = bDContactsRepository.GetDataModel(ctx, recevie.MContactID, false);
			string accountCode;
			if (string.IsNullOrWhiteSpace(dataModel.MCCurrentAccountCode))
			{
				BDAccountModel leafAccountByCode = bDAccountRepository.GetLeafAccountByCode(ctx, baseBDAccountList, "1122");
				accountCode = leafAccountByCode.MCode;
			}
			else
			{
				accountCode = dataModel.MCCurrentAccountCode;
			}
			BDAccountModel bDAccountModel = (from x in baseBDAccountList
			where x.MCode == accountCode
			select x).FirstOrDefault();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MOrgID", ctx.MOrgID);
			sqlWhere.Equal("MAccountID", bDAccountModel.MItemID);
			sqlWhere.Equal("MContactID", recevie.MContactID);
			sqlWhere.Equal("MCurrencyID", recevie.MCyID);
			GLInitBalanceModel gLInitBalanceModel = GetDataModelByFilter(ctx, sqlWhere);
			bool flag = true;
			if (gLInitBalanceModel == null)
			{
				gLInitBalanceModel = new GLInitBalanceModel();
				gLInitBalanceModel.MOrgID = ctx.MOrgID;
				gLInitBalanceModel.MAccountID = bDAccountModel.MItemID;
				gLInitBalanceModel.MCurrencyID = recevie.MCyID;
				gLInitBalanceModel.MContactID = recevie.MContactID;
				flag = false;
			}
			int balanceCoefficient = GetBalanceCoefficient(3, accountCode);
			if (!recevie.IsUpdate)
			{
				GLInitBalanceModel gLInitBalanceModel2 = gLInitBalanceModel;
				gLInitBalanceModel2.MInitBalance += recevie.MTaxTotalAmt * (decimal)balanceCoefficient;
				GLInitBalanceModel gLInitBalanceModel3 = gLInitBalanceModel;
				gLInitBalanceModel3.MInitBalanceFor += recevie.MTaxTotalAmtFor * (decimal)balanceCoefficient;
			}
			else
			{
				IVReceiveModel receiveEditModel = IVReceiveRepository.GetReceiveEditModel(ctx, recevie.MID);
				bool flag2 = receiveEditModel != null && receiveEditModel.MCyID != recevie.MCyID;
				if (flag2)
				{
					SqlWhere sqlWhere2 = new SqlWhere();
					sqlWhere2.Equal("MOrgID", ctx.MOrgID);
					sqlWhere2.Equal("MAccountID", bDAccountModel.MItemID);
					sqlWhere2.Equal("MContactID", receiveEditModel.MContactID);
					sqlWhere2.Equal("MCurrencyID", receiveEditModel.MCyID);
					GLInitBalanceModel dataModelByFilter = GetDataModelByFilter(ctx, sqlWhere2);
					if (dataModelByFilter != null)
					{
						GLInitBalanceModel gLInitBalanceModel4 = dataModelByFilter;
						gLInitBalanceModel4.MInitBalance -= receiveEditModel.MTaxTotalAmt * (decimal)balanceCoefficient;
						GLInitBalanceModel gLInitBalanceModel5 = dataModelByFilter;
						gLInitBalanceModel5.MInitBalanceFor -= receiveEditModel.MTaxTotalAmtFor * (decimal)balanceCoefficient;
						List<CommandInfo> updateInitBalanceCmds = BDAccountRepository.GetUpdateInitBalanceCmds(ctx, dataModelByFilter, baseBDAccountList);
						list.AddRange(updateInitBalanceCmds);
					}
				}
				if ((receiveEditModel != null & flag) && !flag2)
				{
					GLInitBalanceModel gLInitBalanceModel6 = gLInitBalanceModel;
					gLInitBalanceModel6.MInitBalance -= receiveEditModel.MTaxTotalAmt * (decimal)balanceCoefficient;
					GLInitBalanceModel gLInitBalanceModel7 = gLInitBalanceModel;
					gLInitBalanceModel7.MInitBalanceFor -= receiveEditModel.MTaxTotalAmtFor * (decimal)balanceCoefficient;
				}
				GLInitBalanceModel gLInitBalanceModel8 = gLInitBalanceModel;
				gLInitBalanceModel8.MInitBalance += recevie.MTaxTotalAmt * (decimal)balanceCoefficient;
				GLInitBalanceModel gLInitBalanceModel9 = gLInitBalanceModel;
				gLInitBalanceModel9.MInitBalanceFor += recevie.MTaxTotalAmtFor * (decimal)balanceCoefficient;
			}
			list.AddRange(BDAccountRepository.GetUpdateInitBalanceCmds(ctx, gLInitBalanceModel, baseBDAccountList));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return list = new List<CommandInfo>();
		}

		public List<CommandInfo> UpdateAccountBalanceByPayment(MContext ctx, IVPaymentModel payment, bool isDelete = false)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BDContactsRepository bDContactsRepository = new BDContactsRepository();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			string accountCode = "";
			if (payment.MContactType == "Supplier")
			{
				BDContactsModel dataModel = bDContactsRepository.GetDataModel(ctx, payment.MContactID, false);
				if (string.IsNullOrWhiteSpace(dataModel.MCCurrentAccountCode))
				{
					BDAccountModel leafAccountByCode = bDAccountRepository.GetLeafAccountByCode(ctx, baseBDAccountList, "2202");
					accountCode = leafAccountByCode.MCode;
				}
				else
				{
					accountCode = dataModel.MCCurrentAccountCode;
				}
			}
			else
			{
				BDEmployeesRepository bDEmployeesRepository = new BDEmployeesRepository();
				BDEmployeesModel dataModel2 = bDEmployeesRepository.GetDataModel(ctx, payment.MContactID, false);
				accountCode = (string.IsNullOrWhiteSpace(dataModel2.MCurrentAccountCode) ? "1221" : dataModel2.MCurrentAccountCode);
			}
			BDAccountModel bDAccountModel = (from x in baseBDAccountList
			where x.MCode == accountCode
			select x).FirstOrDefault();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MOrgID", ctx.MOrgID);
			sqlWhere.Equal("MAccountID", bDAccountModel.MItemID);
			if (payment.MContactType == "Employees")
			{
				sqlWhere.Equal("MContactID", "0");
			}
			else
			{
				sqlWhere.Equal("MContactID", payment.MContactID);
			}
			sqlWhere.Equal("MCurrencyID", payment.MCyID);
			GLInitBalanceModel gLInitBalanceModel = GetDataModelByFilter(ctx, sqlWhere);
			bool flag = true;
			if (gLInitBalanceModel == null)
			{
				gLInitBalanceModel = new GLInitBalanceModel();
				gLInitBalanceModel.MOrgID = ctx.MOrgID;
				gLInitBalanceModel.MAccountID = bDAccountModel.MItemID;
				gLInitBalanceModel.MCurrencyID = payment.MCyID;
				gLInitBalanceModel.MContactID = payment.MContactID;
				flag = false;
			}
			if (payment.MContactType == "Employees")
			{
				gLInitBalanceModel.MContactID = "0";
			}
			int balanceCoefficient = GetBalanceCoefficient(4, accountCode);
			if (!payment.IsUpdate)
			{
				GLInitBalanceModel gLInitBalanceModel2 = gLInitBalanceModel;
				gLInitBalanceModel2.MInitBalance += payment.MTaxTotalAmt * (decimal)balanceCoefficient;
				GLInitBalanceModel gLInitBalanceModel3 = gLInitBalanceModel;
				gLInitBalanceModel3.MInitBalanceFor += payment.MTaxTotalAmtFor * (decimal)balanceCoefficient;
			}
			else
			{
				IVPaymentModel paymentEditModel = IVPaymentRepository.GetPaymentEditModel(ctx, payment.MID);
				bool flag2 = paymentEditModel != null && paymentEditModel.MCyID != payment.MCyID;
				if (flag2)
				{
					SqlWhere sqlWhere2 = new SqlWhere();
					sqlWhere2.Equal("MOrgID", ctx.MOrgID);
					sqlWhere2.Equal("MAccountID", bDAccountModel.MItemID);
					sqlWhere2.Equal("MContactID", paymentEditModel.MContactID);
					sqlWhere2.Equal("MCurrencyID", paymentEditModel.MCyID);
					GLInitBalanceModel dataModelByFilter = GetDataModelByFilter(ctx, sqlWhere2);
					if (dataModelByFilter != null)
					{
						GLInitBalanceModel gLInitBalanceModel4 = dataModelByFilter;
						gLInitBalanceModel4.MInitBalance -= paymentEditModel.MTaxTotalAmt * (decimal)balanceCoefficient;
						GLInitBalanceModel gLInitBalanceModel5 = dataModelByFilter;
						gLInitBalanceModel5.MInitBalanceFor -= paymentEditModel.MTaxTotalAmtFor * (decimal)balanceCoefficient;
						List<CommandInfo> updateInitBalanceCmds = BDAccountRepository.GetUpdateInitBalanceCmds(ctx, dataModelByFilter, baseBDAccountList);
						list.AddRange(updateInitBalanceCmds);
					}
				}
				if ((paymentEditModel != null & flag) && !flag2)
				{
					GLInitBalanceModel gLInitBalanceModel6 = gLInitBalanceModel;
					gLInitBalanceModel6.MInitBalance -= paymentEditModel.MTaxTotalAmt * (decimal)balanceCoefficient;
					GLInitBalanceModel gLInitBalanceModel7 = gLInitBalanceModel;
					gLInitBalanceModel7.MInitBalanceFor -= paymentEditModel.MTaxTotalAmtFor * (decimal)balanceCoefficient;
				}
				GLInitBalanceModel gLInitBalanceModel8 = gLInitBalanceModel;
				gLInitBalanceModel8.MInitBalance += payment.MTaxTotalAmt * (decimal)balanceCoefficient;
				GLInitBalanceModel gLInitBalanceModel9 = gLInitBalanceModel;
				gLInitBalanceModel9.MInitBalanceFor += payment.MTaxTotalAmtFor * (decimal)balanceCoefficient;
			}
			list.AddRange(BDAccountRepository.GetUpdateInitBalanceCmds(ctx, gLInitBalanceModel, baseBDAccountList));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return list = new List<CommandInfo>();
		}

		private string GetBillMapAccountId(MContext ctx, string contactId, int type)
		{
			List<BDAccountModel> baseBDAccountList = accountDal.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			string accountCode = "";
			switch (type)
			{
			case 1:
			{
				BDContactsRepository bDContactsRepository = new BDContactsRepository();
				BDContactsModel dataModel2 = bDContactsRepository.GetDataModel(ctx, contactId, false);
				if (string.IsNullOrWhiteSpace(dataModel2.MCCurrentAccountCode))
				{
					BDAccountModel leafAccountByCode = accountDal.GetLeafAccountByCode(ctx, baseBDAccountList, "1122");
					accountCode = leafAccountByCode.MCode;
				}
				else
				{
					accountCode = dataModel2.MCCurrentAccountCode;
				}
				break;
			}
			case 2:
			{
				BDContactsRepository bDContactsRepository2 = new BDContactsRepository();
				BDContactsModel dataModel3 = bDContactsRepository2.GetDataModel(ctx, contactId, false);
				if (string.IsNullOrWhiteSpace(dataModel3.MCCurrentAccountCode))
				{
					BDAccountModel leafAccountByCode2 = accountDal.GetLeafAccountByCode(ctx, baseBDAccountList, "2202");
					accountCode = leafAccountByCode2.MCode;
				}
				else
				{
					accountCode = dataModel3.MCCurrentAccountCode;
				}
				break;
			}
			case 3:
			{
				BDEmployeesRepository bDEmployeesRepository = new BDEmployeesRepository();
				BDEmployeesModel dataModel = bDEmployeesRepository.GetDataModel(ctx, contactId, false);
				accountCode = (string.IsNullOrWhiteSpace(dataModel.MCurrentAccountCode) ? "1221" : dataModel.MCurrentAccountCode);
				break;
			}
			}
			BDAccountModel bDAccountModel = (from x in baseBDAccountList
			where x.MCode == accountCode
			select x).First();
			return bDAccountModel.MItemID;
		}

		public GLInitBalanceModel GetDeleteInitBalance(MContext ctx, List<GLInitBalanceModel> updateList, string accountId, string currencyId, string contactId)
		{
			GLInitBalanceModel gLInitBalanceModel = (from x in updateList
			where x.MOrgID == ctx.MOrgID && x.MAccountID == accountId && x.MContactID == contactId && x.MCurrencyID == currencyId
			select x).FirstOrDefault();
			if (gLInitBalanceModel == null)
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MAccountID", accountId);
				sqlWhere.Equal("MOrgID", ctx.MOrgID);
				sqlWhere.Equal("MContactID", contactId);
				sqlWhere.Equal("MCurrencyID", currencyId);
				sqlWhere.Equal("MIsDelete", 0);
				gLInitBalanceModel = GetDataModelByFilter(ctx, sqlWhere);
			}
			if (gLInitBalanceModel == null)
			{
				return null;
			}
			return gLInitBalanceModel;
		}

		public OperationResult DeleteInitBalance(MContext ctx)
		{
			string sql = $"update t_gl_initbalance set MIsDelete = 1 where MOrgID='{ctx.MOrgID}' and MIsDelete = 0 ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSql(sql);
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public OperationResult DeleteInitBalanceByFilter(MContext ctx, SqlWhere filter)
		{
			CommandInfo deleteInitBalanceCmds = GetDeleteInitBalanceCmds(ctx, filter);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(new List<CommandInfo>
			{
				deleteInitBalanceCmds
			});
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public CommandInfo GetDeleteInitBalanceCmds(MContext ctx, SqlWhere filter)
		{
			CommandInfo commandInfo = new CommandInfo();
			string commandText = $"update t_gl_initbalance set MIsDelete = 1 where MOrgID='{ctx.MOrgID}' and MIsDelete = 0 ";
			if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
			{
				commandText = $"update t_gl_initbalance set MIsDelete = 1 {filter.WhereSqlString} and MOrgID='{ctx.MOrgID}' and MIsDelete = 0 ";
				DbParameter[] array = commandInfo.Parameters = filter.Parameters;
			}
			commandInfo.CommandText = commandText;
			return commandInfo;
		}

		public GLInitBalanceModel ConvertToGLInitBalanceModel(MContext ctx, BDBankBalanceModel bankBalanceModel, string accountId, string cyId, string contactId = null)
		{
			GLInitBalanceModel gLInitBalanceModel = new GLInitBalanceModel();
			gLInitBalanceModel.IsNew = true;
			gLInitBalanceModel.MInitBalance = (bankBalanceModel?.MTotalAmt ?? decimal.Zero);
			gLInitBalanceModel.MInitBalanceFor = (bankBalanceModel?.MTotalAmtFor ?? decimal.Zero);
			gLInitBalanceModel.MContactID = (string.IsNullOrWhiteSpace(contactId) ? "0" : contactId);
			gLInitBalanceModel.MAccountID = accountId;
			gLInitBalanceModel.MOrgID = ctx.MOrgID;
			gLInitBalanceModel.MCurrencyID = cyId;
			return gLInitBalanceModel;
		}
	}
}
