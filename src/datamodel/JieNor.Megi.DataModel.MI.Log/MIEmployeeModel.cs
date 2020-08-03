using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI.Log
{
	[DataContract]
	public class MIEmployeeModel : MigrateLogBaseModel
	{
		[DataMember]
		public string MMegiLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiFirstName
		{
			get;
			set;
		}
	}
}
