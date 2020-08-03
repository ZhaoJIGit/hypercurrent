using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVInvoiceAttachmentModel : BillAttachmentModel
	{
		public IVInvoiceAttachmentModel()
			: base("T_IV_InvoiceAttachment")
		{
		}
	}
}
