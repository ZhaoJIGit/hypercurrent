using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class ItemRowModel
	{
		[DisplayName("Number")]
		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DisplayName("Purchases Price")]
		[DataMember]
		public string MPurPrice
		{
			get;
			set;
		}

		[DisplayName("Sales Price")]
		[DataMember]
		public string MSalPrice
		{
			get;
			set;
		}

		[DisplayName("Description")]
		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DisplayName("Purchases Tax")]
		[DataMember]
		public string MPurTaxTypeID
		{
			get;
			set;
		}

		[DisplayName("Sales Tax")]
		[DataMember]
		public string MSalTaxTypeID
		{
			get;
			set;
		}

		[DisplayName("InventoryAccount")]
		[DataMember]
		public string MInventoryAccountCode
		{
			get;
			set;
		}

		[DisplayName("IncomeAccount")]
		[DataMember]
		public string MIncomeAccountCode
		{
			get;
			set;
		}

		[DisplayName("CostAccount")]
		[DataMember]
		public string MCostAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string ExpenseAccount
		{
			get;
			set;
		}

		[DataMember]
		public string IsExpenseItem
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsExpenseItem
		{
			get;
			set;
		}
	}
}
