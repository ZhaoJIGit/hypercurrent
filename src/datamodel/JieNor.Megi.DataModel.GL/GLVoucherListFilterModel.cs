using JieNor.Megi.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLVoucherListFilterModel : SqlWhere
	{
		[DataMember]
		public int Year
		{
			get;
			set;
		}

		[DataMember]
		public int Period
		{
			get;
			set;
		}

		[DataMember]
		public string AccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string AccountTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string KeyWord
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DecimalKeyWord
		{
			get;
			set;
		}

		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MFastCode
		{
			get;
			set;
		}

		[DataMember]
		public string MDescription
		{
			get;
			set;
		}

		[DataMember]
		public string MIsMulti
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
		public string From
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public int StartYear
		{
			get;
			set;
		}

		[DataMember]
		public int EndYear
		{
			get;
			set;
		}

		[DataMember]
		public int StartPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int EndPeriod
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
		public string MTransferTypeID
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeDraft
		{
			get;
			set;
		}

		[DataMember]
		public List<string> AccountIDList
		{
			get;
			set;
		}

		[DataMember]
		public int MStartYearPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int MEndYearPeriod
		{
			get;
			set;
		}

		[DataMember]
		public string CheckGroupValueId
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
		public bool IncludeCheckType
		{
			get;
			set;
		}

		[DataMember]
		public bool SingleModule
		{
			get;
			set;
		}

		[DataMember]
		public int SortByType
		{
			get;
			set;
		}

		[DataMember]
		public int SortType
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> CheckTypeValueList
		{
			get;
			set;
		}
	}
}
