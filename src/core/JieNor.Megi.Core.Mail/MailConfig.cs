namespace JieNor.Megi.Core.Mail
{
	internal class MailConfig
	{
		private int _port = 25;

		public string Smtp
		{
			get;
			set;
		}

		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}

		public string UserName
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string DisplayName
		{
			get;
			set;
		}
	}
}
