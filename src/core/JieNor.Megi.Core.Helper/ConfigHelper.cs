using JieNor.Megi.Common.Redis;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.DisLock;
using JieNor.Megi.Core.Repository;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;

namespace JieNor.Megi.Core.Helper
{
	public class ConfigHelper
	{
		public void RegisterSysConfig()
		{
			RedisClientManager.Register(new RedisConfig
			{
				WriteServer = ConfigurationManager.AppSettings["RedisConfig.WriteServer"],
				ReadServer = ConfigurationManager.AppSettings["RedisConfig.ReadServer"],
				KeyPrefix = ConfigurationManager.AppSettings["RedisConfig.KeyPrefix"]
			});
			DisLockManager.Register(GetDisLockConfig());
		}

		private DisLockConfig GetDisLockConfig()
		{
			DisLockConfig disLockConfig = new DisLockConfig();
			if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["DisLockConfig.SyncLockEnable"]))
			{
				disLockConfig.SyncLockEnable = Convert.ToBoolean(ConfigurationManager.AppSettings["DisLockConfig.SyncLockEnable"]);
			}
			if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["DisLockConfig.DefaultReleaseMin"]))
			{
				disLockConfig.DefaultReleaseMin = Convert.ToInt32(ConfigurationManager.AppSettings["DisLockConfig.DefaultReleaseMin"]);
			}
			if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["DisLockConfig.RetryInterval"]))
			{
				disLockConfig.RetryInterval = Convert.ToInt32(ConfigurationManager.AppSettings["DisLockConfig.RetryInterval"]);
			}
			if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["DisLockConfig.RetryCount"]))
			{
				disLockConfig.RetryCount = Convert.ToInt32(ConfigurationManager.AppSettings["DisLockConfig.RetryCount"]);
			}
			if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["DisLockConfig.HystrixInterval"]))
			{
				disLockConfig.HystrixInterval = Convert.ToInt32(ConfigurationManager.AppSettings["DisLockConfig.HystrixInterval"]);
			}
			if (disLockConfig.SyncLockEnable)
			{
				try
				{
					string text = "select * from t_sys_lockoperation";
					DataSet dataSet = DbHelperMySQL.Query(text.ToString());
					if (dataSet.Tables[0].Rows.Count > 0)
					{
						List<LockOperation> list2 = disLockConfig.LockOperations = ModelInfoManager.DataTableToList<LockOperation>(dataSet.Tables[0]);
					}
					else
					{
						disLockConfig.SyncLockEnable = false;
					}
				}
				catch
				{
					disLockConfig.SyncLockEnable = false;
				}
			}
			return disLockConfig;
		}

		private T GetConfigFromDB<T>()
		{
			Type typeFromHandle = typeof(T);
			object obj = Activator.CreateInstance(typeFromHandle);
			string name = typeFromHandle.Name;
			PropertyInfo[] properties = typeFromHandle.GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				string singleSectionValue = GetSingleSectionValue(name, propertyInfo.Name);
				if (propertyInfo.CanWrite && !string.IsNullOrEmpty(singleSectionValue))
				{
					try
					{
						propertyInfo.SetValue(obj, singleSectionValue, null);
					}
					catch (Exception)
					{
					}
				}
			}
			return (T)obj;
		}

		private string GetSingleSectionValue(string configTypeName, string fieldName)
		{
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MitemID", configTypeName + "." + fieldName)
			};
			object single = DbHelperMySQL.GetSingle("select MValue from t_sys_config where MitemID=@MitemID", cmdParms);
			if (single == null)
			{
				return null;
			}
			return single.ToString();
		}
	}
}
