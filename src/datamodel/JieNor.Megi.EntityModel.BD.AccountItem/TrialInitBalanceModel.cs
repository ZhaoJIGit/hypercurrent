namespace JieNor.Megi.EntityModel.BD.AccountItem
{
	public class TrialInitBalanceModel
	{
		public bool Success
		{
			get;
			set;
		}

		public decimal DebitAmount
		{
			get;
			set;
		}

		public decimal CreditAmount
		{
			get;
			set;
		}

		public decimal YtdDebit
		{
			get;
			set;
		}

		public decimal YtdCredit
		{
			get;
			set;
		}
	}
}
