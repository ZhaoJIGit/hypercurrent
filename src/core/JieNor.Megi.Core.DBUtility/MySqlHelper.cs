using JieNor.Megi.Common.Utility;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace JieNor.Megi.Core.DBUtility
{
	public abstract class MySqlHelper
	{
		public static readonly string DBConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();

		private static void PrepareCommand(MySqlConnection conn, MySqlTransaction trans, MySqlCommand cmd, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
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
			cmd.CommandType = cmdType;
			if (cmdParms != null)
			{
				foreach (MySqlParameter mySqlParameter in cmdParms)
				{
					if (mySqlParameter.Value != null && (mySqlParameter.DbType == DbType.String || mySqlParameter.DbType == DbType.StringFixedLength || mySqlParameter.DbType == DbType.AnsiString || mySqlParameter.DbType == DbType.AnsiStringFixedLength))
					{
						mySqlParameter.Value = XSSFilter.XssFilter(mySqlParameter.Value.ToString());
					}
					cmd.Parameters.Add(mySqlParameter);
				}
			}
		}

		public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				PrepareCommand(conn, null, mySqlCommand, cmdType, cmdText, cmdParms);
				int result = mySqlCommand.ExecuteNonQuery();
				mySqlCommand.Parameters.Clear();
				return result;
			}
		}

		public static int ExecuteNonQuery(MySqlConnection conn, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			PrepareCommand(conn, null, mySqlCommand, cmdType, cmdText, cmdParms);
			int result = mySqlCommand.ExecuteNonQuery();
			mySqlCommand.Parameters.Clear();
			return result;
		}

		public static int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			PrepareCommand(trans.Connection, trans, mySqlCommand, cmdType, cmdText, cmdParms);
			int result = mySqlCommand.ExecuteNonQuery();
			mySqlCommand.Parameters.Clear();
			return result;
		}

		public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				PrepareCommand(conn, null, mySqlCommand, cmdType, cmdText, cmdParms);
				object result = mySqlCommand.ExecuteScalar();
				mySqlCommand.Parameters.Clear();
				return result;
			}
		}

		public static object ExecuteScalar(MySqlConnection conn, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			PrepareCommand(conn, null, mySqlCommand, cmdType, cmdText, cmdParms);
			object result = mySqlCommand.ExecuteScalar();
			mySqlCommand.Parameters.Clear();
			return result;
		}

		public static MySqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			try
			{
				PrepareCommand(mySqlConnection, null, mySqlCommand, cmdType, cmdText, cmdParms);
				MySqlDataReader result = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				mySqlCommand.Parameters.Clear();
				return result;
			}
			catch
			{
				mySqlConnection.Close();
				throw;
			}
		}

		public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] cmdParms)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				PrepareCommand(mySqlConnection, null, mySqlCommand, cmdType, cmdText, cmdParms);
				MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
				DataSet dataSet = new DataSet();
				mySqlDataAdapter.Fill(dataSet);
				mySqlConnection.Close();
				mySqlCommand.Parameters.Clear();
				return dataSet;
			}
		}
	}
}
