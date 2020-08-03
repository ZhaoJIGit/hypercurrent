using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.ExpenseList
{
	[DataContract]
	public class ExpenseListModel
	{
		public string EmployeeTitle
		{
			get;
			set;
		}

		public string DepartmentTitle
		{
			get;
			set;
		}

		public string RefTitle
		{
			get;
			set;
		}

		public string DateTitle
		{
			get;
			set;
		}

		public string DueDateTitle
		{
			get;
			set;
		}

		public string ExpectedDateTitle
		{
			get;
			set;
		}

		public string TotalTitle
		{
			get;
			set;
		}

		public string PaidTitle
		{
			get;
			set;
		}

		public string DueTitle
		{
			get;
			set;
		}

		public string CurrencyTitle
		{
			get;
			set;
		}

		public string StatusTitle
		{
			get;
			set;
		}

		public string ExpenseItemTitle
		{
			get;
			set;
		}

		public string DescriptionTitle
		{
			get;
			set;
		}

		public string QuantityTitle
		{
			get;
			set;
		}

		public string UnitPriceTitle
		{
			get;
			set;
		}

		public string AmountTitle
		{
			get;
			set;
		}

		public string AuditorTitle
		{
			get;
			set;
		}

		public string TaxTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem1Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem2Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem3Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem4Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem5Name
		{
			get;
			set;
		}

		[DisplayName("Expense List Rows")]
		[DataMember]
		public ExpenseListRowCollection ExpenseListRows
		{
			get;
			set;
		}
	}
}
