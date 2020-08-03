using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTAccountBalanceModel
	{
		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountParentID
		{
			get;
			set;
		}

		[DataMember]
		public string MNumberID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		public string MCheckGroupValueID
		{
			get;
			set;
		}

		public string MCheckGroupValueName
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginDebitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginCreditBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebitFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCreditFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdDebitFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdCreditFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndDebitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndCreditBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public int MYearPeriod
		{
			get;
			set;
		}

		public bool IsParentAccount
		{
			get;
			set;
		}

		public string MOrderField
		{
			get;
			set;
		}
	}
}
