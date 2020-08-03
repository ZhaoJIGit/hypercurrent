using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	public class BDAttachmentListFilterModel : SqlWhere
	{
		public string categoryId
		{
			get;
			set;
		}

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
