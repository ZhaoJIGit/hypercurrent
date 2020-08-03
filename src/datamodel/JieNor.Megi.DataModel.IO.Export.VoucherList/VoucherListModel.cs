using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.VoucherList
{
	[DataContract]
	public class VoucherListModel : ExportBaseModel
	{
		public string DateTitle
		{
			get;
			set;
		}

		public string DateTitleWithValue
		{
			get;
			set;
		}

		public string NumberTitle
		{
			get;
			set;
		}

		public string NumberTitleWithValue
		{
			get;
			set;
		}

		public string ReferenceTitle
		{
			get;
			set;
		}

		public string AccountNoTitle
		{
			get;
			set;
		}

		public string AccountNameTitle
		{
			get;
			set;
		}

		public string AccountTitle
		{
			get;
			set;
		}

		public string ContactTitle
		{
			get;
			set;
		}

		public string DebitTitle
		{
			get;
			set;
		}

		public string CreditTitle
		{
			get;
			set;
		}

		public string StatusTitle
		{
			get;
			set;
		}

		public string CreatorTitle
		{
			get;
			set;
		}

		public string CreatorTitleWithValue
		{
			get;
			set;
		}

		public string AuditorTitle
		{
			get;
			set;
		}

		public string AuditorTitleWithValue
		{
			get;
			set;
		}

		public string Auditor
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string TotalTitleWithValue
		{
			get;
			set;
		}

		public string AttachmentsTitleWithValue
		{
			get;
			set;
		}

		public string CurrencyTitle
		{
			get;
			set;
		}

		public string ExchangeRateTitle
		{
			get;
			set;
		}

		public string AmountOfOriginalCurrencyTitle
		{
			get;
			set;
		}

		public string PageInfoTitle
		{
			get;
			set;
		}

		public string PageInfoTitleWithValue
		{
			get;
			set;
		}

		public string OrgTitle
		{
			get;
			set;
		}

		public string OrgTitleWithValue
		{
			get;
			set;
		}

		public string PageInfo
		{
			get;
			set;
		}

		public string DebitTotal
		{
			get;
			set;
		}

		public string CreditTotal
		{
			get;
			set;
		}

		[DataMember]
		public string AccountingDimensionTitle
		{
			get;
			set;
		}

		[DataMember]
		public VoucherListRowCollection VoucherListRows
		{
			get;
			set;
		}
	}
}
