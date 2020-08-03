using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVAccountTransactionsModel
	{
		[DataMember]
		public string MID
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
		public string MContactName
		{
			get;
			set;
		}

		[DataMember]
		public string MDescription
		{
			get;
			set;
		}

		[DataMember]
		public string MReference
		{
			get;
			set;
		}

		[DataMember]
		public string MSpent
		{
			get;
			set;
		}

		[DataMember]
		public string MReceived
		{
			get;
			set;
		}

		[DataMember]
		public string MStatus
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
		public string MTransType
		{
			get;
			set;
		}

		[DataMember]
		public string MAttachIDs
		{
			get;
			set;
		}

		[DataMember]
		public int MReconcileStatu
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

		[DataMember]
		public decimal MVerificationAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MVerificationAmtBaseCurrency
		{
			get;
			set;
		}

		[DataMember]
		public decimal MVerifyAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MVerifyAmtFor
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
	}
}
