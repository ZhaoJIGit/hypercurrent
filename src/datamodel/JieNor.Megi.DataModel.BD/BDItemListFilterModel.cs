using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDItemListFilterModel : SqlWhere
	{
		[DataMember]
		public string Keyword
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

		[DataMember]
		public bool IsActive
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeDisable
		{
			get;
			set;
		}
	}
}
