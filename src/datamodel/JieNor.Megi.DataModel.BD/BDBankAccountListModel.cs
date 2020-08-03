using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankAccountListModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MName
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
		public string MAcctGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctGroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctTypeName
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxTypeName
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

		[DataMember]
		public decimal MYDTDebitFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYDTDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYDTCreditFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYDTCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYTDFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYTD
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndBalance
		{
			get;
			set;
		}

		[DataMember]
		public string MBankName
		{
			get;
			set;
		}

		[DataMember]
		public string MBankAcctName
		{
			get;
			set;
		}

		[DataMember]
		public string MBankNo
		{
			get;
			set;
		}

		public bool MIsBank
		{
			get;
			set;
		}

		public int MBankAccountType
		{
			get;
			set;
		}

		public bool MIsShowInHome
		{
			get;
			set;
		}

		public string MBankTypeID
		{
			get;
			set;
		}
	}
}
