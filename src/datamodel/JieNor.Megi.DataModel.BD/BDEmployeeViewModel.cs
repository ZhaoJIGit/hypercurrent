using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDEmployeeViewModel
	{
		[DataMember]
		public string ColumnName
		{
			get;
			set;
		}

		[DataMember]
		public string ColumnTitle
		{
			get;
			set;
		}
	}
}
