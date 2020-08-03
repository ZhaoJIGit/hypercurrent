using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Item
{
	[DataContract]
	public class ItemModel : ExportBaseModel
	{
		public string InventoryItemTitle
		{
			get;
			set;
		}

		public string ItemCodeTitle
		{
			get;
			set;
		}

		public string DescriptionTitle
		{
			get;
			set;
		}

		public string CostPriceTitle
		{
			get;
			set;
		}

		public string SalePriceTitle
		{
			get;
			set;
		}

		public string TaxTitle
		{
			get;
			set;
		}

		public string InventoryAccountTitle
		{
			get;
			set;
		}

		public string IncomeAccountTitle
		{
			get;
			set;
		}

		public string CostAccountTitle
		{
			get;
			set;
		}

		public string ExpenseAccountTitle
		{
			get;
			set;
		}

		public string IsExpenseItemTitle
		{
			get;
			set;
		}

		[DisplayName("Item Rows")]
		public ItemRowCollection ItemRows
		{
			get;
			set;
		}
	}
}
