using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrgContactModel : BDModel
	{
		[DataMember]
		public string MTelephone
		{
			get;
			set;
		}

		[DataMember]
		public string MFax
		{
			get;
			set;
		}

		[DataMember]
		public string MMobile
		{
			get;
			set;
		}

		[DataMember]
		public string MDDI
		{
			get;
			set;
		}

		[DataMember]
		public string MEmail
		{
			get;
			set;
		}

		[DataMember]
		public string MWebsite
		{
			get;
			set;
		}

		[DataMember]
		public string MTBShop
		{
			get;
			set;
		}

		[DataMember]
		public string MSkype
		{
			get;
			set;
		}

		[DataMember]
		public string MTwitter
		{
			get;
			set;
		}

		[DataMember]
		public string MLinkedIn
		{
			get;
			set;
		}

		[DataMember]
		public string MFacebook
		{
			get;
			set;
		}

		[DataMember]
		public string MGooglePlus
		{
			get;
			set;
		}

		[DataMember]
		public string MQQ
		{
			get;
			set;
		}

		[DataMember]
		public string MWeChat
		{
			get;
			set;
		}

		[DataMember]
		public string MWeibo
		{
			get;
			set;
		}

		public BASOrgContactModel()
			: base("T_Bas_OrgContact")
		{
		}
	}
}
