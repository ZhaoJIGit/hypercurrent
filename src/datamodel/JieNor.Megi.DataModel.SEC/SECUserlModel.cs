using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECUserlModel : BDModel
	{
		[DataMember]
		public string MPKID
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
		public string MLocaleID
		{
			get;
			set;
		}

		[DataMember]
		public string MFristName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MJobTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MBriefBio
		{
			get;
			set;
		}

		[DataMember]
		public string ItemId
		{
			get;
			set;
		}

		[DataMember]
		public string MProfileImage
		{
			get;
			set;
		}

		[DataMember]
		public string MMobilePhone
		{
			get;
			set;
		}

		public SECUserlModel()
			: base("T_Sec_User_L")
		{
		}
	}
}
