using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.ServiceContract.MSG;
using System;

namespace JieNor.Megi.Identity.AutoManager
{
	public class MSGMessageManager
	{
		public static int GetReceiveMessageCount()
		{
			IMSGMessage sysService = ServiceHostManager.GetSysService<IMSGMessage>();
			using (sysService as IDisposable)
			{
				return sysService.GetReceiveMessageCount(null).ResultData;
			}
		}
	}
}
