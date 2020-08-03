using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryPaymentListFilterModel : SqlWhere
	{
		[DataMember]
		public string PayRunID
		{
			get;
			set;
		}

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
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string ApiModule
		{
			get;
			set;
		}

		[DataMember]
		public string StartDate
		{
			get;
			set;
		}

		[DataMember]
		public string EndDate
		{
			get;
			set;
		}
	}
}
