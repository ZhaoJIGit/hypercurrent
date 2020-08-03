using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASResultModel
	{
		[DataMember]
		public bool IsSuccess
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
		public string MPKID
		{
			get;
			set;
		}
	}
}
