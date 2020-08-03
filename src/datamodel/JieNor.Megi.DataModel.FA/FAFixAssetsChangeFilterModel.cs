using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FA
{
	public class FAFixAssetsChangeFilterModel : SqlWhere
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}
	}
}
