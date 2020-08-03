using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI.Log
{
	[DataContract]
	public class MIContactModel : MigrateLogBaseModel
	{
		[DataMember]
		public string MMegiContactType
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiContactName
		{
			get;
			set;
		}
	}
}
