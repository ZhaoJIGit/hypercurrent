using JieNor.Megi.DataModel.SEC;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASUserInfoModel
	{
		[DataMember]
		public SECUserModel UserModel
		{
			get;
			set;
		}

		[DataMember]
		public SECUserlModel UserlModel
		{
			get;
			set;
		}
	}
}
