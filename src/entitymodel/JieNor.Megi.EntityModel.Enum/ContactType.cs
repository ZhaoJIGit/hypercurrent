using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public class ContactType
	{
		[DataMember]
		public const string Customer = "Customer";

		[DataMember]
		public const string Supplier = "Supplier";

		[DataMember]
		public const string Employees = "Employees";

		[DataMember]
		public const string Other = "Other";
	}
}
