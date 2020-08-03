using JieNor.Megi.DataModel.GL;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTDepreciationBaseModel
	{
		public string MitemId
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
		public GLCheckGroupValueModel MCheckGroupValueModel
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
		public DateTime MHandledDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDepreciationFromPeriod
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherID
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
		public DateTime MChangeFromPeriod
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCreateDate
		{
			get;
			set;
		}

		[DataMember]
		public string MCreatorID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MModifyDate
		{
			get;
			set;
		}

		[DataMember]
		public string MModifierID
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
		public string MContactNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemGroupIDDep
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemGroupNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1NameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1GroupNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2NameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2GroupNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3NameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3GroupNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4NameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4GroupNameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5NameDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5GroupNameDep
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

		[DataMember]
		public string MContactIDDep
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeIDDep
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemIDDep
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemIDDep
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemIDDep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1Dep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2Dep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3Dep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4Dep
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5Dep
		{
			get;
			set;
		}
	}
}
