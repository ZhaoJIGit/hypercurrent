using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class ORFilter : SqlFilter
	{
		[DataMember]
		public override string FilterString
		{
			get
			{
				return " Or " + base.FilterString;
			}
		}

		public ORFilter(string fieldName, SqlOperators sqlOperator, string value)
			: base(fieldName, sqlOperator, value)
		{
		}

		public ORFilter(string fieldName, SqlOperators sqlOperator, decimal value)
			: base(fieldName, sqlOperator, value)
		{
		}

		public ORFilter(string fieldName, SqlOperators sqlOperator, int value)
			: base(fieldName, sqlOperator, value)
		{
		}

		public ORFilter(string fieldName, SqlOperators sqlOperator, DateTime value)
			: base(fieldName, sqlOperator, value)
		{
		}

		public ORFilter(string fieldName, SqlOperators sqlOperator, string[] values)
			: base(fieldName, sqlOperator, values)
		{
		}

		public ORFilter(string fieldName, SqlOperators sqlOperator, List<string> values)
			: base(fieldName, sqlOperator, values)
		{
		}
	}
}
