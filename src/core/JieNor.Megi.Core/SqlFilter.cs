using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class SqlFilter
	{
		[DataMember]
		private string filterString = "";

		[DataMember]
		protected List<MySqlParameter> param = new List<MySqlParameter>();

		[DataMember]
		public virtual string FilterString
		{
			get
			{
				return filterString;
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
				return param.ToArray();
			}
			set
			{
			}
		}

		private string ParameterName
		{
			get
			{
				return string.Format("@FilterPara_{0}", param.Count + 1);
			}
		}

		public SqlFilter()
		{
		}

		public SqlFilter(SqlFilter filter)
		{
			param.AddRange(filter.param);
			filterString = filter.FilterString;
		}

		public SqlFilter(string fieldName, SqlOperators sqlOperator, string value)
		{
			MySqlParameter mySqlParameter = new MySqlParameter(ParameterName, MySqlDbType.VarChar, 2000)
			{
				Value = value
			};
			param.Add(mySqlParameter);
			filterString = GetFilterString(fieldName, sqlOperator, mySqlParameter.ParameterName);
		}

		public SqlFilter(string fieldName, SqlOperators sqlOperator, decimal value)
		{
			MySqlParameter mySqlParameter = new MySqlParameter(ParameterName, MySqlDbType.Decimal, 50)
			{
				Value = (object)value
			};
			param.Add(mySqlParameter);
			filterString = GetFilterString(fieldName, sqlOperator, mySqlParameter.ParameterName);
		}

		public SqlFilter(string fieldName, SqlOperators sqlOperator, int value)
		{
			MySqlParameter mySqlParameter = new MySqlParameter(ParameterName, MySqlDbType.Int32, 50)
			{
				Value = (object)value
			};
			param.Add(mySqlParameter);
			filterString = GetFilterString(fieldName, sqlOperator, mySqlParameter.ParameterName);
		}

		public SqlFilter(string fieldName, SqlOperators sqlOperator, DateTime value)
		{
			MySqlParameter mySqlParameter = new MySqlParameter(ParameterName, MySqlDbType.DateTime)
			{
				Value = (object)value
			};
			param.Add(mySqlParameter);
			filterString = GetFilterString(fieldName, sqlOperator, mySqlParameter.ParameterName);
		}

		public SqlFilter(string fieldName, SqlOperators sqlOperator, string[] values)
		{
			BuildInFilter(fieldName, sqlOperator, values);
		}

		public SqlFilter(string fieldName, SqlOperators sqlOperator, List<string> values)
		{
			BuildInFilter(fieldName, sqlOperator, values.ToArray());
		}

		private void BuildInFilter(string fieldName, SqlOperators sqlOperator, string[] values)
		{
			if (sqlOperator != SqlOperators.In && sqlOperator != SqlOperators.NotIn)
			{
				throw new Exception("Error SqlOperator. Accept In / Not In Operator Only");
			}
			List<string> list = new List<string>();
			foreach (string value in values)
			{
				MySqlParameter mySqlParameter = new MySqlParameter(ParameterName, value);
				param.Add(mySqlParameter);
				list.Add(mySqlParameter.ParameterName);
			}
			filterString = GetFilterString(fieldName, sqlOperator, list.ToArray());
		}

		public void AddParameter(MySqlParameter para)
		{
			param.Add(para);
		}

		public void AppendFilter(SqlFilter filter, string logicString = " And ")
		{
			string str = ReBuildParamenter(filter);
			if (!string.IsNullOrWhiteSpace(filterString))
			{
				filterString = " ( " + filterString + " ) " + logicString;
			}
			filterString = filterString + " (" + str + ") ";
		}

		public void AppendFilter(FilterList filters, string logicString)
		{
			if (!string.IsNullOrWhiteSpace(filterString))
			{
				filterString = " ( " + filterString + " ) " + logicString;
			}
			filterString += " ( ";
			for (int i = 0; i < filters.Count - 1; i++)
			{
				filterString += ReBuildParamenter(((List<SqlFilter>)filters)[i]);
			}
			filterString += ReBuildParamenter(((List<SqlFilter>)filters)[filters.Count - 1]);
			filterString += " ) ";
		}

		private string ReBuildParamenter(SqlFilter filter)
		{
			string text = filter.FilterString;
			int count = param.Count;
			for (int num = filter.Parameters.Length - 1; num >= 0; num--)
			{
				MySqlParameter mySqlParameter = filter.Parameters[num];
				Regex regex = new Regex(mySqlParameter.ParameterName + "(?!\\d)", RegexOptions.IgnoreCase);
				text = regex.Replace(text, string.Format("@FilterPara_{0}", count + num + 1));
				MySqlParameter mySqlParameter2 = new MySqlParameter(ParameterName, mySqlParameter.MySqlDbType, mySqlParameter.Size);
				mySqlParameter2.Value = mySqlParameter.Value;
				param.Add(mySqlParameter2);
			}
			return text;
		}

		private string GetOperate(SqlOperators sqlOperator)
		{
			switch (sqlOperator)
			{
			case SqlOperators.Greater:
				return " > ";
			case SqlOperators.Less:
				return " < ";
			case SqlOperators.Equal:
				return " = ";
			case SqlOperators.LessOrEqual:
				return " <= ";
			case SqlOperators.GreaterOrEqual:
				return " >= ";
			case SqlOperators.NotEqual:
				return " <> ";
			case SqlOperators.StartsLike:
			case SqlOperators.EndsLike:
			case SqlOperators.Like:
				return " LIKE ";
			case SqlOperators.NotLike:
				return " NOT LIKE ";
			case SqlOperators.In:
				return " In ";
			case SqlOperators.NotIn:
				return " Not In ";
			case SqlOperators.IsNotNull:
				return " is not null ";
			case SqlOperators.IsNull:
				return " is null ";
			default:
				throw new Exception("This operator type is not supported");
			}
		}

		private string GetFilterString(string fieldName, SqlOperators sqlOperator, params string[] paraName)
		{
			if (paraName.Length == 0)
			{
				throw new Exception("Error Paramenter.");
			}
			string operate = GetOperate(sqlOperator);
			if (sqlOperator == SqlOperators.In || sqlOperator == SqlOperators.NotIn)
			{
				return string.Format(" {0} {1} ({2}) ", fieldName, operate, string.Join(",", paraName));
			}
			if (sqlOperator == SqlOperators.NotLike || sqlOperator == SqlOperators.Like)
			{
				return string.Format(" {0} {1} concat('%',{2},'%') ", fieldName, operate, string.Join(",", paraName));
			}
			switch (sqlOperator)
			{
			case SqlOperators.StartsLike:
				return string.Format(" {0} {1} concat({2},'%') ", fieldName, operate, string.Join(",", paraName));
			case SqlOperators.EndsLike:
				return string.Format(" {0} {1} concat('%',{2}) ", fieldName, operate, string.Join(",", paraName));
			case SqlOperators.IsNotNull:
				return string.Format(" {0} is not null ", fieldName);
			default:
				return string.Format(" {0} {1} {2} ", fieldName, operate, paraName[0]);
			}
		}
	}
}
