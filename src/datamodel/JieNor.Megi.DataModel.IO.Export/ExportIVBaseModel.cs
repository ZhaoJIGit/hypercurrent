using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export
{
	[DataContract]
	public class ExportIVBaseModel : ExportBaseModel
	{
		[DisplayName("RegisteredAddress")]
		[DataMember]
		public string RegAddress
		{
			get;
			set;
		}

		[DisplayName("RegisteredAddress(Align Center)")]
		[DataMember]
		public string RegAddressAlignCenter
		{
			get;
			set;
		}

		[DataMember]
		public string TaxNumber
		{
			get;
			set;
		}

		[DataMember]
		public string TaxNumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TaxNumberContainTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TermsTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TermsAndPaymentAdvice
		{
			get;
			set;
		}

		[DataMember]
		public string PaymentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PayeeAccount
		{
			get;
			set;
		}

		[DisplayName("ContactPostalAddress")]
		[DataMember]
		public string ContactAddress
		{
			get;
			set;
		}

		[DataMember]
		public string ContactPhysicalAddress
		{
			get;
			set;
		}

		[DataMember]
		public string ContactPhone
		{
			get;
			set;
		}
	}
}
