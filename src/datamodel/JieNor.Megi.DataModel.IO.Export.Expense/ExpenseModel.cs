using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Expense
{
	[DataContract]
	public class ExpenseModel
	{
		public string Title
		{
			get;
			set;
		}

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

		public string ClaimerTitle
		{
			get;
			set;
		}

		public string Claimer
		{
			get;
			set;
		}

		public string AuditorTitle
		{
			get;
			set;
		}

		public string Auditor
		{
			get;
			set;
		}

		public string TaxTitle
		{
			get;
			set;
		}

		[DisplayName("Employee")]
		[DataMember]
		public string MEmployee
		{
			get;
			set;
		}

		[DisplayName("Department")]
		[DataMember]
		public string MDepartment
		{
			get;
			set;
		}

		[DisplayName("Date")]
		[DataMember]
		public string MBizDate
		{
			get;
			set;
		}

		[DisplayName("DueDate")]
		[DataMember]
		public string MDueDate
		{
			get;
			set;
		}

		[DisplayName("ExpectedPaymentDate")]
		[DataMember]
		public string MExpectedDate
		{
			get;
			set;
		}

		[DisplayName("Reference")]
		[DataMember]
		public string MReference
		{
			get;
			set;
		}

		[DataMember]
		public string Status
		{
			get;
			set;
		}

		[DataMember]
		public string Total
		{
			get;
			set;
		}

		[DataMember]
		public string Currency
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

		[DataMember]
		public ExpenseEntryCollection ExpenseEntrys
		{
			get;
			set;
		}
	}
}
