using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECRoleModel : BDModel
	{
		[DataMember]
		public string MAppID
		{
			get;
			set;
		}

		[DataMember]
		public int MShowIndex
		{
			get;
			set;
		}

		public SECRoleModel()
			: base("T_Sec_Role")
		{
		}
	}
}
