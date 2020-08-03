using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAttachmentCategoryModel : BDModel
	{
		[DataMember]
		public string MCategoryName
		{
			get;
			set;
		}

		[DataMember]
		public string MBizObject
		{
			get;
			set;
		}

		public BDAttachmentCategoryModel()
			: base("T_BD_AttachmentCategory")
		{
		}
	}
}
