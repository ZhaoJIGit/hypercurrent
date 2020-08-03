using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankBillEntryListFilterModel : SqlWhere
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
		public string MKeyword
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
		public DateTime? ExactDate
		{
			get;
			set;
		}

		[DataMember]
		public string TransAcctName
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public int SrcFrom
		{
			get;
			set;
		}

		[DataMember]
		public decimal? AmountFrom
		{
			get;
			set;
		}

		[DataMember]
		public decimal? AmountTo
		{
			get;
			set;
		}

		[DataMember]
		public bool IsExactAmount
		{
			get;
			set;
		}
	}
}
