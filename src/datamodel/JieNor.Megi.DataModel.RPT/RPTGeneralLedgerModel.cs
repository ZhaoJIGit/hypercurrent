using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTGeneralLedgerModel
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

		public string MSummary
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

		public string MContactName
		{
			get;
			set;
		}

		public string MDC
		{
			get;
			set;
		}

		public string MPeriod
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
		public decimal MDebitFor
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
		public decimal MCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBalance
		{
			get;
			set;
		}

		public bool IsShowInitBalance
		{
			get;
			set;
		}

		public string GroupByFeild
		{
			get;
			set;
		}

		public int AmountType
		{
			get;
			set;
		}

		public string MCheckGroupValueID
		{
			get;
			set;
		}
	}
}
