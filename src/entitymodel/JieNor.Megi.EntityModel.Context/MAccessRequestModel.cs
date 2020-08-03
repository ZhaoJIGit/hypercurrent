using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Context
{
	[DataContract]
	public class MAccessRequestModel
	{
		[DataMember]
		public int RequestType = 2;

		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string BizModule
		{
			get;
			set;
		}

		[DataMember]
		public string BizAccess
		{
			get;
			set;
		}

		public MAccessRequestModel and(MAccessRequestModel other)
		{
			return this;
		}

		public MAccessRequestModel or(MAccessRequestModel other)
		{
			return this;
		}
	}
}
