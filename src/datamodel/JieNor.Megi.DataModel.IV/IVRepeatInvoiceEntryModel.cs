using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVRepeatInvoiceEntryModel : IVEntryBaseModel
	{
		[DataMember]
		public string MTempEntryID
		{
			get;
			set;
		}

		public IVRepeatInvoiceEntryModel()
			: base("T_IV_RepeatInvoiceEntry")
		{
		}
	}
}
