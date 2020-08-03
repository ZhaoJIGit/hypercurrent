using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class SqlGroupBy
	{
		[DataMember]
		private List<string> group = new List<string>();

		[DataMember]
		public string GroupByString
		{
			get
			{
				if (group.Count == 0)
				{
					return " ";
				}
				return " Group By " + string.Join(",", group);
			}
			set
			{
			}
		}

		public SqlGroupBy GroupBy(string groupByString)
		{
			string[] array = groupByString.Trim().Split(',');
			string[] array2 = array;
			foreach (string fieldName in array2)
			{
				string groupFieldName = GetGroupFieldName(fieldName);
				group.Add(groupFieldName);
			}
			return this;
		}

		public override string ToString()
		{
			return GroupByString;
		}

		private static string GetGroupFieldName(string fieldName)
		{
			string text = fieldName.Replace(" ", "");
			if (text.Length > 30)
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
