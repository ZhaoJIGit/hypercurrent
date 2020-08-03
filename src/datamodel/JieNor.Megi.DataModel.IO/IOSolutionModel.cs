using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO
{
	[DataContract]
	public class IOSolutionModel : IOModel
	{
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public int MTypeID
		{
			get;
			set;
		}

		[DataMember]
		public int MHeaderRowIndex
		{
			get;
			set;
		}

		[DataMember]
		public int MDataRowIndex
		{
			get;
			set;
		}

		[ModelEntry]
		[ApiDetail]
		[DataMember]
		public List<IOSolutionConfigModel> MConfig
		{
			get;
			set;
		}

		[DataMember]
		public string MFileName
		{
			get;
			set;
		}

		public IOSolutionModel()
			: base("T_IO_Solution")
		{
		}
	}
}
