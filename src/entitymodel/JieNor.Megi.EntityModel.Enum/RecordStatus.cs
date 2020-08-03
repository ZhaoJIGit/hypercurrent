using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum RecordStatus
	{
		[DataMember]
		Empty = -2,
		[DataMember]
		Draft,
		[DataMember]
		Saved,
		[DataMember]
		Approved
	}
}
