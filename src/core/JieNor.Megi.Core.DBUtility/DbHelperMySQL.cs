using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Logger;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JieNor.Megi.Core.DBUtility
{
	public abstract class DbHelperMySQL
	{
		private static string connectionString = PubConstant.ConnectionString;

		private static string[] XssParameterNameWhiteList = new string[1]
		{
			"@MReportLayout"
		};

		public DbHelperMySQL()
		{
		}

		public static int GetMaxID(string FieldName, string TableName)
		{
			string sQLString = "select max(" + FieldName + ")+1 from " + TableName;
			object single = GetSingle(sQLString);
			if (single == null)
			{
				return 1;
			}
			return int.Parse(single.ToString());
		}

		public static bool Exists(string strSql)
		{
			object single = GetSingle(strSql);
			if (object.Equals(single, null) || object.Equals(single, DBNull.Value) || int.Parse(single.ToString()) == 0)
			{
				return false;
			}
			return true;
		}

		public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
		{
			object single = GetSingle(strSql, cmdParms);
			if (object.Equals(single, null) || object.Equals(single, DBNull.Value) || int.Parse(single.ToString()) == 0)
			{
				return false;
			}
			return true;
		}

		public static int ExecuteSql(string SQLString)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						return mySqlCommand.ExecuteNonQuery();
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static int ExecuteNonQuery(MContext ctx, string SQLString, string connString)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						mySqlCommand.CommandTimeout = 600;
						int result = mySqlCommand.ExecuteNonQuery();
						MSqlLogger.LOG(ctx, mySqlCommand);
						return result;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static int ExecuteSqlTran(MContext ctx, List<string> SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					mySqlCommand.Connection = mySqlConnection;
					using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
					{
						mySqlCommand.Transaction = mySqlTransaction;
						try
						{
							int num = 0;
							for (int i = 0; i < SQLStringList.Count; i++)
							{
								string text = SQLStringList[i];
								if (text.Trim().Length > 1)
								{
									mySqlCommand.CommandText = text;
									num += mySqlCommand.ExecuteNonQuery();
									MSqlLogger.LOG(ctx, mySqlCommand);
								}
							}
							mySqlTransaction.Commit();
							return num;
						}
						catch (Exception ex)
						{
							mySqlTransaction.Rollback();
							MLogger.Log("ExecuteSqlTran", ex, null);
							return 0;
						}
						finally
						{
							if (mySqlConnection.State == ConnectionState.Open)
							{
								mySqlConnection.Close();
							}
						}
					}
				}
			}
		}

		public static int ExecuteSql(MContext ctx, string SQLString, string content)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					MySqlParameter mySqlParameter = new MySqlParameter("@content", SqlDbType.Text);
					mySqlParameter.Value = content;
					mySqlParameter.Direction = ParameterDirection.Input;
					mySqlCommand.CommandType = CommandType.StoredProcedure;
					mySqlCommand.Parameters.Add(mySqlParameter);
					try
					{
						mySqlConnection.Open();
						int result = mySqlCommand.ExecuteNonQuery();
						MSqlLogger.LOG(ctx, mySqlCommand);
						return result;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static object GetSingle(string SQLString)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						object obj = mySqlCommand.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							return null;
						}
						return obj;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static DataSet Query(string SQLString)
		{
			return Query(connectionString, SQLString);
		}

		public static DataSet Query(string connString, string SQLString)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connString))
			{
				DataSet dataSet = new DataSet();
				using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(SQLString, connString))
				{
					try
					{
						mySqlConnection.Open();
						mySqlDataAdapter.Fill(dataSet, "ds");
						return dataSet;
					}
					catch (MySqlException ex)
					{
						throw new Exception(ex.Message);
					}
					finally
					{
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static int ExecuteSql(MContext ctx, string SQLString, params MySqlParameter[] cmdParms)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						PrepareCommand(mySqlCommand, mySqlConnection, null, SQLString, cmdParms);
						int result = mySqlCommand.ExecuteNonQuery();
						MSqlLogger.LOG(ctx, mySqlCommand);
						mySqlCommand.Parameters.Clear();
						return result;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static void ExecuteSqlTran(MContext ctx, Hashtable SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						IDictionaryEnumerator enumerator = SQLStringList.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
								string cmdText = dictionaryEntry.Key.ToString();
								MySqlParameter[] cmdParms = (MySqlParameter[])dictionaryEntry.Value;
								PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, cmdText, cmdParms);
								int num = mySqlCommand.ExecuteNonQuery();
								MSqlLogger.LOG(ctx, mySqlCommand);
								mySqlCommand.Parameters.Clear();
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
						mySqlTransaction.Commit();
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
					finally
					{
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static bool ExecuteSqlTran(MContext ctx, MultiDBCommand cmd)
		{
			return ExecuteSqlTran(ctx, new MultiDBCommand[1]
			{
				cmd
			});
		}

		public static bool ExecuteSqlTran(MContext ctx, MultiDBCommand[] cmdArray)
		{
            //cmdArray = cmdArray.Select(i=>i.CommandList.Where(i=>i.CommandText!=""));

            //cmdArray = cmdArray.SelectMany(c => c.CommandList, (o, p) =>
            //{
            //    if (!string.IsNullOrWhiteSpace(p.CommandText)) { return o; }
            //    return null;
            //}).Where(x => x != null).ToArray();

            int num = cmdArray.Length;
			MySqlConnection[] array = new MySqlConnection[num];
			MySqlTransaction[] array2 = new MySqlTransaction[num];
			try
			{
                
				for (int i = 0; i < num; i++)
				{
					MultiDBCommand multiDBCommand = cmdArray[i];
					array[i] = new MySqlConnection(multiDBCommand.ConnectionString);
					array[i].Open();
					array2[i] = array[i].BeginTransaction();
				}
				for (int j = 0; j < num; j++)
				{
					if (cmdArray[j].CommandList.Count==0) {
						continue;
					}
					MySqlCommand mySqlCommand = new MySqlCommand();
					mySqlCommand.CommandTimeout = 28800;
					BatchPrepareCommand(mySqlCommand, array[j], array2[j], cmdArray[j].CommandList, j);
					mySqlCommand.ExecuteNonQuery();
					MSqlLogger.LOG(ctx, mySqlCommand);
					mySqlCommand.Parameters.Clear();
				}
				for (int k = 0; k < num; k++)
				{
					array2[k].Commit();
				}
			}
			catch (Exception ex)
			{
				for (int l = 0; l < num; l++)
				{
					array2[l].Rollback();
				}
				MLogger.Log(ex);
				return false;
			}
			finally
			{
				for (int m = 0; m < num; m++)
				{
					array[m].Close();
				}
			}
			return true;
		}

		private static void BatchPrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, List<CommandInfo> cmdList, int index)
		{


			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < cmdList.Count; i++)
			{
				CommandInfo commandInfo = cmdList[i];
				if (commandInfo != null)
				{
					string text = commandInfo.CommandText;
					MySqlParameter[] array = (MySqlParameter[])commandInfo.Parameters;
					if (array != null)
					{
						foreach (MySqlParameter item in from a in array
						orderby a.ParameterName.Length descending
						select a)
						{
							string parameterName = item.ParameterName;
							string oldValue = parameterName;
							parameterName = string.Format("@_{0}{1}_", index, i) + parameterName.Replace("@", "");
							text = text.Replace(oldValue, parameterName);
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
							if (!cmd.Parameters.Contains(parameterName))
							{
								cmd.Parameters.AddWithValue(parameterName, item.Value);
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

		public static int ExecuteSqlTran(MContext ctx, List<CommandInfo> cmdList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						int num = 0;
						foreach (CommandInfo cmd in cmdList)
						{
							string commandText = cmd.CommandText;
							MySqlParameter[] cmdParms = (MySqlParameter[])cmd.Parameters;
							PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, commandText, cmdParms);
							if (cmd.EffentNextType == EffentNextType.WhenHaveContine || cmd.EffentNextType == EffentNextType.WhenNoHaveContine)
							{
								if (cmd.CommandText.ToLower().IndexOf("count(") == -1)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								object obj = mySqlCommand.ExecuteScalar();
								bool flag = false;
								if (obj == null && obj == DBNull.Value)
								{
									flag = false;
								}
								flag = (Convert.ToInt32(obj) > 0);
								if (cmd.EffentNextType == EffentNextType.WhenHaveContine && !flag)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								if (cmd.EffentNextType == EffentNextType.WhenNoHaveContine & flag)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
							}
							else
							{
								int num2 = mySqlCommand.ExecuteNonQuery();
								MSqlLogger.LOG(ctx, mySqlCommand);
								num += num2;
								if (cmd.EffentNextType == EffentNextType.ExcuteEffectRows && num2 == 0)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								mySqlCommand.Parameters.Clear();
							}
						}
						mySqlTransaction.Commit();
						return num;
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
					finally
					{
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static object GetSingle(string SQLString, params MySqlParameter[] cmdParms)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						PrepareCommand(mySqlCommand, mySqlConnection, null, SQLString, cmdParms);
						object obj = mySqlCommand.ExecuteScalar();
						mySqlCommand.Parameters.Clear();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							return null;
						}
						return obj;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				MySqlCommand mySqlCommand = new MySqlCommand();
				PrepareCommand(mySqlCommand, mySqlConnection, null, SQLString, cmdParms);
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
						if (mySqlConnection != null && mySqlConnection.State == ConnectionState.Open)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
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
			if (cmdParms != null)
			{
				foreach (MySqlParameter mySqlParameter in cmdParms)
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
	}
}
