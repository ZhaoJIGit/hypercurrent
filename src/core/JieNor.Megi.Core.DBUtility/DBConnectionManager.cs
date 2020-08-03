using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;

namespace JieNor.Megi.Core.DBUtility
{
	public static class DBConnectionManager
	{
		private static ConcurrentDictionary<string, MySqlConnection> connCache = new ConcurrentDictionary<string, MySqlConnection>();

		public static MySqlConnection GetDBConnection(MContext ctx)
		{
			string text = "";
			text = ((ctx != null && !string.IsNullOrEmpty(ctx.MOrgID) && !ctx.IsSys) ? DynamicPubConstant.ConnectionString(ctx.MOrgID) : PubConstant.ConnectionString);
			return new MySqlConnection(text);
		}
	}
}
