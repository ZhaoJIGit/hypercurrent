using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAttachmentModel : BDModel
	{
		[DataMember]
		public string MCategoryID
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MUploadName
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSize
		{
			get;
			set;
		}

		[DataMember]
		public string MPath
		{
			get;
			set;
		}

		public BDAttachmentModel()
			: base("T_BD_Attachment")
		{
		}
	}
}
