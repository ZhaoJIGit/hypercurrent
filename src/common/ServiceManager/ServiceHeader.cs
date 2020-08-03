using System.Runtime.Serialization;

namespace JieNor.Megi.Common.ServiceManager
{
	[DataContract]
	public class ServiceHeader
	{
		[DataMember]
		public string MAccessToken
		{
			get;
			set;
		}
	}
}
