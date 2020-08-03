using JieNor.Megi.Core;
using JieNor.Megi.EntityModel.Context;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECUserPermissionListFilterModel : SqlWhere
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
		public MContext MContext
		{
			get;
			set;
		}
	}
}
