using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASPushMessageModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MMessageType
		{
			get;
			set;
		}

		[DataMember]
		public string MContent
		{
			get;
			set;
		}

		[DataMember]
		public string MCreator
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MStartPushDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndPushDate
		{
			get;
			set;
		}

		[DataMember]
		public int MPushCount
		{
			get;
			set;
		}

		[DataMember]
		public int MInterval
		{
			get;
			set;
		}

		[DataMember]
		public int MCompleteCount
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastPushDate
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
	}
}
