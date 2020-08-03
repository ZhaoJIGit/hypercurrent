using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import
{
	[DataContract]
	public enum IOTemplateFieldType
	{
		[EnumMember]
		Text = 1,
		[EnumMember]
		Decimal,
		[EnumMember]
		Date
	}
}
