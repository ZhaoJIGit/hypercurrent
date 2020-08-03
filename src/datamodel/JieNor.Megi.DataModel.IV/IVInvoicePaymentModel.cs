using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVInvoicePaymentModel
	{
		[DataMember]
		public IVInvoiceModel MInvoice
		{
			get;
			set;
		}

		[DataMember]
		public IVPaymentModel MPayment
		{
			get;
			set;
		}
	}
}
