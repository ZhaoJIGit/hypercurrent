using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVPaymentAttachmentModel : BillAttachmentModel
	{
		public IVPaymentAttachmentModel()
			: base("T_IV_PaymentAttachment")
		{
		}
	}
}
