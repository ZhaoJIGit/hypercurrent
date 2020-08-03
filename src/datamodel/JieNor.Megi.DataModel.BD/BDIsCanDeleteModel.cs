using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDIsCanDeleteModel
	{
		[DataMember]
		public bool Success
		{
			get;
			set;
		}

		[DataMember]
		public bool AllSuccess
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
		public string BizObjectName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsDelete
		{
			get;
			set;
		}

		[DataMember]
		public List<BDIsCanDeleteEntryModel> EntryList
		{
			get;
			set;
		}
	}
}
