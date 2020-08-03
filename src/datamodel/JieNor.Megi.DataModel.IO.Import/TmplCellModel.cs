using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import
{
	[DataContract]
	public class TmplCellModel
	{
		[DataMember]
		public string Title
		{
			get;
			set;
		}

		[DataMember]
		public string FieldName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsRequired
		{
			get;
			set;
		}

		[DataMember]
		public int RowSpan
		{
			get;
			set;
		}

		[DataMember]
		public string CellSpanTitle
		{
			get;
			set;
		}

		[DataMember]
		public TmplCellDescType DescType
		{
			get;
			set;
		}
	}
}
