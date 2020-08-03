using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI.Log
{
	[DataContract]
	public class MICurrencyModel : MigrateLogBaseModel
	{
		[DataMember]
		public string MCurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}
	}
}
