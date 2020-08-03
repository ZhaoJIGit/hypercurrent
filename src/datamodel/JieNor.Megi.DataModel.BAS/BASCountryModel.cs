using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASCountryModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MCountryName
		{
			get;
			set;
		}

		[DataMember]
		public string MContinentName
		{
			get;
			set;
		}
	}
}
