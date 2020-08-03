using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class InvoiceEntryModel : BaseReportRowModel
	{
		[DataMember]
		[DisplayName("Number")]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Description")]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Quantity")]
		public string MQty
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Price")]
		public string MPrice
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Discount")]
		public string MDiscount
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Tax Rate")]
		public string MTaxID
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Track Item1")]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Track Item2")]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Track Item3")]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Track Item4")]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Track Item5")]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Amount")]
		public string MAmountFor
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("TaxAmount")]
		public string MTaxAmtFor
		{
			get;
			set;
		}
	}
}
