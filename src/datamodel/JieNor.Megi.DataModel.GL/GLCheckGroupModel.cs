using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLCheckGroupModel : BDModel, IEqualityComparer<GLCheckGroupModel>, ICloneable
	{
		[DataMember]
		[ApiMember("ContactID")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MContactID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("EmployeeID")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MEmployeeID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MerItemID")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MMerItemID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ExpItemID")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MExpItemID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PaItemID")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MPaItemID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem1")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem2")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem3")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem4")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem5")]
		[ApiEnum(EnumMappingType.CheckTypeStatus)]
		public int MTrackItem5
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

		public GLCheckGroupModel()
			: base("T_GL_CHECKGROUP")
		{
		}

		public GLCheckGroupModel(string tableName)
			: base(tableName)
		{
		}

		public int GetHashCode(GLCheckGroupModel obj)
		{
			int num = obj.MContactID;
			int hashCode = num.GetHashCode();
			num = obj.MEmployeeID;
			int hashCode2 = num.GetHashCode();
			num = obj.MMerItemID;
			int hashCode3 = num.GetHashCode();
			num = obj.MExpItemID;
			int hashCode4 = num.GetHashCode();
			num = obj.MPaItemID;
			int hashCode5 = num.GetHashCode();
			num = obj.MTrackItem1;
			int hashCode6 = num.GetHashCode();
			num = obj.MTrackItem2;
			int hashCode7 = num.GetHashCode();
			num = obj.MTrackItem3;
			int hashCode8 = num.GetHashCode();
			num = obj.MTrackItem4;
			int hashCode9 = num.GetHashCode();
			num = obj.MTrackItem5;
			int hashCode10 = num.GetHashCode();
			return hashCode ^ hashCode2 ^ hashCode3 ^ hashCode4 ^ hashCode5 ^ hashCode6 ^ hashCode7 ^ hashCode8 ^ hashCode9 ^ hashCode10;
		}

		public bool Equals(GLCheckGroupModel x, GLCheckGroupModel y)
		{
			if (y == null && x == null)
			{
				return true;
			}
			if (x == y)
			{
				return true;
			}
			return x.MContactID == y.MContactID && x.MEmployeeID == y.MEmployeeID && x.MMerItemID == y.MMerItemID && x.MExpItemID == y.MExpItemID && x.MPaItemID == y.MPaItemID && x.MTrackItem1 == y.MTrackItem1 && x.MTrackItem2 == y.MTrackItem2 && x.MTrackItem3 == y.MTrackItem3 && x.MTrackItem4 == y.MTrackItem4 && x.MTrackItem5 == y.MTrackItem5;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
