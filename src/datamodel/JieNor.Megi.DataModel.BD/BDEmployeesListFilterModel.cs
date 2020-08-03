using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDEmployeesListFilterModel : SqlWhere
	{
		[DataMember]
		public string searchFilter
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFromExport
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
		public string IsActive
		{
			get;
			set;
		}
	}
}
