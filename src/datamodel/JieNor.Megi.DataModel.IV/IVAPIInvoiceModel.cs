using JieNor.Megi.Core.Attribute;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVAPIInvoiceModel : IVInvoiceModel
	{
		[DataMember]
		[ApiMember("InvoiceID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MInvoiceID
		{
			get
			{
				return base.MID;
			}
			set
			{
				base.MID = value;
			}
		}

		[DataMember(EmitDefaultValue = true)]
		[ApiMember("InvoiceNumber")]
		[DBField("MNumber")]
		public string MInvoiceNumber
		{
			get
			{
				return base.MNumber;
			}
			set
			{
				base.MNumber = value;
			}
		}
	}
}
