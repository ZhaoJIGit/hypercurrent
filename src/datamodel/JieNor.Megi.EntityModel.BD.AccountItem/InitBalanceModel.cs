using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.BD.AccountItem
{
	public class InitBalanceModel
	{
		[DataMember]
		public string MItemID
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
		public decimal MInitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitBalance
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

		public decimal MYtdDebitFor
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

		public decimal MYtdCreditFor
		{
			get;
			set;
		}
	}
}
