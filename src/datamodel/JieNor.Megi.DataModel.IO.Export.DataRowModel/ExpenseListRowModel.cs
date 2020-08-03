using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class ExpenseListRowModel : ExportListBaseModel, ICloneable
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DisplayName("Employee")]
		[DataMember]
		public string MEmployee
		{
			get;
			set;
		}

		[DisplayName("Department")]
		[DataMember]
		public string MDepartment
		{
			get;
			set;
		}

		[DisplayName("Date")]
		[DataMember]
		public string MBizDate
		{
			get;
			set;
		}

		[DisplayName("DueDate")]
		[DataMember]
		public string MDueDate
		{
			get;
			set;
		}

		[DisplayName("ExpectedPaymentDate")]
		[DataMember]
		public string MExpectedDate
		{
			get;
			set;
		}

		[DataMember]
		public string Status
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
		public decimal? Paid
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Due
		{
			get;
			set;
		}

		[DataMember]
		public string Currency
		{
			get;
			set;
		}

		[DisplayName("Reference")]
		[DataMember]
		public string MReference
		{
			get;
			set;
		}

		[DataMember]
		public string Claimer
		{
			get;
			set;
		}

		[DataMember]
		public string Auditor
		{
			get;
			set;
		}

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
		public decimal? MQty
		{
			get;
			set;
		}

		[DisplayName("Price")]
		[DataMember]
		public decimal? MPrice
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
