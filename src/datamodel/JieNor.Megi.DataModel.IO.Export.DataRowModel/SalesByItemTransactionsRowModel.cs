using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class SalesByItemTransactionsRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Date
		{
			get;
			set;
		}

		[DataMember]
		public string To
		{
			get;
			set;
		}

		[DataMember]
		public string Description
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Quantity
		{
			get;
			set;
		}

		[DataMember]
		public decimal? UnitPrice
		{
			get;
			set;
		}

		[DataMember]
		public string UnitPriceCurrency
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Disc
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Total
		{
			get;
			set;
		}

		[DataMember]
		public string TotalCurrency
		{
			get;
			set;
		}

		[DataMember]
		public decimal? TotalCNY
		{
			get;
			set;
		}

		[DataMember]
		public string QuantityStr
		{
			get;
			set;
		}

		[DataMember]
		public string UnitPriceStr
		{
			get;
			set;
		}

		[DataMember]
		public string DiscStr
		{
			get;
			set;
		}

		[DataMember]
		public string TotalStr
		{
			get;
			set;
		}

		[DataMember]
		public string TotalCNYStr
		{
			get;
			set;
		}
	}
}
