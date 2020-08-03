using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class InvoicesRowModel : BaseReportRowModel
	{
		[DataMember]
		public string InvoiceDate
		{
			get;
			set;
		}

		[DataMember]
		public string InvoiceNo
		{
			get;
			set;
		}

		[DataMember]
		public string DueDate
		{
			get;
			set;
		}

		[DataMember]
		public string DueDays
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
		public decimal? Paid
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Due
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DueCNY
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
		public string TotalStr
		{
			get;
			set;
		}

		[DataMember]
		public string PaidStr
		{
			get;
			set;
		}

		[DataMember]
		public string DueStr
		{
			get;
			set;
		}

		[DataMember]
		public string DueCNYStr
		{
			get;
			set;
		}
	}
}
