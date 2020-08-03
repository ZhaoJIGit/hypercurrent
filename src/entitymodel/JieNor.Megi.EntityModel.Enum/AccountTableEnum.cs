using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public class AccountTableEnum
	{
		[DataMember]
		public const string CEAS = "1";

		[DataMember]
		public const string CASS = "2";

		[DataMember]
		public const string UCAS = "3";
	}
}
