using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;

namespace JieNor.Megi.DataRepository.SYS
{
	public class SYSStorageRepository : DataServiceT<SYSOrgAppModel>
	{
		private int _maxFreeDBCount = 2;

		/// <summary>
		/// 300
		/// </summary>
		private static readonly string _dbOrgCount = ConfigurationManager.AppSettings["DBOrgCount"];

		private static readonly string _dbUpdatePath = ConfigurationManager.AppSettings["UpdatePath"];

		/// <summary>
		/// 获取活动的数据库标识(生产)，如果不存在，即返回默认的数据库标识
		/// </summary>
		/// <returns></returns>
		public string GetActiveStorageID()
		{
			return GetActiveStorageID(1);
		}

		/// <summary>
		/// 获取活动的数据库(生产)
		/// </summary>
		/// <returns></returns>
		public SYSStorageModel GetActiveStorageModel()
		{
			return GetActiveStorageModel(1);
		}

		public string GetDemoActiveStorageID()
		{
			return GetActiveStorageID(2);
		}

		/// <summary>
		/// 根据服务类型（1-生产，2-demo）获取活动的数据库
		/// </summary>
		/// <param name="serverType"></param>
		/// <returns></returns>
		public SYSStorageModel GetActiveStorageModel(int serverType)
		{
			int num = 0;
			int.TryParse(_dbOrgCount, out num);
			if (num <= 0)
			{
				num = 50;
			}
			string sQLString = $"SELECT a.* FROM T_Sys_Storage a \n                                        INNER JOIN  T_Sys_StorageServer b ON a.MServerID=b.MItemID\n                                         WHERE b.MServerType={serverType} AND a.MOrgCount<{num} ORDER BY a.MOrgCount DESC, a.MItemID LIMIT 0,1;";
			DataSet dataSet = DbHelperMySQL.Query(sQLString);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<SYSStorageModel> list = ModelInfoManager.DataTableToList<SYSStorageModel>(dataSet.Tables[0]);
				return list[0];
			}
			return null;
		}

		/// <summary>
		/// 获取活动的数据库标识，如果不存在，即返回默认的数据库标识
		/// </summary>
		/// <param name="serverType"></param>
		/// <returns></returns>
		private string GetActiveStorageID(int serverType)
		{
			SYSStorageModel activeStorageModel = GetActiveStorageModel(serverType);
			return (activeStorageModel == null) ? ConfigurationManager.AppSettings["DBDefaultStorageID"] : activeStorageModel.MItemID;
		}

		public void UpdateOrgCount(string storageId)
		{
			string sQLString = "UPDATE T_Sys_Storage SET MOrgCount=MOrgCount+1 where MItemID=@MItemID";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = storageId;
			DbHelperMySQL.ExecuteSql(new MContext(), sQLString, array);
		}

