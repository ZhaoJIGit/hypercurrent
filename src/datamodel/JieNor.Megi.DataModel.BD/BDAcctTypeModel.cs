using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAcctTypeModel : BDModel
	{
		[DataMember]
		public string MAccountGroupID
		{
			get;
			set;
		}

		public BDAcctTypeModel()
			: base("t_bas_accounttype")
		{
		}
	}
}
