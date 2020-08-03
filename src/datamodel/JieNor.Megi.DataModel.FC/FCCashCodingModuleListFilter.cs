using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FC
{
	public class FCCashCodingModuleListFilter : SqlWhere
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
		public string MCode
		{
			get;
			set;
		}
	}
}
