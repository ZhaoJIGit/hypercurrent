using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Context
{
	[DataContract]
	public class MActionResult<T>
	{
		[DataMember]
		public List<MActionResultCodeEnum> ResultCode = new List<MActionResultCodeEnum>();

		[DataMember]
		public List<string> Message = new List<string>();

		[DataMember]
		public bool IsMActionResult = true;

		[DataMember]
		public T ResultData
		{
			get;
			set;
		}

		[DataMember]
		public bool Success
		{
			get
			{
				return ResultCode.Count == 0 || (ResultCode.Count == 1 && ResultCode[0] == MActionResultCodeEnum.Success);
			}
			private set
			{
			}
		}

		public string ResultCodeString
		{
			get
			{
				string text = string.Empty;
				foreach (MActionResultCodeEnum item in ResultCode)
				{
					text = text + item + ",";
				}
				return text.TrimEnd(',');
			}
		}

		public MActionResult<T> ToApiResult()
		{
			return this;
		}

		public MJsonResult ToJsonResult()
		{
			return new MJsonResult
			{
				Codes = ResultCode,
				Messages = Message,
				Data = (object)ResultData
			};
		}
	}
}
