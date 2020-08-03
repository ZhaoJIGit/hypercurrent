using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrgRegistrationInfo
	{
		[DataMember]
		public string OrgName
		{
			get;
			set;
		}

		[DataMember]
		public string OrgPayTaxCountry
		{
			get;
			set;
		}
	}
}
