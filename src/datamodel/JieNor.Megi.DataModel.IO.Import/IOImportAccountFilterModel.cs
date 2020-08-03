using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import
{
	[DataContract]
	public class IOImportAccountFilterModel
	{
		[DataMember]
		public bool IsCover
		{
			get;
			set;
		}

		[DataMember]
		public int Type
		{
			get;
			set;
		}
	}
}
