using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTBankAndCashSummaryFilterBaseModel : ReportFilterBase
	{
		[DataMember]
		public string MAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public bool IsShowDetail
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRate
		{
			get;
			set;
		}

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal OpeningBlance
		{
			get;
			set;
		}

		[DataMember]
		public bool ShowInUSD
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

		public RPTBankAndCashSummaryFilterBaseModel()
		{
			MEndDate = DateTime.Now;
		}
	}
}
