using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.COM;
using System;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class AccessController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage GetAccess(string token)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				MAccessResponseModel mAccessResponseModel = new MAccessResponseModel();
				ICOMAccess sysService = ServiceHostManager.GetSysService<ICOMAccess>();
				using (sysService as IDisposable)
				{
					mAccessResponseModel = sysService.GetAccessResultByList(null, token).ResultData;
				}
				if (mAccessResponseModel == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				return ResponseHelper.toJson(mAccessResponseModel, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
