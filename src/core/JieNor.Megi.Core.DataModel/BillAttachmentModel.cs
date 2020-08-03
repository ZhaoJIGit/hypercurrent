using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BillAttachmentModel : BizDataModel
	{
		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public string MAttachID
		{
			get;
			set;
		}

		public BillAttachmentModel(string tableName)
			: base(tableName)
		{
		}
	}
}
