using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLCheckTypeModel : BDModel
	{
		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string MColumnName
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

		public GLCheckTypeModel()
			: base("T_GL_CHECKTYPE")
		{
		}

		public GLCheckTypeModel(string tableName)
			: base(tableName)
		{
		}
	}
}
