using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLVoucherReferenceModel : BDModel
	{
		[DataMember]
		public string MContent
		{
			get;
			set;
		}

		[DataMember]
		public string MLocaleID
		{
			get;
			set;
		}

		public GLVoucherReferenceModel()
			: base("T_GL_VOUCHERREFERENCE")
		{
		}

		public GLVoucherReferenceModel(string tableName)
			: base(tableName)
		{
		}
	}
}
