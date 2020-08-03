namespace JieNor.Megi.Common.Redis
{
	public class RedisConfig
	{
		public string Password
		{
			get;
			set;
		}

		public string ReadServer
		{
			get;
			set;
		}

		public string WriteServer
		{
			get;
			set;
		}

		public string KeyPrefix
		{
			get;
			set;
		}

		public bool Ssl
		{
			get;
			set;
		}

		public int DBNum
		{
			get;
			set;
		}

		public RedisConfig()
		{
			Password = "";
		}
	}
}
