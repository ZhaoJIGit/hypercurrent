using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Repository;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace JieNor.Megi.Core.DBUtility
{
	public class DynamicPubConstant
	{
		private static List<OrgAppStorageModel> storageLst = new List<OrgAppStorageModel>();

		private static object _lock = new object();

		public static string ConnectionString(string orgId)
		{
			if (storageLst == null || storageLst.Count == 0)
			{
				storageLst = GetModelList();
			}
			OrgAppStorageModel orgAppStorageModel = storageLst.FirstOrDefault((OrgAppStorageModel f) => f.MOrgID.EqualsIgnoreCase(orgId));
			if (orgAppStorageModel == null)
			{
				lock (_lock)
				{
					orgAppStorageModel = GetModelByKey(orgId);
					if (orgAppStorageModel != null && storageLst.Count((OrgAppStorageModel t) => t.MOrgID.EqualsIgnoreCase(orgId)) == 0)
					{
						storageLst.Add(orgAppStorageModel);
					}
				}
			}
			if (orgAppStorageModel == null)
			{
				return PubConstant.ConnectionString;
			}
			string text = ConfigurationManager.AppSettings["ForceConnectionString"];
			return string.IsNullOrWhiteSpace(text) ? ("server=" + orgAppStorageModel.MDBServerName + ";Port=" + orgAppStorageModel.MDBServerPort + ";database=" + orgAppStorageModel.MBDName + ";uid=" + orgAppStorageModel.MUserName + ";pwd=" + orgAppStorageModel.MPassWord + ";Allow Zero Datetime=True;SslMode=None;charset=utf8;pooling=true;Max Pool Size=100") : text;
		}

		private static OrgAppStorageModel GetModelByKey(string orgId)
		{
			string sQLString = "SELECT \n                                t1.MDetailID,\n                                t3.MOrgID,\n                                t2.MDBServerName,\n                                t2.MDBServerPort,\n                                t2.MUserName,\n                                t2.MPassWord,\n                                t2.MBDName,\n                                t2.MConOtherInfo,\n                                t1.MIsDelete,\n                                t1.MCreatorID,\n                                t1.MCreateDate,\n                                t1.MModifierID,\n                                t1.MModifyDate\n                            FROM\n                                T_SYS_OrgAppStorage t1\n                                    JOIN\n                                T_SYS_Storage t2 ON t1.MStorageID = t2.MItemID\n                                    AND t1.MIsActive = 1\n                                    AND t2.MIsDelete = 0\n                                    INNER JOIN\n                                T_SYS_OrgApp t3 ON t1.MOrgAppID = t3.MEntryID\n                                    AND t3.MIsDelete = 0\n                                    AND t3.MIsActive = 1\n                            WHERE\n                                t3.MOrgID = @MOrgID AND t1.MIsActive = 1";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", orgId)
			};
			DataSet dataSet = DbHelperMySQL.Query(sQLString, cmdParms);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<OrgAppStorageModel> list = ModelInfoManager.DataTableToList<OrgAppStorageModel>(dataSet.Tables[0]);
				return list[0];
			}
			return null;
		}

		private static List<OrgAppStorageModel> GetModelList()
		{
			string text = "SELECT \n                                t1.MDetailID,\n                                t3.MOrgID,\n                                t2.MDBServerName,\n                                t2.MDBServerPort,\n                                t2.MUserName,\n                                t2.MPassWord,\n                                t2.MBDName,\n                                t2.MConOtherInfo,\n                                t1.MIsDelete,\n                                t1.MCreatorID,\n                                t1.MCreateDate,\n                                t1.MModifierID,\n                                t1.MModifyDate\n                            FROM\n                                T_SYS_OrgAppStorage t1\n                                    JOIN\n                                T_SYS_Storage t2 ON t1.MStorageID = t2.MItemID\n                                    AND t1.MIsActive = 1\n                                    AND t2.MIsDelete = 0\n                                    INNER JOIN\n                                T_SYS_OrgApp t3 ON t1.MOrgAppID = t3.MEntryID\n                                    AND t3.MIsDelete = 0\n                                    AND t3.MIsActive = 1";
			OrgAppStorageModel orgAppStorageModel = new OrgAppStorageModel();
			DataSet dataSet = DbHelperMySQL.Query(text.ToString());
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				return ModelInfoManager.DataTableToList<OrgAppStorageModel>(dataSet.Tables[0]);
			}
			return new List<OrgAppStorageModel>();
		}
	}
}
