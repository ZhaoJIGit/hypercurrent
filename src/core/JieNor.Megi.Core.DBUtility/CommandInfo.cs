using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace JieNor.Megi.Core.DBUtility
{
	public class CommandInfo
	{
		public object ShareObject = null;

		public object OriginalData = null;

		public string CommandText;

		public DbParameter[] Parameters;

		public EffentNextType EffentNextType = EffentNextType.None;

		public string TableName;

		private event EventHandler _solicitationEvent;

		public event EventHandler SolicitationEvent
		{
			add
			{
				_solicitationEvent += value;
			}
			remove
			{
				_solicitationEvent -= value;
			}
		}

		public void OnSolicitationEvent()
		{
			if (this._solicitationEvent != null)
			{
				this._solicitationEvent(this, new EventArgs());
			}
		}

		public CommandInfo()
		{
		}

		public CommandInfo(string sqlText, MySqlParameter[] para)
		{
			CommandText = sqlText;
			DbParameter[] array = Parameters = para;
		}

		public CommandInfo(string tableName, string sqlText, MySqlParameter[] para)
		{
			TableName = tableName;
			CommandText = sqlText;
			DbParameter[] array = Parameters = para;
		}

		public CommandInfo(string sqlText, MySqlParameter[] para, EffentNextType type)
		{
			CommandText = sqlText;
			DbParameter[] array = Parameters = para;
			EffentNextType = type;
		}
	}
}
