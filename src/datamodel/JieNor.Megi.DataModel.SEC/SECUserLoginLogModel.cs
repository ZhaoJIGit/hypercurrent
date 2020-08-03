using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECUserLoginLogModel : BDModel
	{
		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLoginDate
		{
			get;
			set;
		}

		[DataMember]
		public string MFullName
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

		public SECUserLoginLogModel()
			: base("T_User_LoginLog")
		{
		}
	}
}
