using JieNor.Megi.Common.Redis;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.MResource;
using System;
using System.Timers;
using System.Web;

namespace JieNor.Megi.Service.Web
{
	public class Global : HttpApplication
	{
		private static Timer timer;

		public static int timerInterval = 8;

		protected void Application_Start(object sender, EventArgs e)
		{
			InitTimer();
			ConfigHelper configHelper = new ConfigHelper();
			configHelper.RegisterSysConfig();
			RedisClientManager.Instance.Subscribe("RefreshSysConfig", delegate
			{
				ConfigHelper configHelper2 = new ConfigHelper();
				configHelper2.RegisterSysConfig();
			});
		}

		protected void Application_End(object sender, EventArgs e)
		{
			CloseTimer();
		}

		private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			MResourceHelper.ReleaseGLDataPool();
		}

		public static void InitTimer()
		{
			timer = new Timer();
			timer.Interval = (double)(timerInterval * 1000);
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
		}

		public static void CloseTimer()
		{
			if (timer != null)
			{
				timer.Stop();
				timer = null;
			}
		}
	}
}
