using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLCheckGroupValueModel : BDModel, IEqualityComparer<GLCheckGroupValueModel>
	{
		[Serializable]
		public class GLBillAmountInfo
		{
			[DataMember]
			public decimal MAmount
			{
				get;
				set;
			}

			[DataMember]
			public decimal MAmountFor
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

			public int MDir
			{
				get;
				set;
			}
		}

		[DataMember]
		[ApiMember("MContactID")]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public int MContactType
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MEmployeeID")]
		public string MEmployeeID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MMerItemID")]
		public string MMerItemID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MExpItemID")]
		public string MExpItemID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MPaItemID")]
		public string MPaItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemGroupID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MTrackItem1")]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MTrackItem2")]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MTrackItem3")]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MTrackItem4")]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MTrackItem5")]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1GroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2GroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3GroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4GroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5GroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MContactName
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeName
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemName
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemName
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemName
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemGroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1GroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2GroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3GroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4GroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5GroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MContactIDName
		{
			get
			{
				return MContactName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MEmployeeIDName
		{
			get
			{
				return MEmployeeName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MMerItemIDName
		{
			get
			{
				return MMerItemName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MExpItemIDName
		{
			get
			{
				return MExpItemName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MPaItemIDName
		{
			get
			{
				return MPaItemName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MContactIDTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeIDTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemIDTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemIDTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemIDTitle
		{
			get
			{
				return MPaItemGroupName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MTrackItem1Title
		{
			get
			{
				return MTrackItem1GroupName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MTrackItem2Title
		{
			get
			{
				return MTrackItem2GroupName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MTrackItem3Title
		{
			get
			{
				return MTrackItem3GroupName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MTrackItem4Title
		{
			get
			{
				return MTrackItem4GroupName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MTrackItem5Title
		{
			get
			{
				return MTrackItem5GroupName;
			}
			set
			{
			}
		}

		public string MAccountCode
		{
			get;
			set;
		}

		public string MAccountID
		{
			get;
			set;
		}

		public GLBillAmountInfo MBillAmountInfo
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmountFor
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

		public string CheckGroupCombinationName
		{
			get
			{
				string text = "";
				if (!string.IsNullOrWhiteSpace(MContactName))
				{
					text = text + MContactName + "-";
				}
				if (!string.IsNullOrWhiteSpace(MEmployeeName))
				{
					text = text + MEmployeeName + "-";
				}
				if (!string.IsNullOrWhiteSpace(MMerItemName))
				{
					text = text + MMerItemName + "-";
				}
				if (!string.IsNullOrWhiteSpace(MExpItemName))
				{
					text = text + MExpItemName + "-";
				}
				if (!string.IsNullOrWhiteSpace(MPaItemGroupName))
				{
					text = text + MPaItemGroupName + "-";
				}
				if (!string.IsNullOrWhiteSpace(MPaItemName))
				{
					text = text + MPaItemName + "-";
				}
				if (!string.IsNullOrWhiteSpace(MTrackItem1Name))
				{
					text = text + MTrackItem1Name + "-";
				}
				if (!string.IsNullOrWhiteSpace(MTrackItem2Name))
				{
					text = text + MTrackItem2Name + "-";
				}
				if (!string.IsNullOrWhiteSpace(MTrackItem3Name))
				{
					text = text + MTrackItem3Name + "-";
				}
				if (!string.IsNullOrWhiteSpace(MTrackItem4Name))
				{
					text = text + MTrackItem4Name + "-";
				}
				if (!string.IsNullOrWhiteSpace(MTrackItem5Name))
				{
					text = text + MTrackItem5Name + "-";
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					text = text.Substring(0, text.Length - 1);
				}
				return text;
			}
		}

		public GLCheckGroupValueModel()
			: base("T_GL_CHECKGROUPVALUE")
		{
		}

		public GLCheckGroupValueModel(string tableName)
			: base(tableName)
		{
		}

		public bool Equals(GLCheckGroupValueModel x, GLCheckGroupValueModel y)
		{
			if (y == null && x == null)
			{
				return true;
			}
			if (x == y)
			{
				return true;
			}
			return (x.MOrgID ?? string.Empty) == (y.MOrgID ?? string.Empty) && (x.MContactID ?? string.Empty) == (y.MContactID ?? string.Empty) && (x.MEmployeeID ?? string.Empty) == (y.MEmployeeID ?? string.Empty) && (x.MMerItemID ?? string.Empty) == (y.MMerItemID ?? string.Empty) && (x.MExpItemID ?? string.Empty) == (y.MExpItemID ?? string.Empty) && (x.MPaItemID ?? string.Empty) == (y.MPaItemID ?? string.Empty) && (x.MTrackItem1 ?? string.Empty) == (y.MTrackItem1 ?? string.Empty) && (x.MTrackItem2 ?? string.Empty) == (y.MTrackItem2 ?? string.Empty) && (x.MTrackItem3 ?? string.Empty) == (y.MTrackItem3 ?? string.Empty) && (x.MTrackItem4 ?? string.Empty) == (y.MTrackItem4 ?? string.Empty) && (x.MTrackItem5 ?? string.Empty) == (y.MTrackItem5 ?? string.Empty);
		}

		public int GetHashCode(GLCheckGroupValueModel obj)
		{
			int num = (obj.MOrgID != null) ? obj.MOrgID.GetHashCode() : 0;
			int num2 = (obj.MContactID != null) ? obj.MContactID.GetHashCode() : 0;
			int num3 = (obj.MEmployeeID != null) ? obj.MEmployeeID.GetHashCode() : 0;
			int num4 = (obj.MMerItemID != null) ? obj.MMerItemID.GetHashCode() : 0;
			int num5 = (obj.MExpItemID != null) ? obj.MExpItemID.GetHashCode() : 0;
			int num6 = (obj.MPaItemID != null) ? obj.MPaItemID.GetHashCode() : 0;
			int num7 = (obj.MTrackItem1 != null) ? obj.MTrackItem1.GetHashCode() : 0;
			int num8 = (obj.MTrackItem2 != null) ? obj.MTrackItem2.GetHashCode() : 0;
			int num9 = (obj.MTrackItem3 != null) ? obj.MTrackItem3.GetHashCode() : 0;
			int num10 = (obj.MTrackItem4 != null) ? obj.MTrackItem4.GetHashCode() : 0;
			int num11 = (obj.MTrackItem5 != null) ? obj.MTrackItem5.GetHashCode() : 0;
			return num ^ num2 ^ num3 ^ num4 ^ num5 ^ num6 ^ num7 ^ num8 ^ num9 ^ num10 ^ num11;
		}

		public override int GetHashCode()
		{
			int num = (base.MOrgID != null) ? base.MOrgID.GetHashCode() : 0;
			int num2 = (MContactID != null) ? MContactID.GetHashCode() : 0;
			int num3 = (MEmployeeID != null) ? MEmployeeID.GetHashCode() : 0;
			int num4 = (MMerItemID != null) ? MMerItemID.GetHashCode() : 0;
			int num5 = (MExpItemID != null) ? MExpItemID.GetHashCode() : 0;
			int num6 = (MPaItemID != null) ? MPaItemID.GetHashCode() : 0;
			int num7 = (MTrackItem1 != null) ? MTrackItem1.GetHashCode() : 0;
			int num8 = (MTrackItem2 != null) ? MTrackItem2.GetHashCode() : 0;
			int num9 = (MTrackItem3 != null) ? MTrackItem3.GetHashCode() : 0;
			int num10 = (MTrackItem4 != null) ? MTrackItem4.GetHashCode() : 0;
			int num11 = (MTrackItem5 != null) ? MTrackItem5.GetHashCode() : 0;
			return num ^ num2 ^ num3 ^ num4 ^ num5 ^ num6 ^ num7 ^ num8 ^ num9 ^ num10 ^ num11;
		}
	}
}
