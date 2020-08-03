using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MSG
{
	[DataContract]
	public class MSGMessageModel : BizDataModel
	{
		public string MSenderID
		{
			get;
			set;
		}

		[DataMember]
		public string MReceiverID
		{
			get;
			set;
		}

		[DataMember]
		public string MTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MContent
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsRead
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsReply
		{
			get;
			set;
		}

		[DataMember]
		public string MGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MReplyID
		{
			get;
			set;
		}

		public MSGMessageModel()
			: base("T_Msg_Message")
		{
			MType = Convert.ToInt32(MSGMessageTypeModel.Basic);
		}
	}
}
