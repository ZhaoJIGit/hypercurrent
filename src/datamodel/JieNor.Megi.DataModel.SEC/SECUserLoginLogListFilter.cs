using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	public class SECUserLoginLogListFilter : SqlWhere
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
	}
}
