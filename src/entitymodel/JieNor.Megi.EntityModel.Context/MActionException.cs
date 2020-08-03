using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Context
{
	[Serializable]
	public class MActionException : Exception
	{
		private List<MActionResultCodeEnum> codes;

		private List<string> messages;

		[DataMember]
		public List<MActionResultCodeEnum> Codes
		{
			get
			{
				return codes ?? new List<MActionResultCodeEnum>();
			}
			set
			{
				codes = value;
			}
		}

		[DataMember]
		public List<string> Messages
		{
			get
			{
				return messages ?? new List<string>();
			}
			set
			{
				messages = value;
			}
		}

		public MActionException()
		{
		}

		public MActionException(List<string> message)
		{
			Codes = new List<MActionResultCodeEnum>
			{
				MActionResultCodeEnum.ExceptionExist
			};
			Messages = message;
		}

		public MActionException(List<MActionResultCodeEnum> _code)
		{
			Codes = _code;
		}
	}
}
