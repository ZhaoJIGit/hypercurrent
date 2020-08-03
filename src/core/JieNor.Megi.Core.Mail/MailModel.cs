using System.Collections.Generic;

namespace JieNor.Megi.Core.Mail
{
	public class MailModel
	{
		private List<string> _toAddressList;

		private List<string> _attachmentList;

		public string Subject
		{
			get;
			set;
		}

		public string Body
		{
			get;
			set;
		}

		public List<string> ToAddressList
		{
			get
			{
				return _toAddressList;
			}
			set
			{
				_toAddressList = value;
			}
		}

		public List<string> AttachmentList
		{
			get
			{
				return _attachmentList;
			}
			set
			{
				_attachmentList = value;
			}
		}

		public MailModel()
		{
			_toAddressList = new List<string>();
			_attachmentList = new List<string>();
		}
	}
}
