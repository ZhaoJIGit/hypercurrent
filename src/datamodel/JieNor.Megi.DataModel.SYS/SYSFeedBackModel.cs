using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSFeedBackModel : BDModel
	{
		[DataMember]
		public string MModule
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
		public string MFeedContact
		{
			get;
			set;
		}

		[DataMember]
		public string MFeedEmail
		{
			get;
			set;
		}

		[DataMember]
		public string MFeedTelphone
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		public SYSFeedBackModel()
			: base("T_Sys_FeedBack")
		{
		}
	}
}
