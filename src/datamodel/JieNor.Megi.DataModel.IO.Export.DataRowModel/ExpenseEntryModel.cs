using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class ExpenseEntryModel : ICloneable
	{
		[DisplayName("ExpenseItem")]
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("TrackItem1")]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DisplayName("TrackItem2")]
		[DataMember]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DisplayName("TrackItem3")]
		[DataMember]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DisplayName("TrackItem4")]
		[DataMember]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DisplayName("TrackItem5")]
		[DataMember]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DisplayName("Quantity")]
		[DataMember]
		public string MQty
		{
			get;
			set;
		}

		[DisplayName("Price")]
		[DataMember]
		public string MPrice
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Amount
		{
			get;
			set;
		}

		[DataMember]
		public decimal? TaxAmount
		{
			get;
			set;
		}

		[DisplayName("Description")]
		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
