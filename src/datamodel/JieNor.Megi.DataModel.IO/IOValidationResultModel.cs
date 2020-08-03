using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO
{
	[DataContract]
	public class IOValidationResultModel
	{
		[DataMember]
		public int RowIndex
		{
			get;
			set;
		}

		[DataMember]
		public IOValidationTypeEnum FieldType
		{
			get;
			set;
		}

		[DataMember]
		public string FieldValue
		{
			get;
			set;
		}

		[DataMember]
		public string Message
		{
			get;
			set;
		}

		[DataMember]
		public string Id
		{
			get;
			set;
		}
	}
}
