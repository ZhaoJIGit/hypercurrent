using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.LOG
{
	[DataContract]
	public class MLog
	{
		[DataMember]
		public string Id
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDate
		{
			get;
			set;
		}

		public string MTable
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
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MOperateType
		{
			get;
			set;
		}

		[DataMember]
		public string MSql
		{
			get;
			set;
		}

		[DataMember]
		public string MParameter
		{
			get;
			set;
		}

		[DataMember]
		public string MDatabase
		{
			get;
			set;
		}
	}
}
