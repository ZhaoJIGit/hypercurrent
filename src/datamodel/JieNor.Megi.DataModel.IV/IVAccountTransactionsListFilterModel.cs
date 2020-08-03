using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVAccountTransactionsListFilterModel : SqlWhere
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
		public DateTime? StartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? EndDate
		{
			get;
			set;
		}

		[DataMember]
		public string TransactionType
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFromExcel
		{
			get;
			set;
		}

		[DataMember]
		public string Amount
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
