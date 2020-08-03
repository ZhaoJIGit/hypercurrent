using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	public class REGTaxTateListFilterModel : SqlWhere
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
		public string SearchFilter
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get;
			set;
		} = true;

	}
}
