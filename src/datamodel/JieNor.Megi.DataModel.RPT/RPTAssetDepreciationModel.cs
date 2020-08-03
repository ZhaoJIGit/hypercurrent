using JieNor.Megi.DataModel.GL;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTAssetDepreciationModel
	{
		[DataMember]
		public string MItemID
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
		public DateTime MDate
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
		public string MAccountParentID
		{
			get;
			set;
		}

		[DataMember]
		public string MNumberID
		{
			get;
			set;
		}

		public string MSummary
		{
			get;
			set;
		}

		public string MContactName
		{
			get;
			set;
		}

		public string MDC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNetAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPeriodDepreciatedAmount
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsLoadedCheckGroup
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOriginalAmount
		{
			get;
			set;
		}

		[DataMember]
		public int MYearPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginBalance
		{
			get;
			set;
		}

		[DataMember]
		public string VoucherDate
		{
			get;
			set;
		}

		[DataMember]
		public string VoucherNumber
		{
			get;
			set;
		}

		[DataMember]
		public string Explanation
		{
			get;
			set;
		}

		[DataMember]
		public string CheckGroupValue
		{
			get;
			set;
		}

		[DataMember]
		public decimal OriginDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal OriginCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal OrginEndBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal DepreciationDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal DepreciationCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal DepreciationEndBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal DepreciationReservesDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal DepreciationReservesCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal DepreciationReservesEndBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal NetAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal PrepareForDecreaseAmount
		{
			get;
			set;
		}

		[DataMember]
		public bool IsBalance
		{
			get;
			set;
		}

		[DataMember]
		public string GroupValueStr
		{
			get;
			set;
		}

		[DataMember]
		public string MDepCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MExpCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MFixCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MContactNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemGroupIDExp
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemGroupNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1NameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1GroupNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2NameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2GroupNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3NameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3GroupNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4NameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4GroupNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5NameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5GroupNameExp
		{
			get;
			set;
		}

		[DataMember]
		public string MContactIDExp
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeIDExp
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemIDExp
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemIDExp
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemIDExp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1Exp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2Exp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3Exp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4Exp
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5Exp
		{
			get;
			set;
		}
	}
}
