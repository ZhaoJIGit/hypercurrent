using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASTimezoneModel
	{
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MLocalName
		{
			get;
			set;
		}
	}
}
