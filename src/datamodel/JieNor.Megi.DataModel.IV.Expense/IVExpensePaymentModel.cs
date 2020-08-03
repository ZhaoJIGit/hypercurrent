using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Expense
{
	[DataContract]
	public class IVExpensePaymentModel
	{
		[DataMember]
		public IVExpenseModel MExpense
		{
			get;
			set;
		}

		[DataMember]
		public IVPaymentModel MPayment
		{
			get;
			set;
		}
	}
}
