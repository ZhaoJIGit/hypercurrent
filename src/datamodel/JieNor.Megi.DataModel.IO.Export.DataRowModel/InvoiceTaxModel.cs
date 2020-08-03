using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class InvoiceTaxModel
	{
		[DataMember]
		public string TaxName
		{
			get;
			set;
		}

		[DisplayName("TaxAmount")]
		[DataMember]
		public string TaxAmtFor
		{
			get;
			set;
		}
	}
}
