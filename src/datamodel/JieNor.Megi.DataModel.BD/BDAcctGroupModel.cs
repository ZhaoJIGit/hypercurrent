using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAcctGroupModel : BDModel
	{
		public BDAcctGroupModel()
			: base("T_BD_AcctGroup")
		{
		}
	}
}
