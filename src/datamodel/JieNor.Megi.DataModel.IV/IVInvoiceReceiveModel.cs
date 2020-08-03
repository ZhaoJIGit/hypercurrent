using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVInvoiceReceiveModel
	{
		[DataMember]
		public IVInvoiceModel MInvoice
		{
			get;
			set;
		}

		[DataMember]
		public IVReceiveModel MPayment
		{
			get;
			set;
		}
	}
}
