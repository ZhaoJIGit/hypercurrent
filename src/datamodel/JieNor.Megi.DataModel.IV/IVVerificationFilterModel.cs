using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVVerificationFilterModel
	{
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
		public DateTime ReceiveStartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime ReceiveEndDate
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetBillType
		{
			get;
			set;
		}

		[DataMember]
		public List<string> BillIdList
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
		public string MOrgIDs
		{
			get;
			set;
		}
	}
}
