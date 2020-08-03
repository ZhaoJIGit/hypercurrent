using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVReceiveAttachmentModel : BillAttachmentModel
	{
		public IVReceiveAttachmentModel()
			: base("T_IV_ReceiveAttachment")
		{
		}
	}
}
