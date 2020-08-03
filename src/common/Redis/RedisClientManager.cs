using StackExchange.Redis;
using System.Linq;

namespace JieNor.Megi.Common.Redis
{
	public class RedisClientManager
	{
		private static RedisClientHelper _client = null;

		public static RedisClientHelper Instance
		{
			get
			{
				if (_client == null)
				{
					return new RedisClientHelper();
				}
				return _client;
			}
		}

		public static void Register(RedisConfig config)
		{
			if (config != null && !string.IsNullOrEmpty(config.ReadServer))
			{
				ConfigurationOptions configOption = new ConfigurationOptions();
				configOption.ServiceName = config.WriteServer;
				configOption.Password = config.Password;
				configOption.AbortOnConnectFail = false;
				configOption.DefaultDatabase = config.DBNum;
				configOption.Ssl = config.Ssl;
				configOption.EndPoints.Add(config.WriteServer);
				foreach (string item in config.ReadServer.Split(',').ToList())
				{
					configOption.EndPoints.Add(item);
				}
				_client = new RedisClientHelper(config.DBNum, configOption, config.KeyPrefix);
			}
		}
	}
}
