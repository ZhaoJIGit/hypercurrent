using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSOrgAppStorageModel : BDDetailModel
	{
		[DataMember]
		public string MOrgAppID
		{
			get;
			set;
		}

		[DataMember]
		public string MStorageID
		{
			get;
			set;
		}

		public SYSOrgAppStorageModel()
			: base("T_SYS_OrgAppStorage")
		{
		}
	}
}
