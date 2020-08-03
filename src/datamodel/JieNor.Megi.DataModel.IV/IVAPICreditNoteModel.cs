using JieNor.Megi.Core.Attribute;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVAPICreditNoteModel : IVInvoiceModel
	{
		[DataMember]
		[ApiMember("CreditNoteID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MCreditNoteID
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
		[ApiMember("CreditNoteNumber")]
		[DBField("MNumber")]
		public string MCreditNoteNumber
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
