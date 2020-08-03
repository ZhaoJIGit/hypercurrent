using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDTrackEntryModel : BDEntryModel
	{
		[DataMember]
		public bool __active__ = true;

		[DataMember]
		[ApiMember("TrackingCategoryID", true, IgnoreInGet = true)]
		public string MTrackingCategoryID
		{
			get
			{
				return base.MItemID;
			}
			set
			{
				base.MItemID = value;
			}
		}

		[DataMember]
		[ApiMember("TrackingOptionID", IsPKField = true)]
		public string MTrackingOptionID
		{
			get
			{
				return base.MEntryID;
			}
			set
			{
				base.MEntryID = value;
			}
		}

		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, false, false)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get
			{
				return __active__;
			}
			set
			{
				__active__ = value;
			}
		}

		[DataMember]
		[ApiMember("Status")]
		public string MStatus
		{
			get
			{
				return MIsActive ? "ACTIVE" : "DISABLED";
			}
			set
			{
				MIsActive = (value == "ACTIVE" || value == "1" || string.IsNullOrWhiteSpace(value));
			}
		}

		public BDTrackEntryModel()
			: base("T_BD_TrackEntry")
		{
		}
	}
}
