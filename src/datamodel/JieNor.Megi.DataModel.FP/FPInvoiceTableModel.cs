using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPInvoiceTableModel : BDModel
	{
		[DataMember]
		public string MInvoiceID
		{
			get;
			set;
		}

		[DataMember]
		public string MTableID
		{
			get;
			set;
		}

		[DataMember]
		public int MInvoiceType
		{
			get;
			set;
		}

		public FPInvoiceTableModel()
			: base("T_FP_Invoice_Table")
		{
		}

		public FPInvoiceTableModel(string tableName)
			: base(tableName)
		{
		}
	}
}
