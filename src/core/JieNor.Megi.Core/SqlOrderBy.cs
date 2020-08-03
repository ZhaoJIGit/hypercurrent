using JieNor.Megi.Common.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class SqlOrderBy
	{
		[DataMember]
		private List<string> order = new List<string>();

		[DataMember]
		public string OrderByString
		{
			get
			{
				if (order.Count == 0)
				{
					return "";
				}
				return " Order By " + string.Join(",", order);
			}
			set
			{
			}
		}

		public SqlOrderBy OrderBy(string orderByString)
		{
			string[] array = orderByString.Trim().Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Trim().IndexOf(' ') > -1)
				{
					string[] array3 = text.Trim().Split(' ');
					string fieldName = array3[0];
					List<string> list = new List<string>();
					if (array3.Length > 2)
					{
						for (int j = 0; j < array3.Length - 1; j++)
						{
							list.Add(array3[j]);
						}
						fieldName = string.Join(" ", list);
					}
					string orderFieldName = GetOrderFieldName(fieldName);
					SqlOrderDir sqlOrderDir = SqlOrderDir.Asc;
					if (array3[array3.Length - 1].Trim().EqualsIgnoreCase(1.ToString()))
					{
						sqlOrderDir = SqlOrderDir.Desc;
					}
					order.Add(string.Format("{0} {1}", orderFieldName, sqlOrderDir));
				}
				else
				{
					string orderFieldName2 = GetOrderFieldName(text);
					order.Add(string.Format("{0} {1}", orderFieldName2, SqlOrderDir.Asc));
				}
			}
			return this;
		}

		public SqlOrderBy OrderBy(string fieldName, SqlOrderDir direct = SqlOrderDir.Asc)
		{
			string orderFieldName = GetOrderFieldName(fieldName);
			order.Add(string.Format("{0} {1}", orderFieldName, direct));
			return this;
		}

		public SqlOrderBy ThenBy(string fieldName, SqlOrderDir direct = SqlOrderDir.Asc)
		{
			string orderFieldName = GetOrderFieldName(fieldName);
			order.Add(string.Format("{0} {1}", orderFieldName, direct));
			return this;
		}

		public override string ToString()
		{
			return OrderByString;
		}

		private static string GetOrderFieldName(string fieldName)
		{
			string text = fieldName.Trim();
			if (text.Length > 60)
			{
				throw new Exception("Error: field name too long !");
			}
			if (text.IndexOf(";") >= 0 || text.IndexOf("'") >= 0 || text.IndexOf("--") >= 0)
			{
				throw new Exception("Error: field name contains illegal characters !");
			}
			return text;
		}
	}
}
