using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class ObjectPermissionModel
	{
		[DataMember]
		public string MAppID
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
		public string MBizObjectName
		{
			get;
			set;
		}

		[DataMember]
		public string MPermissionItemID
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
		public string MPermissionItemName
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
