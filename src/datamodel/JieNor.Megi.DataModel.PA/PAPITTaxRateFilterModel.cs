using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPITTaxRateFilterModel : SqlWhere
	{
		[DataMember]
		public DateTime SalaryDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal? NetSalary
		{
			get;
			set;
		}

		[DataMember]
		public string EmployeeID
		{
			get;
			set;
		}
	}
}
