using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOPLogModel
	{
		[DataMember]
		public string MPKID
		{
			get;
			set;
		}

		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		[DataMember]
		public string MItem
		{
			get;
			set;
		}

		[DataMember]
		public string MAction
		{
			get;
			set;
		}

		[DataMember]
		public string MFormatID
		{
			get;
			set;
		}

		[DataMember]
		public string MValues
		{
			get;
			set;
		}
	}
}
