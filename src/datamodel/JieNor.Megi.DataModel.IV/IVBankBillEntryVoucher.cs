using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankBillEntryVoucher : BizDataModel
	{
		[DataMember]
		public string MBankBillEntryID
		{
			get;
			set;
		}

		public IVBankBillEntryVoucher()
			: base("t_iv_bankbillentryvoucher")
		{
		}
	}
}
