using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECMenuPermissionModel
	{
		[DataMember]
		public string MMenuID
		{
			get;
			set;
		}

		[DataMember]
		public string MBizObjectID
		{
			get;
			set;
		}

		[DataMember]
		public string MPerMItemID
		{
			get;
			set;
		}
	}
}
