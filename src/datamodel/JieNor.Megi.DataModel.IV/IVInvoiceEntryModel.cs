using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVInvoiceEntryModel : IVEntryBaseModel
	{
		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MBizDate
		{
			get;
			set;
		}

		[DataMember]
		public int MRowIndex
		{
			get;
			set;
		}

		public IVInvoiceEntryModel()
			: base("T_IV_InvoiceEntry")
		{
		}
	}
}
