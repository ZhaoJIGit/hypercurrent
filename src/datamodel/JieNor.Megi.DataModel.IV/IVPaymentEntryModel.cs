using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVPaymentEntryModel : IVEntryBaseModel
	{
		[DataMember]
		public string MExpItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MDepartment
		{
			get;
			set;
		}

		public IVPaymentEntryModel()
			: base("T_IV_PaymentEntry")
		{
		}
	}
}
