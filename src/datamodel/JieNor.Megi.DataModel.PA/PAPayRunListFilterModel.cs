using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPayRunListFilterModel : SqlWhere
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
		public int Status
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

		[DataMember]
		public bool IsMorePayPunList
		{
			get;
			set;
		}
	}
}
