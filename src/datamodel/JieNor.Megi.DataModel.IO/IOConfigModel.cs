using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO
{
	[DataContract]
	public class IOConfigModel : IOModel
	{
		[DataMember]
		public int MTypeID
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
		public string MSimilarName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsRequired
		{
			get;
			set;
		}

		[DataMember]
		public string MExpression
		{
			get;
			set;
		}

		[DataMember]
		public int MSequence
		{
			get;
			set;
		}

		[DataMember]
		public int MIsKey
		{
			get;
			set;
		}

		public IOConfigModel()
			: base("T_IO_Config")
		{
		}

		public IOConfigModel(string tableName)
			: base(tableName)
		{
		}
	}
}
