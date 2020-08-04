using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Logger;
using JieNor.Megi.Core.Mongo;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.Core.DBUtility
{
	public class DynamicDbHelperMySQL : IDisposable
	{
		private MySqlConnection connection = null;

		private MContext mContext = null;

		private int cmdTimeout = 100;

		private int maxParaTag = 2000;

		private string[] XssParameterNameWhiteList = new string[1]
		{
			"@MReportLayout"
		};

		private bool disposed = false;

		public DynamicDbHelperMySQL(MContext ctx)
		{
			mContext = ctx;
		}

		public DynamicDbHelperMySQL(MContext ctx, int cmdTimeout)
		{
			mContext = ctx;
			this.cmdTimeout = cmdTimeout;
		}

		public bool Exists(string strSql, params MySqlParameter[] cmdParms)
		{
			object single = GetSingle(strSql, cmdParms);
			int num = 0;
			if (!object.Equals(single, null) && !object.Equals(single, DBNull.Value))
			{
				num = int.Parse(single.ToString());
			}
			return num != 0;
		}

		public bool Exists(string connString, string strSql, params MySqlParameter[] cmdParms)
		{
			object single = GetSingle(connString, strSql, cmdParms);
			int num = 0;
			if (!object.Equals(single, null) && !object.Equals(single, DBNull.Value))
			{
				num = int.Parse(single.ToString());
			}
			return num != 0;
		}

		public int ExecuteSql(string sql)
		{
			return ExecuteSqlByTime(sql, cmdTimeout);
		}

		public int ExecuteSql(string sql, string content)
		{
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
				{
					connection.Open();
					MySqlParameter mySqlParameter = new MySqlParameter("@content", SqlDbType.NText);
					mySqlParameter.Value = content;
					mySqlCommand.Parameters.Add(mySqlParameter);
					try
					{
						MongoBDHelper.Monitoring(mContext, new List<string>
						{
							sql
						});
						MSqlLogger.LOG(mContext, mySqlCommand);
						return mySqlCommand.ExecuteNonQuery();
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public int ExecuteSqlByTime(string sql, int cmdTimeout)
		{
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
				{
					try
					{
						connection.Open();
						mySqlCommand.CommandTimeout = cmdTimeout;
						MongoBDHelper.Monitoring(mContext, new List<string>
						{
							sql
						});
						MSqlLogger.LOG(mContext, mySqlCommand);
						return mySqlCommand.ExecuteNonQuery();
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public DataTable ExcecuteProcudure(string procedureName, MySqlParameter[] paramList)
		{
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(procedureName, connection))
				{
					connection.Open();
					mySqlCommand.CommandType = CommandType.StoredProcedure;
					mySqlCommand.CommandTimeout = cmdTimeout;
					for (int i = 0; i < paramList.Length; i++)
					{
						mySqlCommand.Parameters.AddWithValue(paramList[i].ParameterName, paramList[i].Value);
					}
					try
					{
						DataTable dataTable = new DataTable();
						using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand))
						{
							mySqlDataAdapter.Fill(dataTable);
							return dataTable;
						}
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					catch (Exception ex2)
					{
						throw ex2;
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public object GetSingle(string sql)
		{
			return GetSingle(sql, cmdTimeout);
		}

		public object GetSingle(string sql, int cmdTimeout)
		{
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
				{
					try
					{
						connection.Open();
						mySqlCommand.CommandTimeout = cmdTimeout;
						object obj = mySqlCommand.ExecuteScalar();
						return (object.Equals(obj, null) || object.Equals(obj, DBNull.Value)) ? null : obj;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public object GetSingle(string sql, params MySqlParameter[] cmdParms)
		{
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						PrepareCommand(mySqlCommand, connection, null, sql, cmdParms);
						object obj = mySqlCommand.ExecuteScalar();
						mySqlCommand.Parameters.Clear();
						return (object.Equals(obj, null) || object.Equals(obj, DBNull.Value)) ? null : obj;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public object GetSingle(string connString, string sql, params MySqlParameter[] cmdParms)
		{
			using (connection = new MySqlConnection(connString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						PrepareCommand(mySqlCommand, connection, null, sql, cmdParms);
						object obj = mySqlCommand.ExecuteScalar();
						mySqlCommand.Parameters.Clear();
						return (object.Equals(obj, null) || object.Equals(obj, DBNull.Value)) ? null : obj;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public DataSet Query(string sql)
		{
			return Query(sql, cmdTimeout);
		}

		public DataSet Query(string sql, int cmdTimeout)
		{
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, connection))
				{
					try
					{
						DataSet dataSet = new DataSet();
						mySqlDataAdapter.SelectCommand.CommandTimeout = cmdTimeout;
						mySqlDataAdapter.Fill(dataSet, "ds");
						return dataSet;
					}
					catch (MySqlException ex)
					{
						throw new Exception(ex.Message);
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public DataSet Query(string sql, params MySqlParameter[] cmdParms)
		{
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				MySqlCommand mySqlCommand = new MySqlCommand();
				mySqlCommand.CommandTimeout = cmdTimeout;
				PrepareCommand(mySqlCommand, connection, null, sql, cmdParms);
				using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand))
				{
					DataSet dataSet = new DataSet();
					try
					{
						mySqlDataAdapter.Fill(dataSet, "ds");
						mySqlCommand.Parameters.Clear();
						return dataSet;
					}
					catch (MySqlException ex)
					{
						throw new Exception(ex.Message);
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public int ExecuteSql(string sql, params MySqlParameter[] cmdParms)
		{
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						PrepareCommand(mySqlCommand, connection, null, sql, cmdParms);
						int result = mySqlCommand.ExecuteNonQuery();
						MongoBDHelper.Monitoring(mContext, new List<string>
						{
							sql
						});
						MSqlLogger.LOG(mContext, mySqlCommand);
						mySqlCommand.Parameters.Clear();
						return result;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			if (!cmdList.Any())
			{
				return 0;
			}
			bool flag = true;
			List<MySqlCommand> list = new List<MySqlCommand>();
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				connection.Open();
				using (MySqlTransaction mySqlTransaction = connection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					mySqlCommand.CommandTimeout = cmdTimeout;
					try
					{
						int num = 0;
						BatchPrepareCommand(mySqlCommand, connection, mySqlTransaction, cmdList, ref num);
						int num2 = mySqlCommand.ExecuteNonQuery();
						list.Add(mySqlCommand.Clone());
						while (num < cmdList.Count)
						{
							mySqlCommand.Parameters.Clear();
							BatchPrepareCommand(mySqlCommand, connection, mySqlTransaction, cmdList, ref num);
							num2 += mySqlCommand.ExecuteNonQuery();
							list.Add(mySqlCommand.Clone());
						}
						mySqlTransaction.Commit();
						return num2;
					}
					catch (Exception ex)
					{
						MLogger.Log(ex);
						mySqlTransaction.Rollback();
						flag = false;
						throw;
					}
					finally
					{
						if (flag)
						{
							MongoBDHelper.Monitoring(mContext, (from x in cmdList
							select x.CommandText).ToList());
							MSqlLogger.LOG(mContext, cmdList, connection);
						}
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public int ExecuteSqlTran(List<CommandInfo> cmdList, bool useForBatchCmds = false)
		{
			if (!useForBatchCmds)
			{
				return ExecuteSqlTran(cmdList);
			}
			if (!cmdList.Any())
			{
				return 0;
			}
			bool flag = true;
			List<MySqlCommand> list = new List<MySqlCommand>();
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				connection.Open();
				using (MySqlTransaction mySqlTransaction = connection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					mySqlCommand.CommandTimeout = cmdTimeout;
					try
					{
						BatchPrepareCommand(mySqlCommand, connection, mySqlTransaction, cmdList);
						int result = mySqlCommand.ExecuteNonQuery();
						mySqlTransaction.Commit();
						list.Add(mySqlCommand);
						return result;
					}
					catch (Exception ex)
					{
						MLogger.Log(ex);
						mySqlTransaction.Rollback();
						flag = false;
						throw;
					}
					finally
					{
						if (flag)
						{
							MongoBDHelper.Monitoring(mContext, (from x in cmdList
							select x.CommandText).ToList());
							MSqlLogger.LOG(mContext, cmdList, connection);
						}
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		private int ExecuteSqlTranOneByOne(List<CommandInfo> cmdList)
		{
			bool flag = true;
			List<MySqlCommand> list = new List<MySqlCommand>();
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				connection.Open();
				using (MySqlTransaction mySqlTransaction = connection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					mySqlCommand.CommandTimeout = cmdTimeout;
					int num = -1;
					try
					{
						int num2 = 0;
						for (int i = 0; i < cmdList.Count; i++)
						{
							CommandInfo commandInfo = cmdList[i];
							num = i;
							if (commandInfo != null)
							{
								string commandText = commandInfo.CommandText;
								MySqlParameter[] cmdParms = (MySqlParameter[])commandInfo.Parameters;
								PrepareCommand(mySqlCommand, connection, mySqlTransaction, commandText, cmdParms);
								if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine || commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine)
								{
									if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
									{
										mySqlTransaction.Rollback();
										return 0;
									}
									object obj = mySqlCommand.ExecuteScalar();
									bool flag2 = false;
									if (obj == null && obj == DBNull.Value)
									{
										flag2 = false;
									}
									flag2 = (Convert.ToInt32(obj) > 0);
									if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine && !flag2)
									{
										mySqlTransaction.Rollback();
										return 0;
									}
									if (commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine & flag2)
									{
										mySqlTransaction.Rollback();
										return 0;
									}
								}
								else
								{
									int num3 = mySqlCommand.ExecuteNonQuery();
									list.Add(mySqlCommand.Clone());
									num2 += num3;
									if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && num3 == 0)
									{
										mySqlTransaction.Rollback();
										return 0;
									}
									mySqlCommand.Parameters.Clear();
								}
							}
						}
						mySqlTransaction.Commit();
						return num2;
					}
					catch (Exception ex)
					{
						MLogger.Log(ex);
						mySqlTransaction.Rollback();
						flag = false;
						throw;
					}
					finally
					{
						if (flag)
						{
							MSqlLogger.LOG(mContext, cmdList, connection);
						}
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		public void ExecuteSqlTranWithIndentity(List<CommandInfo> lstSql)
		{
			bool flag = true;
			List<MySqlCommand> list = new List<MySqlCommand>();
			using (connection = DBConnectionManager.GetDBConnection(mContext))
			{
				connection.Open();
				using (MySqlTransaction mySqlTransaction = connection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					mySqlCommand.CommandTimeout = cmdTimeout;
					try
					{
						int num = 0;
						foreach (CommandInfo item in lstSql)
						{
							string commandText = item.CommandText;
							MySqlParameter[] array = (MySqlParameter[])item.Parameters;
							MySqlParameter[] array2 = array;
							foreach (MySqlParameter mySqlParameter in array2)
							{
								if (mySqlParameter.Direction == ParameterDirection.InputOutput)
								{
									mySqlParameter.Value = num;
								}
							}
							PrepareCommand(mySqlCommand, connection, mySqlTransaction, commandText, array);
							int num2 = mySqlCommand.ExecuteNonQuery();
							list.Add(mySqlCommand.Clone());
							MySqlParameter[] array3 = array;
							foreach (MySqlParameter mySqlParameter2 in array3)
							{
								if (mySqlParameter2.Direction == ParameterDirection.Output)
								{
									num = Convert.ToInt32(mySqlParameter2.Value);
								}
							}
							mySqlCommand.Parameters.Clear();
						}
						mySqlTransaction.Commit();
					}
					catch (Exception ex)
					{
						MLogger.Log(ex);
						mySqlTransaction.Rollback();
						flag = false;
						throw;
					}
					finally
					{
						if (flag)
						{
							MongoBDHelper.Monitoring(mContext, (from x in list
							select x.CommandText).ToList());
							MSqlLogger.LOG(mContext, lstSql, connection);
						}
						if (connection != null && connection.State == ConnectionState.Open)
						{
							connection.Close();
						}
					}
				}
			}
		}

		private void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
		{
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			if (trans != null)
			{
				cmd.Transaction = trans;
			}
			cmd.CommandType = CommandType.Text;
			cmdParms = AddCommonParameter(cmdParms);
			if (cmdParms != null)
			{
				MySqlParameter[] array = cmdParms;
				foreach (MySqlParameter mySqlParameter in array)
				{
					if (mySqlParameter.Value != null && !XssParameterNameWhiteList.Contains(mySqlParameter.ParameterName) && (mySqlParameter.DbType == DbType.String || mySqlParameter.DbType == DbType.StringFixedLength || mySqlParameter.DbType == DbType.AnsiString || mySqlParameter.DbType == DbType.AnsiStringFixedLength))
					{
						mySqlParameter.Value = XSSFilter.XssFilter(mySqlParameter.Value.ToString());
					}
					if (mySqlParameter.Value != null && (mySqlParameter.DbType == DbType.DateTime || mySqlParameter.DbType == DbType.DateTime2 || mySqlParameter.DbType == DbType.Date))
					{
						DateTime dateTime = default(DateTime);
						if (DateTime.TryParse(mySqlParameter.Value.ToString(), out dateTime))
						{
							mySqlParameter.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
						}
					}
					if ((mySqlParameter.Direction == ParameterDirection.InputOutput || mySqlParameter.Direction == ParameterDirection.Input) && mySqlParameter.Value == null)
					{
						mySqlParameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(mySqlParameter);
				}
			}
		}

		private MySqlParameter[] AddCommonParameter(MySqlParameter[] cmdParms)
		{
			MContext obj = mContext;
			MySqlParameter mySqlParameter = (obj != null) ? obj.GetParameters((MySqlParameter)null).FirstOrDefault((MySqlParameter x) => x.ParameterName == "@AppSource") : null;
			if (mySqlParameter != null)
			{
				if (cmdParms == null)
				{
					cmdParms = new MySqlParameter[1]
					{
						mySqlParameter
					};
				}
				else if (!cmdParms.ToList().Exists((MySqlParameter x) => x.ParameterName == "@AppSource"))
				{
					List<MySqlParameter> list = cmdParms.ToList();
					list.Add(mySqlParameter);
					cmdParms = list.ToArray();
				}
			}
			return cmdParms;
		}

		private void BatchPrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, List<CommandInfo> cmdList, ref int beginIndex)
		{
			int i = beginIndex;
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			StringBuilder stringBuilder = new StringBuilder();
			for (; i < cmdList.Count; i++)
			{
				CommandInfo commandInfo = cmdList[i];
				if (commandInfo != null)
				{
					string text = commandInfo.CommandText;
					if (commandInfo.Parameters != null)
					{
						MySqlParameter[] cmdParms = (MySqlParameter[])commandInfo.Parameters;
						cmdParms = AddCommonParameter(cmdParms);
						List<MySqlParameter> list = (from a in cmdParms
						orderby a.ParameterName.Length descending
						select a).ToList();
						foreach (MySqlParameter item in list)
						{
							string text2 = item.ParameterName;
							if (commandInfo.Parameters.Length < maxParaTag)
							{
								string oldValue = text2;
								text2 = string.Format("@_{0}_", i) + text2.Replace("@", "");
								text = text.Replace(oldValue, text2);
							}
							if (item.Value != null && !XssParameterNameWhiteList.Contains(item.ParameterName) && (item.DbType == DbType.String || item.DbType == DbType.StringFixedLength || item.DbType == DbType.AnsiString || item.DbType == DbType.AnsiStringFixedLength))
							{
								item.Value = XSSFilter.XssFilter(item.Value.ToString());
							}
							if (item.Value != null && (item.DbType == DbType.DateTime || item.DbType == DbType.DateTime2 || item.DbType == DbType.Date))
							{
								DateTime dateTime = default(DateTime);
								if (DateTime.TryParse(item.Value.ToString(), out dateTime))
								{
									item.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
								}
							}
							if ((item.Direction == ParameterDirection.InputOutput || item.Direction == ParameterDirection.Input) && item.Value == null)
							{
								item.Value = DBNull.Value;
							}
							if (!cmd.Parameters.Contains(text2))
							{
								cmd.Parameters.AddWithValue(text2, item.Value);
							}
						}
					}
					text = text.TrimEnd().TrimEnd(';');
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(";" + text);
					}
					else
					{
						stringBuilder.Append(text);
					}
					if (commandInfo.Parameters != null && commandInfo.Parameters.Length >= maxParaTag)
					{
						break;
					}
				}
			}
			cmd.CommandText = stringBuilder.ToString();
			if (trans != null)
			{
				cmd.Transaction = trans;
			}
			cmd.CommandType = CommandType.Text;
			beginIndex = i + 1;
		}

		private void BatchPrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, List<CommandInfo> cmdList)
		{
			int i = 0;
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			StringBuilder stringBuilder = new StringBuilder();
			for (; i < cmdList.Count; i++)
			{
				CommandInfo commandInfo = cmdList[i];
				if (commandInfo != null)
				{
					string text = commandInfo.CommandText;
					DbParameter[] array = commandInfo.Parameters = AddCommonParameter((MySqlParameter[])commandInfo.Parameters);
					if (commandInfo.Parameters != null)
					{
						MySqlParameter[] source = (MySqlParameter[])commandInfo.Parameters;
						List<MySqlParameter> list = (from a in source
						orderby a.ParameterName.Length descending
						select a).ToList();
						foreach (MySqlParameter item in list)
						{
							string text2 = item.ParameterName;
							if (commandInfo.Parameters.Length < maxParaTag)
							{
								string oldValue = text2;
								text2 = string.Format("@_{0}_", i) + text2.Replace("@", "");
								text = text.Replace(oldValue, text2);
							}
							if (item.Value != null && !XssParameterNameWhiteList.Contains(item.ParameterName) && (item.DbType == DbType.String || item.DbType == DbType.StringFixedLength || item.DbType == DbType.AnsiString || item.DbType == DbType.AnsiStringFixedLength))
							{
								item.Value = XSSFilter.XssFilter(item.Value.ToString());
							}
							if (item.Value != null && (item.DbType == DbType.DateTime || item.DbType == DbType.DateTime2 || item.DbType == DbType.Date))
							{
								DateTime dateTime = default(DateTime);
								if (DateTime.TryParse(item.Value.ToString(), out dateTime))
								{
									item.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
								}
							}
							if ((item.Direction == ParameterDirection.InputOutput || item.Direction == ParameterDirection.Input) && item.Value == null)
							{
								item.Value = DBNull.Value;
							}
							if (!cmd.Parameters.Contains(text2))
							{
								cmd.Parameters.AddWithValue(text2, item.Value);
							}
						}
					}
					text = text.TrimEnd().TrimEnd(';');
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(";" + text);
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
			}
			cmd.CommandText = stringBuilder.ToString();
			if (trans != null)
			{
				cmd.Transaction = trans;
			}
			cmd.CommandType = CommandType.Text;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing && connection != null)
				{
					try
					{
						connection.Close();
						connection.Dispose();
					}
					catch (Exception ex)
					{
						MLogger.Log("ExecuteSqlTran", ex, null);
					}
					finally
					{
						connection = null;
					}
				}
				disposed = true;
			}
		}
	}
}
