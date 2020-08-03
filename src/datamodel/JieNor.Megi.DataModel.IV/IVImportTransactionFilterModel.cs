using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVImportTransactionFilterModel
	{
		[DataMember]
		public string BizType
		{
			get;
			set;
		}

		[DataMember]
		public string AccountId
		{
			get;
			set;
		}

		[DataMember]
		public string ContactType
		{
			get;
			set;
		}
	}
}
