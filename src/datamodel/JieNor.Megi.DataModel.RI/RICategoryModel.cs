using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RI
{
	[DataContract]
	public class RICategoryModel : BDModel
	{
		[DataMember]
		public string MPermission
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public bool MEnable
		{
			get;
			set;
		}

		[DataMember]
		public bool MRequirePass
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsParent
		{
			get
			{
				return MParentID == "0";
			}
			set
			{
			}
		}

		[DataMember]
		public string MLinkUrl
		{
			get;
			set;
		}

		[DataMember]
		public int MIndex
		{
			get;
			set;
		}

		[DataMember]
		public string MFuncName
		{
			get;
			set;
		}

		[DataMember]
		public string MPassText
		{
			get;
			set;
		}

		[DataMember]
		public string MFailText
		{
			get;
			set;
		}

		[DataMember]
		public string MPassTextString
		{
			get;
			set;
		}

		[DataMember]
		public string MFailTextString
		{
			get;
			set;
		}

		[DataMember]
		public string MName
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
		public RICategorySettingModel MSetting
		{
			get;
			set;
		}

		public RICategoryModel()
			: base("T_RI_Category")
		{
		}

		public RICategoryModel(string tableName)
			: base(tableName)
		{
		}
	}
}
