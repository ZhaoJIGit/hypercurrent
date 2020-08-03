using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FC
{
	[DataContract]
	public class FCFastCodeFilterModel : SqlWhere
	{
		[DataMember]
		public string Sort
		{
			get;
			set;
		}

		[DataMember]
		public string Order
		{
			get;
			set;
		}

		[DataMember]
		public string KeyWord
		{
			get;
			set;
		}

		[DataMember]
		public string MFastCode
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}
	}
}
