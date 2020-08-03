using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class InvoiceVerificationModel
	{
		[DisplayName("BusinessType")]
		[DataMember]
		public string BizType
		{
			get;
			set;
		}

		[DataMember]
		public string Amount
		{
			get;
			set;
		}

		[DisplayName("BusinessDate")]
		[DataMember]
		public string BizDate
		{
			get;
			set;
		}
	}
}
