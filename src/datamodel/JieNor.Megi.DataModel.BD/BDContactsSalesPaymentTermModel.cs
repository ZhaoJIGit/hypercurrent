using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsSalesPaymentTermModel
	{
		[DataMember]
		[ApiMember("Day")]
		[DBField("MSalDueDate")]
		public int MDay
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("DayType")]
		[ApiEnum(EnumMappingType.RecOrPayCondition)]
		public string MDayType
		{
			get;
			set;
		}

		public List<string> UpdateFieldList
		{
			get;
			set;
		}
	}
}
