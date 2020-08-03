using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAttachmentListModel : BDAttachmentModel
	{
		[DataMember]
		public string MCreatorName
		{
			get;
			set;
		}

		[DataMember]
		public string MCreateDateFormated
		{
			get;
			set;
		}

		[DataMember]
		public string MSizeFormated
		{
			get;
			set;
		}

		[DataMember]
		public string RelationID
		{
			get;
			set;
		}
	}
}
