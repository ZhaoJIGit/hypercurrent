using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class ANDFilter : SqlFilter
	{
		[DataMember]
		public override string FilterString
		{
			get
			{
				return " And " + base.FilterString;
			}
		}

		public ANDFilter(string fieldName, SqlOperators sqlOperator, string value)
			: base(fieldName, sqlOperator, value)
		{
		}

		public ANDFilter(string fieldName, SqlOperators sqlOperator, decimal value)
			: base(fieldName, sqlOperator, value)
		{
		}

		public ANDFilter(string fieldName, SqlOperators sqlOperator, int value)
			: base(fieldName, sqlOperator, value)
		{
		}

		public ANDFilter(string fieldName, SqlOperators sqlOperator, DateTime value)
			: base(fieldName, sqlOperator, value)
		{
		}

		public ANDFilter(string fieldName, SqlOperators sqlOperator, string[] values)
			: base(fieldName, sqlOperator, values)
		{
		}

		public ANDFilter(string fieldName, SqlOperators sqlOperator, List<string> values)
			: base(fieldName, sqlOperator, values)
		{
		}
	}
}
