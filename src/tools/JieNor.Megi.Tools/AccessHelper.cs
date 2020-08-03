using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.COM;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Tools
{
	public static class AccessHelper
	{
		public static MAccessResponseModel Access(List<MAccessRequestModel> requestList = null)
		{
			ICOMAccess sysService = ServiceHostManager.GetSysService<ICOMAccess>();
			using (sysService as IDisposable)
			{
				return sysService.GetAccessResultByList(requestList, null).ResultData;
			}
		}

		public static MAccessResponseModel Access(MAccessRequestModel request)
		{
			return Access(new List<MAccessRequestModel>
			{
				request
			});
		}

		public static MAccessResponseModel Access(string obj, string item, int type = 2)
		{
			return Access(new List<MAccessRequestModel>
			{
				new MAccessRequestModel
				{
					Name = "0",
					BizAccess = item,
					BizModule = obj,
					RequestType = type
				}
			});
		}
	}
}
