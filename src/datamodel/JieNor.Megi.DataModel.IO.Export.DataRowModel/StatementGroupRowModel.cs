using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class StatementGroupRowModel
	{
		[DisplayName("Date")]
		[DataMember]
		public string MDate
		{
			get;
			set;
		}

		[DisplayName("Activity")]
		[DataMember]
		public string MActivity
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

		[DisplayName("DueDate")]
		[DataMember]
		public string MDueDate
		{
			get;
			set;
		}

		[DisplayName("InvoiceAmount")]
		[DataMember]
		public string MInvoiceAmount
		{
			get;
			set;
		}

		[DisplayName("Payments")]
		[DataMember]
		public string MPayments
		{
			get;
			set;
		}

		[DisplayName("Balance")]
		[DataMember]
		public string MBalance
		{
			get;
			set;
		}
	}
}