		public CommandInfo GetUpdateOrgCountCmdInfo(string storageId)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "UPDATE T_Sys_Storage SET MOrgCount=MOrgCount+1 where MItemID=@MItemID";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", storageId)
			};
			return commandInfo;
		}

		public SYSStorageModel GetStorageModel(string storageId)
		{
			string sQLString = "SELECT * FROM T_Sys_Storage a where MItemID=@MItemID";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = storageId;
			DataSet dataSet = DbHelperMySQL.Query(sQLString, array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<SYSStorageModel> list = ModelInfoManager.DataTableToList<SYSStorageModel>(dataSet.Tables[0]);
				return list[0];
			}
			return null;
		}

		public SYSStorageServerModel GetStorageServerModel(string storageServerId)
		{
			string sQLString = "SELECT * FROM T_Sys_StorageServer a where MItemID=@MItemID";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = storageServerId;
			DataSet dataSet = DbHelperMySQL.Query(sQLString, array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<SYSStorageServerModel> list = ModelInfoManager.DataTableToList<SYSStorageServerModel>(dataSet.Tables[0]);
				return list[0];
			}
			return null;
		}

		public List<SYSStorageServerModel> GetDemoStorageServerList()
		{
			string sQLString = "SELECT * FROM T_Sys_StorageServer a where MServerType=2";
			DataSet dataSet = DbHelperMySQL.Query(sQLString);
			return ModelInfoManager.DataTableToList<SYSStorageServerModel>(dataSet.Tables[0]);
		}

		public List<SYSStorageModel> GetDemoStorageList(string serverId)
		{
			string sQLString = "SELECT * FROM T_Sys_Storage a where MServerID=@MServerID";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MServerID", serverId)
			};
			DataSet dataSet = DbHelperMySQL.Query(sQLString, cmdParms);
			return ModelInfoManager.DataTableToList<SYSStorageModel>(dataSet.Tables[0]);
		}

		public string GetServerConnectionString(string storeServerId)
		{
			SYSStorageServerModel storageServerModel = GetStorageServerModel(storeServerId);
			return GetServerConnectionString(storageServerModel);
		}

		public string GetServerConnectionString(string storeServerId, string dbName)
		{
			SYSStorageServerModel storageServerModel = GetStorageServerModel(storeServerId);
			storageServerModel.MStandardDBName = dbName;
			return GetServerConnectionString(storageServerModel);
		}

		public void CreateBasDB()
		{
			SYSStorageServerModel storageServerInfo = GetStorageServerInfo();
			if (storageServerInfo != null)
			{
				int newStorageID = GetNewStorageID();
				string dbName = $"{storageServerInfo.MDBNamePrefix}_{newStorageID}";
				if (CreateSchema(storageServerInfo, dbName) && CopySchema(storageServerInfo, dbName) && AddStorageRecord(storageServerInfo, dbName, newStorageID))
				{
					UpdateSchemaCount(storageServerInfo.MItemID);
				}
			}
		}

		private bool UpdateSchemaCount(string serverId)
		{
			string sQLString = "UPDATE T_Sys_StorageServer SET MDBCount=MDBCount+1 where MItemID=@MItemID";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = serverId;
			int num = DbHelperMySQL.ExecuteSql(new MContext(), sQLString, array);
			return num > 0 && true;
		}

		private bool CopySchema(SYSStorageServerModel serverModel, string dbName)
		{
			try
			{
				string serverConnectionString = GetServerConnectionString(serverModel);
				string sQLString = $"CALL P_BasDB_Copy('{serverModel.MStandardDBName}','{dbName}')";
				DbHelperMySQL.ExecuteNonQuery(new MContext(), sQLString, serverConnectionString);
				return true;
			}
			catch (Exception ex)
			{
				MLogger.Log(ex);
				return false;
			}
		}

		private bool AddStorageRecord(SYSStorageServerModel serverModel, string dbName, int newStoreId)
		{
			string sQLString = "INSERT INTO T_Sys_Storage(MItemID,MServerID,MDBServerName,MDBServerPort,MUserName,MPassWord,MConOtherInfo,MBDName,MIsDelete,MOrgCount,MCreatorID,MCreateDate,MModifierID,MModifyDate)\r\n                           VALUES( @StorageID,@MServerID, @MDBServerName,@MDBServerPort,@MUserName,@MPassWord,'', @DBName,0,0,'',now(),'',now() )";
			MySqlParameter[] cmdParms = new MySqlParameter[7]
			{
				new MySqlParameter("@StorageID", newStoreId.ToString()),
				new MySqlParameter("@MServerID", serverModel.MItemID),
				new MySqlParameter("@MDBServerName", DESEncrypt.Encrypt(serverModel.MDBServerName)),
				new MySqlParameter("@MDBServerPort", DESEncrypt.Encrypt(serverModel.MDBServerPort)),
				new MySqlParameter("@MUserName", DESEncrypt.Encrypt(serverModel.MUserName)),
				new MySqlParameter("@MPassWord", DESEncrypt.Encrypt(serverModel.MPassWord)),
				new MySqlParameter("@DBName", DESEncrypt.Encrypt(dbName))
			};
			int num = DbHelperMySQL.ExecuteSql(new MContext(), sQLString, cmdParms);
			return num > 0 && true;
		}

		private bool CreateSchema(SYSStorageServerModel serverModel, string dbName)
		{
			string serverConnectionString = GetServerConnectionString(serverModel);
			string sQLString = $"CREATE SCHEMA `{dbName}`";
			int num = DbHelperMySQL.ExecuteNonQuery(new MContext(), sQLString, serverConnectionString);
			return num > 0 && true;
		}

		private int GetFreeDBCount()
		{
			string sQLString = "SELECT COUNT(*) from T_Sys_Storage where MOrgCount=0";
			object single = DbHelperMySQL.GetSingle(sQLString);
			return Convert.ToInt32(single);
		}

		private int GetNewStorageID()
		{
			string sQLString = "SELECT MAX(MItemID) FROM T_Sys_Storage  where MItemID like '1%'";
			object single = DbHelperMySQL.GetSingle(sQLString);
			if (single == null)
			{
				return 100001;
			}
			int num = 0;
			int.TryParse(single.ToString(), out num);
			if (num == 0)
			{
				num = 100000;
			}
			return num + 1;
		}

		private SYSStorageServerModel GetStorageServerInfo()
		{
			string sQLString = $"SELECT * FROM T_Sys_StorageServer a\r\n                            WHERE MDBCount<MMaxDBCount \r\n                            AND (SELECT COUNT(*) from T_Sys_Storage b where b.MOrgCount=0 AND b.MServerID=a.MItemID)<={_maxFreeDBCount}\r\n                            ORDER BY MitemID  LIMIT 0,1";
			DataSet dataSet = DbHelperMySQL.Query(sQLString);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<SYSStorageServerModel> list = ModelInfoManager.DataTableToList<SYSStorageServerModel>(dataSet.Tables[0]);
				return list[0];
			}
			return null;
		}

		public string GetServerConnectionString(SYSStorageServerModel serverModel)
		{
			return $"server={serverModel.MDBServerName};Port={serverModel.MDBServerPort};database={serverModel.MStandardDBName};uid={serverModel.MUserName};pwd={serverModel.MPassWord};Allow Zero Datetime=True;charset=utf8;pooling=true;Max Pool Size=100";
		}

		public List<SYSStorageServerModel> GetOrgServerList(MContext ctx, string orgIds = null, bool getConversionDate = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT a.MItemID AS MOrgID,d.MDBServerName,d.MDBServerPort,d.MUserName,d.MPassword,d.MBDName as MStandardDBName{0} FROM t_bas_organisation a \n                            INNER JOIN T_Sys_OrgApp b ON a.MItemID=b.MOrgID\n                            INNER JOIN T_Sys_OrgAppStorage c ON c.MOrgAppID=b.MEntryID\n                            INNER JOIN T_Sys_Storage d ON d.MItemID=c.MStorageID", getConversionDate ? ",e.MConversionDate" : "");
			if (getConversionDate)
			{
				stringBuilder.Append(" left join t_bas_orginitsetting e on e.MOrgID=a.MItemID and e.MIsDelete=0");
			}
			MySqlParameter[] cmdParms = null;
			if (!string.IsNullOrWhiteSpace(orgIds))
			{
				string whereInSql = base.GetWhereInSql(orgIds, ref cmdParms, null);
				stringBuilder.AppendFormat(" where a.MItemID in ({0}) and c.MIsActive=1 ", whereInSql);
			}
			ctx.IsSys = true;
			List<SYSStorageServerModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<SYSStorageServerModel>(ctx, stringBuilder.ToString(), cmdParms);
			foreach (SYSStorageServerModel item in dataModelBySql)
			{
				item.MDBServerName = DESEncrypt.Decrypt(item.MDBServerName);
				item.MDBServerPort = DESEncrypt.Decrypt(item.MDBServerPort);
				item.MUserName = DESEncrypt.Decrypt(item.MUserName);
				item.MPassWord = DESEncrypt.Decrypt(item.MPassWord);
				item.MStandardDBName = DESEncrypt.Decrypt(item.MStandardDBName);
			}
			return dataModelBySql;
		}
	}
}
