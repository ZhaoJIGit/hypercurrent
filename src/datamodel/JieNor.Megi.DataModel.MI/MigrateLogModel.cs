using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI
{
	[DataContract]
	public class MigrateLogModel : MigrateLogBaseModel
	{
		[DataMember]
		public string MSourceCheckType
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceCurrency
		{
			get;
			set;
		}

		[DataMember]
		public string MMatchNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MNewNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckType
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCheckForCurrency
		{
			get;
			set;
		}

		[DataMember]
		public List<string> TrackIdList
		{
			get;
			set;
		}
	}
}
