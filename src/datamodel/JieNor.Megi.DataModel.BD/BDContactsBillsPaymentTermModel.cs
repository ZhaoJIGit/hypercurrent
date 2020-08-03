using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsBillsPaymentTermModel
	{
		[DataMember]
		[ApiMember("Day")]
		[DBField("MPurDueDate")]
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
