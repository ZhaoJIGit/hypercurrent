using JieNor.Megi.Core.Attribute;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class ValidationError
	{
		[DataMember]
		[ApiMember("Message")]
		public string Message
		{
			get;
			set;
		}

		[DataMember]
		public int Type
		{
			get;
			set;
		}

		[DataMember]
		public int Code
		{
			get;
			set;
		}

		public ValidationError()
		{
		}

		public ValidationError(string message)
		{
			Message = message;
		}

		public ValidationError(string message, int type)
		{
			Message = message;
			Type = type;
		}

		public ValidationError(string message, int type, int code)
		{
			Message = message;
			Type = type;
			Code = code;
		}
	}
}
