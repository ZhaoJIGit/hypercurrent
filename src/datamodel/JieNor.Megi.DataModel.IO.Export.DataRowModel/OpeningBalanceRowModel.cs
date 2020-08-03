using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class OpeningBalanceRowModel : ICloneable
	{
		[DisplayName("Code")]
		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DisplayName("Name")]
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DisplayName("Direction")]
		[DataMember]
		public string MDC
		{
			get;
			set;
		}

		[DataMember]
		public string Currency
		{
			get;
			set;
		}

		[DisplayName("Openingbalance")]
		[DataMember]
		public decimal? MInitBalanceFor
		{
			get;
			set;
		}

		[DisplayName("CumulativeDebitThisYear")]
		[DataMember]
		public decimal? MYtdDebit
		{
			get;
			set;
		}

		[DisplayName("CumulativeCreditThisYear")]
		[DataMember]
		public decimal? MYtdCredit
		{
			get;
			set;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
