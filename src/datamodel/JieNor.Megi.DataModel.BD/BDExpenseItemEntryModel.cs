using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDExpenseItemEntryModel : BDEntryModel
	{
		[DataMember]
		public string MDeptAttr
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		public BDExpenseItemEntryModel()
			: base("T_BD_ExpenseItemEntry")
		{
		}
	}
}
