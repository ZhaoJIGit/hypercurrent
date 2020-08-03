using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.Context
{
	[DataContract]
	public class BizVerificationInfor
	{
		[DataMember]
		public string CheckItem
		{
			get;
			set;
		}

		[DataMember]
		public AlertEnum Level
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
		public string Id
		{
			get;
			set;
		}

		[DataMember]
		public int RowIndex
		{
			get;
			set;
		}

		[DataMember]
		public int DisplayType
		{
			get;
			set;
		}

		[DataMember]
		public string ExtendField
		{
			get;
			set;
		}

		[DataMember]
		public MActionException Exception
		{
			get;
			set;
		}
	}
}
