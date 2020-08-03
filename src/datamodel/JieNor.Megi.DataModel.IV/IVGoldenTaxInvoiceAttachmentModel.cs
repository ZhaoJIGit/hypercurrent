using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVGoldenTaxInvoiceAttachmentModel : BillAttachmentModel
	{
		public IVGoldenTaxInvoiceAttachmentModel()
			: base("T_IV_GoldenTaxInvoiceAttachment")
		{
		}
	}
}
