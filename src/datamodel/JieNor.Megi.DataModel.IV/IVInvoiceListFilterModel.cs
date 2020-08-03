using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVInvoiceListFilterModel : IVListFilterBaseModel
	{
		[DataMember]
		public IVInvoiceSearchWithinEnum MSearchWithin
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public bool MUnsentOnly
		{
			get;
			set;
		}

		[DataMember]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public string Keyword
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsOnlyInitData
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MConversionDate
		{
			get;
			set;
		}

		[DataMember]
		public string SelectedIds
		{
			get;
			set;
		}

		[DataMember]
		public string Sort
		{
			get;
			set;
		}

		[DataMember]
		public string Order
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFromExcel
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFromExport
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFromFapiao
		{
			get;
			set;
		}

		[DataMember]
		public int MIssueStatus
		{
			get;
			set;
		}

		[DataMember]
		public string ApiModule
		{
			get;
			set;
		}
	}
}
