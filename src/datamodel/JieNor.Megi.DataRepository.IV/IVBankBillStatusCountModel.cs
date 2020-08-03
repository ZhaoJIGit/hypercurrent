using System.Runtime.Serialization;

namespace JieNor.Megi.DataRepository.IV
{
	[DataContract]
	public class IVBankBillStatusCountModel
	{
		[DataMember]
		public int NonGeneratedCount
		{
			get;
			set;
		}

		[DataMember]
		public int UnGenerateCount
		{
			get;
			set;
		}

		[DataMember]
		public int GeneratedCount
		{
			get;
			set;
		}

		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		public string MBankTypeID
		{
			get;
			set;
		}

		[DataMember]
		public int AllCount
		{
			get
			{
				return NonGeneratedCount + UnGenerateCount + GeneratedCount;
			}
			set
			{
			}
		}
	}
}
