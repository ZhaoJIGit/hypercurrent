using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import
{
	[DataContract]
	public enum TmplCellDescType
	{
		[EnumMember]
		None,
		[EnumMember]
		CantManualInput,
		[EnumMember]
		NeedManualInput,
		[EnumMember]
		AutoObtainCanEdit,
		[EnumMember]
		AutoCalculateCanEdit,
		[EnumMember]
		AutoCalculateCantEdit
	}
}
