using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.SEC
{
	[DataContract]
	public class SECLoginResultModel
	{
		[DataMember]
		public List<string> MActiveLocaleIDS = new List<string>();

		[DataMember]
		public bool IsSuccess
		{
			get;
			set;
		}

		[DataMember]
		public string Message
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
		public string MAccessToken
		{
			get;
			set;
		}

		[DataMember]
		public string MWhenILogIn
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastLoginDate
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginOrgCode
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginOrgName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginAppCode
		{
			get;
			set;
		}

		[DataMember]
		public bool MExistsOrg
		{
			get;
			set;
		}

		[DataMember]
		public string MFirstName
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
		public string MMobilePhone
		{
			get;
			set;
		}

		[DataMember]
		public string MUserName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsUserInvite
		{
			get;
			set;
		}

		[DataMember]
		public string MRedirectURL
		{
			get;
			set;
		}

		[DataMember]
		public bool MRelogin
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
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public int MLoginErrorCount
		{
			get;
			set;
		}

		[DataMember]
		public string MBrowserTabIndex
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsDeletedOrgID
		{
			get;
			set;
		}
	}
}
