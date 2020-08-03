using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAcctPolicyTypeModel : BDModel
	{
		[DataMember]
		public string MPolicy
		{
			get;
			set;
		}

		[DataMember]
		public string MTypeID
		{
			get;
			set;
		}

		public BDAcctPolicyTypeModel()
			: base("T_BD_AcctPolicyType")
		{
		}
	}
}
