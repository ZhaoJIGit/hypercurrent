using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MSG
{
	[DataContract]
	public class MSGMessageEditModel : MSGMessageModel
	{
		[DataMember]
		public bool MIsSendEmail
		{
			get;
			set;
		}

		[DataMember]
		public List<MSGMessageUserModel> MReceiverList
		{
			get;
			set;
		}
	}
}
