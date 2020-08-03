using System.Collections.Generic;
using System.Text;

namespace JieNor.Megi.Core.Context
{
	public class ImportResult
	{
		private string _message = string.Empty;

		public bool Success
		{
			get;
			set;
		}

		public string Message
		{
			get
			{
				if (!string.IsNullOrEmpty(_message))
				{
					return _message;
				}
				if (MessageList == null || MessageList.Count == 0)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string message in MessageList)
				{
					stringBuilder.AppendFormat("{0}<br/>", message);
				}
				return stringBuilder.ToString();
			}
			set
			{
				_message = value;
			}
		}

		public List<string> MessageList
		{
			get;
			set;
		}

		public string Tag
		{
			get;
			set;
		}

		public string SuccessRowIndexes
		{
			get;
			set;
		}

		public List<BizVerificationInfor> FeedbackInfo
		{
			get;
			set;
		}

		public int NormalFPCount
		{
			get;
			set;
		}

		public int SpecialFPCount
		{
			get;
			set;
		}

		public bool HasGLStartData
		{
			get;
			set;
		}

		public ImportResult()
		{
			MessageList = new List<string>();
			FeedbackInfo = new List<BizVerificationInfor>();
		}
	}
}
