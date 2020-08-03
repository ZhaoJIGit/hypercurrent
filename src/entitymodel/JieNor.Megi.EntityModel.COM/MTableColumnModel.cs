using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.COM
{
	[DataContract]
	public class MTableColumnModel
	{
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string Type
		{
			get;
			set;
		}

		[DataMember]
		public int MaxLength
		{
			get;
			set;
		}

		[DataMember]
		public int DecimalMaxLength
		{
			get;
			set;
		}

		[DataMember]
		public int MaxValue
		{
			get;
			set;
		}
	}
}
