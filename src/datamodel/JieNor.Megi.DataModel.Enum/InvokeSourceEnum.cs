using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.Enum
{
	[DataContract]
	public enum InvokeSourceEnum
	{
		Import = 1,
		API,
		Excel
	}
}
