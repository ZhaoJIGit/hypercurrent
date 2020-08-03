using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Expense
{
	[DataContract]
	public class IVExpenseEntryModel : IVEntryBaseModel
	{
		public override string MTaxID
		{
			get
			{
				return base.MTaxID;
			}
			set
			{
				base.MTaxID = "";
			}
		}

		[DataMember]
		public decimal MApproveAmtFor
		{
			get
			{
				return base.MTaxAmountFor;
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MApproveAmt
		{
			get
			{
				return base.MTaxAmount;
			}
			set
			{
			}
		}

		[DataMember]
		public int MRowIndex
		{
			get;
			set;
		}

		public IVExpenseEntryModel()
			: base("T_IV_ExpenseEntry")
		{
		}
	}
}
