using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVEmailTmplPreviewModel
	{
		[DataMember]
		public string MSubject
		{
			get;
			set;
		}

		[DataMember]
		public string MContent
		{
			get;
			set;
		}

		[DataMember]
		public string MSubjectPreview
		{
			get;
			set;
		}

		[DataMember]
		public string MContentPreview
		{
			get;
			set;
		}
	}
}
