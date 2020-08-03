using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPCodingPageModel
	{
		[DataMember]
		public DataGridJson<FPCodingModel> Codings
		{
			get;
			set;
		}

		[DataMember]
		public string Message
		{
			get;
			set;
		}

		[DataMember]
		public bool Success
		{
			get;
			set;
		}

		[DataMember]
		public FPBaseDataModel BaseData
		{
			get;
			set;
		}
	}
}
