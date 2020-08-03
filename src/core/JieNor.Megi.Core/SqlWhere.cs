using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class SqlWhere
	{
		private int _pageIndex = 1;

		private int _pageSize = 10;

		private int _top = 0;

		[DataMember]
		private SqlFilter filter = null;

		[DataMember]
		private SqlOrderBy orderBy = new SqlOrderBy();

		[DataMember]
		public string SqlString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (!string.IsNullOrWhiteSpace(WhereSqlString))
				{
					stringBuilder.AppendLine(WhereSqlString);
				}
				if (!string.IsNullOrWhiteSpace(OrderBySqlString))
				{
					stringBuilder.AppendLine(OrderBySqlString);
				}
				if (!string.IsNullOrWhiteSpace(PageSqlString))
				{
					stringBuilder.AppendLine(PageSqlString);
				}
				return stringBuilder.ToString();
			}
			set
			{
			}
		}

		[DataMember]
		public string FilterString
		{
			get
			{
				if (filter != null)
				{
					return filter.FilterString;
				}
				return "";
			}
			set
			{
			}
		}

		[DataMember]
		public string WhereSqlString
		{
			get
			{
				if (string.IsNullOrWhiteSpace(FilterString))
				{
					return "";
				}
				return " Where " + FilterString;
			}
			set
			{
			}
		}

		[DataMember]
		public string FilterAndOrderByString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (!string.IsNullOrWhiteSpace(WhereSqlString))
				{
					stringBuilder.AppendLine(WhereSqlString);
				}
				if (!string.IsNullOrWhiteSpace(OrderBySqlString))
				{
					stringBuilder.AppendLine(OrderBySqlString);
				}
				return stringBuilder.ToString();
			}
			set
			{
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

		public string OrderBySqlString
		{
			get
			{
				return orderBy.OrderByString;
			}
		}

		[DataMember]
		public int page
		{
			get
			{
				return _pageIndex;
			}
			set
			{
				_pageIndex = value;
			}
		}

		[DataMember]
		public int rows
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = value;
			}
		}

		[DataMember]
		public int PageIndex
		{
			get
			{
				return _pageIndex;
			}
			set
			{
				_pageIndex = value;
			}
		}

		[DataMember]
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = value;
			}
		}

		[DataMember]
		public int Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top = value;
				_pageSize = value;
				_pageIndex = 1;
			}
		}

		public string PageSqlString
		{
			get
			{
				if (_pageSize == 0)
				{
					return string.Empty;
				}
				int num = (_pageIndex - 1) * _pageSize;
				return string.Format(" LIMIT {0}, {1} ", num, _pageSize);
			}
		}

		public SqlWhere()
		{
			_pageIndex = 1;
			_pageSize = 20;
		}

		public override string ToString()
		{
			return SqlString;
		}

		public void AddParameter(MySqlParameter para)
		{
			if (filter == null)
			{
				filter = new SqlFilter();
			}
			filter.AddParameter(para);
		}

		public SqlWhere GreaterThen(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.Greater, value);
		}

		public SqlWhere GreaterThen(string fieldName, decimal value)
		{
			return AddFilter(fieldName, SqlOperators.Greater, value);
		}

		public SqlWhere GreaterThen(string fieldName, int value)
		{
			return AddFilter(fieldName, SqlOperators.Greater, value);
		}

		public SqlWhere GreaterThen(string fieldName, DateTime value)
		{
			return AddFilter(fieldName, SqlOperators.Greater, value);
		}

		public SqlWhere LessThen(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.Less, value);
		}

		public SqlWhere LessThen(string fieldName, decimal value)
		{
			return AddFilter(fieldName, SqlOperators.Less, value);
		}

		public SqlWhere LessThen(string fieldName, int value)
		{
			return AddFilter(fieldName, SqlOperators.Less, value);
		}

		public SqlWhere LessThen(string fieldName, DateTime value)
		{
			return AddFilter(fieldName, SqlOperators.Less, value);
		}

		public SqlWhere Equal(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.Equal, value);
		}

		public SqlWhere Equal(string fieldName, decimal value)
		{
			return AddFilter(fieldName, SqlOperators.Equal, value);
		}

		public SqlWhere Equal(string fieldName, int value)
		{
			return AddFilter(fieldName, SqlOperators.Equal, value);
		}

		public SqlWhere Equal(string fieldName, DateTime value)
		{
			return AddFilter(fieldName, SqlOperators.Equal, value);
		}

		public SqlWhere StartsLike(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.StartsLike, value);
		}

		public SqlWhere EndsLike(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.EndsLike, value);
		}

		public SqlWhere Like(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.Like, value);
		}

		public SqlWhere NotLike(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.NotLike, value);
		}

		public SqlWhere LessOrEqual(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.LessOrEqual, value);
		}

		public SqlWhere LessOrEqual(string fieldName, decimal value)
		{
			return AddFilter(fieldName, SqlOperators.LessOrEqual, value);
		}

		public SqlWhere LessOrEqual(string fieldName, int value)
		{
			return AddFilter(fieldName, SqlOperators.LessOrEqual, value);
		}

		public SqlWhere LessOrEqual(string fieldName, DateTime value)
		{
			return AddFilter(fieldName, SqlOperators.LessOrEqual, value);
		}

		public SqlWhere GreaterOrEqual(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.GreaterOrEqual, value);
		}

		public SqlWhere GreaterOrEqual(string fieldName, decimal value)
		{
			return AddFilter(fieldName, SqlOperators.GreaterOrEqual, value);
		}

		public SqlWhere GreaterOrEqual(string fieldName, int value)
		{
			return AddFilter(fieldName, SqlOperators.GreaterOrEqual, value);
		}

		public SqlWhere GreaterOrEqual(string fieldName, DateTime value)
		{
			return AddFilter(fieldName, SqlOperators.GreaterOrEqual, value);
		}

		public SqlWhere NotEqual(string fieldName, string value)
		{
			return AddFilter(fieldName, SqlOperators.NotEqual, value);
		}

		public SqlWhere NotEqual(string fieldName, decimal value)
		{
			return AddFilter(fieldName, SqlOperators.NotEqual, value);
		}

		public SqlWhere NotEqual(string fieldName, int value)
		{
			return AddFilter(fieldName, SqlOperators.NotEqual, value);
		}

		public SqlWhere NotEqual(string fieldName, DateTime value)
		{
			return AddFilter(fieldName, SqlOperators.NotEqual, value);
		}

		public SqlWhere In(string fieldName, string[] values)
		{
			return AddFilter(fieldName, SqlOperators.In, values);
		}

		public SqlWhere In(string fieldName, List<string> values)
		{
			return AddFilter(fieldName, SqlOperators.In, values);
		}

		public SqlWhere NotIn(string fieldName, string[] values)
		{
			return AddFilter(fieldName, SqlOperators.NotIn, values);
		}

		public SqlWhere NotIn(string fieldName, List<string> values)
		{
			return AddFilter(fieldName, SqlOperators.NotIn, values);
		}

		public SqlWhere AddFilter(SqlFilter filter)
		{
			if (this.filter == null)
			{
				this.filter = filter;
			}
			else
			{
				this.filter.And(filter);
			}
			return this;
		}

		public SqlWhere AddFilter(FilterList filter)
		{
			if (filter == null || filter.Count == 0)
			{
				return this;
			}
			if (this.filter == null)
			{
				this.filter = new SqlFilter(((List<SqlFilter>)filter)[0]);
				for (int i = 1; i < filter.Count; i++)
				{
					this.filter.And(((List<SqlFilter>)filter)[i]);
				}
			}
			else
			{
				this.filter.And(filter);
			}
			return this;
		}

		public SqlWhere AddFilter(string fieldName, SqlOperators sqlOperator, string value)
		{
			if (filter == null)
			{
				filter = new SqlFilter(fieldName, sqlOperator, value);
			}
			else
			{
				filter.And(new SqlFilter(fieldName, sqlOperator, value));
			}
			return this;
		}

		public SqlWhere AddFilter(string fieldName, SqlOperators sqlOperator, decimal value)
		{
			if (filter == null)
			{
				filter = new SqlFilter(fieldName, sqlOperator, value);
			}
			else
			{
				filter.And(new SqlFilter(fieldName, sqlOperator, value));
			}
			return this;
		}

		public SqlWhere AddFilter(string fieldName, SqlOperators sqlOperator, int value)
		{
			if (filter == null)
			{
				filter = new SqlFilter(fieldName, sqlOperator, value);
			}
			else
			{
				filter.And(new SqlFilter(fieldName, sqlOperator, value));
			}
			return this;
		}

		public SqlWhere AddFilter(string fieldName, SqlOperators sqlOperator, DateTime value)
		{
			if (filter == null)
			{
				filter = new SqlFilter(fieldName, sqlOperator, value);
			}
			else
			{
				filter.And(new SqlFilter(fieldName, sqlOperator, value));
			}
			return this;
		}

		public SqlWhere AddFilter(string fieldName, SqlOperators sqlOperator, string[] values)
		{
			if (filter == null)
			{
				filter = new SqlFilter(fieldName, sqlOperator, values);
			}
			else
			{
				filter.And(new SqlFilter(fieldName, sqlOperator, values));
			}
			return this;
		}

		public SqlWhere AddFilter(string fieldName, SqlOperators sqlOperator, List<string> values)
		{
			if (filter == null)
			{
				filter = new SqlFilter(fieldName, sqlOperator, values);
			}
			else
			{
				filter.And(new SqlFilter(fieldName, sqlOperator, values));
			}
			return this;
		}

		public SqlWhere AddOrFilter(string fieldName, SqlOperators sqlOperator, string value)
		{
			if (filter == null)
			{
				filter = new SqlFilter(fieldName, sqlOperator, value);
			}
			else
			{
				filter.Or(new SqlFilter(fieldName, sqlOperator, value));
			}
			return this;
		}

		public SqlWhere AddDeleteFilter(string fieldName = "MIsDelete", SqlOperators sqlOperator = SqlOperators.Equal, bool delete = false)
		{
			if (filter == null)
			{
				if (delete)
				{
					filter = new SqlFilter("MIsDelete", sqlOperator, 1);
				}
				else
				{
					filter = new SqlFilter("MIsDelete", sqlOperator, 0);
				}
			}
			else if (delete)
			{
				filter.And(new SqlFilter("MIsDelete", sqlOperator, 1));
			}
			else
			{
				filter.And(new SqlFilter("MIsDelete", sqlOperator, 0));
			}
			return this;
		}

		public SqlWhere OrderBy(string orderByString)
		{
			orderBy.OrderBy(orderByString);
			return this;
		}

		public SqlWhere AddOrderBy(string fieldName, SqlOrderDir direct = SqlOrderDir.Asc)
		{
			orderBy.OrderBy(fieldName, direct);
			return this;
		}

		public SqlWhere ThenBy(string fieldName, SqlOrderDir direct = SqlOrderDir.Asc)
		{
			orderBy.ThenBy(fieldName, direct);
			return this;
		}
	}
}
