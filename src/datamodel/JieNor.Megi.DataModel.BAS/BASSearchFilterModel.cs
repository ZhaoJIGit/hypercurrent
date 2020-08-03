using JieNor.Megi.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASSearchFilterModel : SqlWhere
	{
		[DataMember]
		public string KeyWord
		{
			get;
			set;
		}

		[DataMember]
		public string BizColumn
		{
			get;
			set;
		}

		[DataMember]
		public SqlOperators SqlOperator
		{
			get;
			set;
		}

		public DateTime StartDate
		{
			get;
			set;
		}

		public DateTime EndDate
		{
			get;
			set;
		}

		[DataMember]
		public List<string> BizObjectList
		{
			get;
			set;
		}
	}
}
