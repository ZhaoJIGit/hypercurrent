using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using System;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDAttachmentManager
	{
		public static BDAttachmentModel GetAttachmentModel(string id)
		{
			IBDAttachment sysService = ServiceHostManager.GetSysService<IBDAttachment>();
			using (sysService as IDisposable)
			{
				return sysService.GetAttachmentModel(id, null).ResultData;
			}
		}
	}
}
