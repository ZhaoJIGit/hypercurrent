using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECPermisionGrpOperateModel
	{
		[DataMember]
		public string GroupID
		{
			get;
			set;
		}

		[DataMember]
		public bool View
		{
			get;
			set;
		}

		[DataMember]
		public bool Change
		{
			get;
			set;
		}

		[DataMember]
		public bool Approve
		{
			get;
			set;
		}

		[DataMember]
		public bool Export
		{
			get;
			set;
		}
	}
}
