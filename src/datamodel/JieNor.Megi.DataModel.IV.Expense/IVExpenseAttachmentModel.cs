using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Expense
{
	[DataContract]
	public class IVExpenseAttachmentModel : BillAttachmentModel
	{
		public IVExpenseAttachmentModel()
			: base("T_IV_ExpenseAttachment")
		{
		}
	}
}
