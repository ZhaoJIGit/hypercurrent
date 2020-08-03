using MySql.Data.MySqlClient;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class SqlQuery
	{
		[DataMember]
		private string select = "";

		[DataMember]
		private SqlWhere filter = new SqlWhere();

		[DataMember]
		private SqlGroupBy groupBy = new SqlGroupBy();

		private string countSql = string.Empty;

		public string SelectString
		{
			get
			{
				return " Select " + select;
			}
			set
			{
				string text = value.Trim();
				select = text.Substring("Select".Length);
			}
		}

		public SqlWhere SqlWhere
		{
			get
			{
				return filter;
			}
			set
			{
				filter = value;
			}
		}

		[DataMember]
		public MySqlParameter[] Parameters
		{
			get
			{
				if (filter != null)
				{
					return filter.Parameters;
				}
				return new MySqlParameter[0];
			}
			set
			{
			}
		}

		public string Sql
		{
			get
			{
				return SelectString + Environment.NewLine + filter.WhereSqlString + Environment.NewLine + groupBy.GroupByString + Environment.NewLine + filter.OrderBySqlString + Environment.NewLine + filter.PageSqlString;
			}
			set
			{
			}
		}

		public string CountSqlString
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(countSql))
				{
					return countSql;
				}
				string arg = SelectString + Environment.NewLine + filter.WhereSqlString + Environment.NewLine + groupBy.GroupByString;
				return string.Format("Select Count(1) As MRecordCount From ({0}) t ", arg);
			}
			set
			{
				countSql = value;
			}
		}

		public SqlQuery()
		{
		}

		public SqlQuery(string selectSqlString)
		{
			SelectString = selectSqlString;
		}

		public void AddParameter(MySqlParameter para)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			filter.AddParameter(para);
		}

		public SqlQuery OrderBy(string orderByString)
		{
			filter.OrderBy(orderByString);
			return this;
		}

		public SqlQuery AddOrderBy(string fieldName, SqlOrderDir direct = SqlOrderDir.Asc)
		{
			filter.AddOrderBy(fieldName, direct);
			return this;
		}

		public SqlQuery GroupBy(string groupByString)
		{
			groupBy.GroupBy(groupByString);
			return this;
		}

		public override string ToString()
		{
			return Sql;
		}
	}
}
