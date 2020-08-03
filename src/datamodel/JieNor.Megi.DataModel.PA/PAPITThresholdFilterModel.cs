using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPITThresholdFilterModel : SqlWhere
	{
		[DataMember]
		public string EmployeeID
		{
			get;
			set;
		}

		[DataMember]
		public bool IsDefault
		{
			get;
			set;
		}

		[DataMember]
		public DateTime SalaryDate
		{
			get;
			set;
		}
	}
}
