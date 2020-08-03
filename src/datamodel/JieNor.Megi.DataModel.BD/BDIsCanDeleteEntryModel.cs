using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDIsCanDeleteEntryModel
	{
		[DataMember]
		public string Id
		{
			get;
			set;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string ObjectName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCanDelete
		{
			get;
			set;
		}
	}
}
