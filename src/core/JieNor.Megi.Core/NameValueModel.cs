using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class NameValueModel
	{
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MValue
		{
			get;
			set;
		}

		[DataMember]
		public string MValue1
		{
			get;
			set;
		}

		[DataMember]
		public string MValue2
		{
			get;
			set;
		}

		[DataMember]
		public string MTag
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> MChildren
		{
			get;
			set;
		}
	}
}
