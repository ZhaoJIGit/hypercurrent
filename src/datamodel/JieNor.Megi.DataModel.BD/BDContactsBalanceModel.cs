using JieNor.Megi.Core.Attribute;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsBalanceModel
	{
		[DataMember]
		[ApiMember("AccountsReceivable")]
		public BDContactsBalanceDetailModel MAccountsReceivable
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("AccountsPayable")]
		public BDContactsBalanceDetailModel MAccountsPayable
		{
			get;
			set;
		}
	}
}
