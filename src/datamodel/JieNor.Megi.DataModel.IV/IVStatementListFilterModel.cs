using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVStatementListFilterModel : SqlWhere
	{
		[DataMember]
		public string Sort
		{
			get;
			set;
		}

		[DataMember]
		public string Order
		{
			get;
			set;
		}

		[DataMember]
		public DateTime StartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime EndDate
		{
			get;
			set;
		}

		[DataMember]
		public string Filter
		{
			get;
			set;
		}

		[DataMember]
		public string StatementType
		{
			get;
			set;
		}
	}
}
