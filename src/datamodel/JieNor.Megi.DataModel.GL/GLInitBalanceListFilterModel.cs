using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	public class GLInitBalanceListFilterModel : SqlWhere
	{
		[DataMember]
		public string AccountID
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeCheckTypeData
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeInitBalance
		{
			get;
			set;
		}

		[DataMember]
		public bool IsExport
		{
			get;
			set;
		}
	}
}
