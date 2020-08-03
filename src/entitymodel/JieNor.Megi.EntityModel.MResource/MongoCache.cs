using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.MResource
{
	[DataContract]
	public class MongoCache
	{
		[DataMember]
		public string MCacheKey
		{
			get;
			set;
		}

		[DataMember]
		public string Data
		{
			get;
			set;
		}
	}
}
