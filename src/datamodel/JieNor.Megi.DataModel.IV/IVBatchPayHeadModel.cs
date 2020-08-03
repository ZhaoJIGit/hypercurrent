using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBatchPayHeadModel
	{
		[DataMember]
		public string SelectObj
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MPayDate
		{
			get;
			set;
		}

		[DataMember]
		public string MPayBank
		{
			get;
			set;
		}

		[DataMember]
		public List<IVBatchPaymentModel> PaymentEntry
		{
			get;
			set;
		}

		[DataMember]
		public bool IsPayRun
		{
			get;
			set;
		}

		[DataMember]
		public string PayRunID
		{
			get;
			set;
		}

		[DataMember]
		public string SalaryPaymentIDLists
		{
			get;
			set;
		}
	}
}
