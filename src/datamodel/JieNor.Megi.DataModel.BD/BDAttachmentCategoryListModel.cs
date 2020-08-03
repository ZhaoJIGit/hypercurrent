using System.Runtime.Serialization;
using System.Text;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAttachmentCategoryListModel : BDAttachmentCategoryModel
	{
		[DataMember]
		public int AttachmentCount
		{
			get;
			set;
		}

		public string MCategoryNameEllipsis
		{
			get
			{
				if (string.IsNullOrWhiteSpace(base.MCategoryName))
				{
					return string.Empty;
				}
				int num = 15;
				int byteCount = Encoding.GetEncoding("gb2312").GetByteCount(base.MCategoryName);
				if (byteCount > num)
				{
					int num2 = 0;
					for (int i = 0; i < base.MCategoryName.Length; i++)
					{
						int byteCount2 = Encoding.GetEncoding("gb2312").GetByteCount(new char[1]
						{
							base.MCategoryName[i]
						});
						num2 += byteCount2;
						if (num2 > num)
						{
							return base.MCategoryName.Substring(0, i) + "...";
						}
					}
				}
				return base.MCategoryName;
			}
		}
	}
}
