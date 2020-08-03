using JieNor.Megi.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVReconcileTranstionListFilterModel : SqlWhere
	{
		[DataMember]
		public string Keyword
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
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public string BizObject
		{
			get;
			set;
		}

		[DataMember]
		public List<decimal> MAmountList
		{
			get;
			set;
		}

		[DataMember]
		public string MBankID
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
		public DateTime MBankBillDate
		{
			get;
			set;
		}
	}
}
