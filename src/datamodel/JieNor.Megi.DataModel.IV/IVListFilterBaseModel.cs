using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVListFilterBaseModel : SqlWhere
	{
		[DataMember]
		public string BizObject
		{
			get;
			set;
		}
	}
}
