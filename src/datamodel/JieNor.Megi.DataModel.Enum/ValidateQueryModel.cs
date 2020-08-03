using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataModel.Enum
{
	public class ValidateQueryModel
	{
		public string QuerySQL
		{
			get;
			set;
		}

		public MySqlParameter[] Parameter
		{
			get;
			set;
		}

		public MActionResultCodeEnum ErrorCode
		{
			get;
			set;
		}

		public ValidateQueryModel()
		{
		}

		public ValidateQueryModel(string querySQL, MySqlParameter[] parameter)
		{
			QuerySQL = querySQL;
			Parameter = parameter;
		}

		public ValidateQueryModel(MActionResultCodeEnum code)
		{
			ErrorCode = code;
			QuerySQL = " select " + (int)code + " as MCode ";
			Parameter = null;
		}

		public ValidateQueryModel Join(ValidateQueryModel other)
		{
			if (other != null)
			{
				if (string.IsNullOrWhiteSpace(QuerySQL))
				{
					QuerySQL = other.QuerySQL;
				}
				else
				{
					QuerySQL += ((!string.IsNullOrWhiteSpace(other.QuerySQL)) ? (" union all " + other.QuerySQL) : "");
				}
				if (Parameter == null)
				{
					Parameter = other.Parameter;
				}
				else
				{
					Parameter = Distinct(Parameter.Concat((other.Parameter == null) ? new MySqlParameter[0] : other.Parameter).ToArray());
				}
				Parameter = (Parameter ?? new MySqlParameter[0]);
			}
			return this;
		}

		private MySqlParameter[] Distinct(MySqlParameter[] parameters)
		{
			List<string> list = new List<string>();
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			for (int i = 0; i < parameters.Count(); i++)
			{
				if (!list.Contains(parameters[i].ParameterName))
				{
					list2.Add(parameters[i]);
					list.Add(parameters[i].ParameterName);
				}
			}
			return list2.ToArray();
		}
	}
}
