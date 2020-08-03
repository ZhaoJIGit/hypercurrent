using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class ExpenseClaimDetailRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Date
		{
			get;
			set;
		}

		[DataMember]
		public string Type
		{
			get;
			set;
		}

		[DataMember]
		public string Employee
		{
			get;
			set;
		}

		[DataMember]
		public string Reference
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
		public string ExpenseItem
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Amount
		{
			get;
			set;
		}

		[DataMember]
		public decimal? AmountCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Total
		{
			get;
			set;
		}

		[DataMember]
		public string AmountStr
		{
			get;
			set;
		}

		[DataMember]
		public string AmountCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string TotalStr
		{
			get;
			set;
		}
	}
}
