using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class SYSOrgAppModel : BDEntryModel
	{
		[DataMember]
		public string MAppID
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
		public string MUsedStatusID
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
		public DateTime MLastViewDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpireDate
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

		public SYSOrgAppModel()
			: base("T_SYS_OrgApp")
		{
		}
	}
}
