using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Verification
{
	public class IVVerifiactionDocMapModel : IVVerificationModel
	{
		[DataMember]
		public List<IVInvoiceListModel> InvoiceList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVReceiveModel> ReceiveList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVPaymentModel> PaymentList
		{
			get;
			set;
		}
	}
}
