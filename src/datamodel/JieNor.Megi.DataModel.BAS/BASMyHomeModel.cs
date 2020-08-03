using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASMyHomeModel
	{
		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgName
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleID
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleName
		{
			get;
			set;
		}

		[DataMember]
		public int MRegProgress
		{
			get;
			set;
		}

		[DataMember]
		public string MLastViewUserID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsPaid
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsBetaUser
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsBeta
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpiredDate
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
		public BASOrgVersionType MVersionType
		{
			get;
			set;
		}

		[DataMember]
		public string MLastViewUserName
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastViewDate
		{
			get;
			set;
		}

		[DataMember]
		public string MMasterID
		{
			get;
			set;
		}

		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		[DataMember]
		public string MAppID
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaulLocaleID
		{
			get;
			set;
		}

		[DataMember]
		public string Url
		{
			get;
			set;
		}

		[DataMember]
		public bool HasChangePermission
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsExpiredAlert
		{
			get;
			set;
		}

		[DataMember]
		public int MVersionID
		{
			get;
			set;
		}
	}
}
