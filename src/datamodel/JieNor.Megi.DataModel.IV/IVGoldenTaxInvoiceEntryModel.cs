using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVGoldenTaxInvoiceEntryModel : BizEntryDataModel
	{
		[DataMember]
		public string MItemName
		{
			get;
			set;
		}

		[DataMember]
		public string MSpecification
		{
			get;
			set;
		}

		[DataMember]
		public string MUnits
		{
			get;
			set;
		}

		[DataMember]
		public decimal MQty
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPrice
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmount
		{
			get;
			set;
		}

		public IVGoldenTaxInvoiceEntryModel()
			: base("T_IV_GoldenTaxInvoiceEntry")
		{
		}
	}
}
