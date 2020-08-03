using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVContactInvoiceSummaryModel
	{
		[DataMember]
		public decimal SaleAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal BillAmount
		{
			get;
			set;
		}
	}
}
