using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLVoucherInfoModel
	{
		[DataMember]
		public string MVoucherID
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherNumber
		{
			get;
			set;
		}

		[DataMember]
		public int MVoucherStatus
		{
			get;
			set;
		}
	}
}
