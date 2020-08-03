using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECUserPermissionModel
	{
		[DataMember]
		public string MOrgID
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
		public string MPermissionItemNumber
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsGrant
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsRefuset
		{
			get;
			set;
		}
	}
}
