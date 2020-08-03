using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class SalesByItemRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Item
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentUnitPrice
		{
			get;
			set;
		}

		[DataMember]
		public decimal? QuantitySold
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
		public decimal? Total
		{
			get;
			set;
		}

		[DataMember]
		public string TotalCurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public decimal? AveragePrice
		{
			get;
			set;
		}

		[DataMember]
		public string AveragePriceCurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentUnitPriceStr
		{
			get;
			set;
		}

		[DataMember]
		public string QuantitySoldStr
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

		[DataMember]
		public string TotalStr
		{
			get;
			set;
		}

		[DataMember]
		public string AveragePriceStr
		{
			get;
			set;
		}
	}
}
