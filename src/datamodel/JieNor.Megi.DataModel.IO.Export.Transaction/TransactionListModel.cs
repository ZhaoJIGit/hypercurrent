using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Transaction
{
	[DataContract]
	public class TransactionListModel
	{
		public string AccountTitle
		{
			get;
			set;
		}

		public string TypeTitle
		{
			get;
			set;
		}

		public string ContactTypeTitle
		{
			get;
			set;
		}

		public string ContactNameTitle
		{
			get;
			set;
		}

		public string DateTitle
		{
			get;
			set;
		}

		public string DescriptionTitle
		{
			get;
			set;
		}

		public string RefTitle
		{
			get;
			set;
		}

		public string SpentTitle
		{
			get;
			set;
		}

		public string ReceivedTitle
		{
			get;
			set;
		}

		public string StatusTitle
		{
			get;
			set;
		}

		public string IsAdvanceTitle
		{
			get;
			set;
		}

		public string CurrencyTitle
		{
			get;
			set;
		}

		public string TaxTypeTitle
		{
			get;
			set;
		}

		public string ItemTitle
		{
			get;
			set;
		}

		public string ItemDescTitle
		{
			get;
			set;
		}

		public string QuantityTitle
		{
			get;
			set;
		}

		public string UnitPriceTitle
		{
			get;
			set;
		}

		public string DiscountTitle
		{
			get;
			set;
		}

		public string TaxRateTitle
		{
			get;
			set;
		}

		public string AmountTitle
		{
			get;
			set;
		}

		public string TaxAmountTitle
		{
			get;
			set;
		}

		public string DepartmentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem1Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem2Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem3Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem4Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem5Name
		{
			get;
			set;
		}

		[DataMember]
		public TransactionListRowCollection TransactionListRows
		{
			get;
			set;
		}
	}
}
