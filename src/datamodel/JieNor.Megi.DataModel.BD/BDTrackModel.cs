using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDTrackModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		[ApiMember("TrackingCategoryID", IsPKField = true, IgnoreLengthValidate = true)]
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
		[ApiMember("TrackingOptionID", IsReference = true, IgnoreInGet = true)]
		public string MTrackingOptionID
		{
			get;
			set;
		}

		[DataMember]
		public string MPKID
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
		public string MEntryPKID
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryName
		{
			get;
			set;
		}

		[DataMember]
		[ModelEntry]
		[ApiDetail]
		[ApiMember("Options")]
		public List<BDTrackEntryModel> MEntryList
		{
			get;
			set;
		}

		[DataMember]
		public string MLocaleID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		public BDTrackModel()
			: base("T_BD_Track")
		{
		}

		public void AddEntry(object obj)
		{
			BDTrackEntryModel item = obj as BDTrackEntryModel;
			if (obj != null)
			{
				if (MEntryList == null)
				{
					MEntryList = new List<BDTrackEntryModel>();
				}
				MEntryList.Add(item);
			}
		}
	}
}
