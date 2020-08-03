using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.AutoManager
{
	public class BDTrackManager
	{
		public static List<NameValueModel> GetTrackList()
		{
			IBDTrack sysService = ServiceHostManager.GetSysService<IBDTrack>();
			using (sysService as IDisposable)
			{
				return sysService.GetTrackBasicInfo(null, null).ResultData;
			}
		}
	}
}
