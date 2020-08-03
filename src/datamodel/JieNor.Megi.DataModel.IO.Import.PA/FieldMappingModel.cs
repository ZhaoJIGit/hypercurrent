using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import.PA
{
	[DataContract]
	public class FieldMappingModel
	{
		[DataMember]
		public int DataRowStartIndex
		{
			get;
			set;
		}

		[DataMember]
		public int DataColumnStartIndex
		{
			get;
			set;
		}

		[DataMember]
		public string[] AllFieldList
		{
			get;
			set;
		}

		[DataMember]
		public string[] RequiredFieldList
		{
			get;
			set;
		}
	}
}
