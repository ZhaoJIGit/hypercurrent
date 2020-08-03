using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class BankAndCashSummaryEntryModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MBizObject
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MBizDate
		{
			get;
			set;
		}

		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string Transaction
		{
			get;
			set;
		}

		[DataMember]
		public string MBankAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string Reference
		{
			get;
			set;
		}

		[DataMember]
		public decimal IniBlance
		{
			get;
			set;
		}

		[DataMember]
		public decimal Received
		{
			get;
			set;
		}

		[DataMember]
		public decimal Payment
		{
			get;
			set;
		}

		[DataMember]
		public decimal ReceivedFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal PaymentFor
		{
			get;
			set;
		}

		[DataMember]
		public string MTableType
		{
			get;
			set;
		}

		[DataMember]
		public decimal OpeningBlance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCyRate
		{
			get;
			set;
		}

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}
	}
}
