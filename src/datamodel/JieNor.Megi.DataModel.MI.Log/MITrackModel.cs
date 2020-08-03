using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI.Log
{
	[DataContract]
	public class MITrackModel : MigrateLogBaseModel
	{
		[DataMember]
		public string MMegiType
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiOption
		{
			get;
			set;
		}
	}
}
