using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class OrgDataStorageModel : BizDataModel
	{
		public OrgDataStorageModel(string tableName)
			: base("T_SYS_OrgAppStorage")
		{
		}
	}
}
