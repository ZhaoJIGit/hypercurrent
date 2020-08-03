using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Invoice
{
	[DataContract]
	public class IVRepeatPreviewPlaceholderModel
	{
		[DataMember]
		public string Title
		{
			get;
			set;
		}

		[DataMember]
		public string Content
		{
			get;
			set;
		}
	}
}
