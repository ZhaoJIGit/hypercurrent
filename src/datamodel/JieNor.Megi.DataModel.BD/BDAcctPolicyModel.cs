using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAcctPolicyModel : BDModel
	{
		public BDAcctPolicyModel()
			: base("T_BD_AcctPolicy")
		{
		}
	}
}
