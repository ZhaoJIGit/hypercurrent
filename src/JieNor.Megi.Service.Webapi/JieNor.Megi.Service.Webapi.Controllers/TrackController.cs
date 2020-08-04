using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class TrackController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetTrackList(string token)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				List<BDTrackModel> list = new List<BDTrackModel>();
				IBDTrack sysService = ServiceHostManager.GetSysService<IBDTrack>();
				using (sysService as IDisposable)
				{
					list = sysService.GetList("", token).ResultData;
				}
				if (list == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				return ResponseHelper.toJson(list, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetTrackBasicInfo(string token)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				IBDTrack sysService = ServiceHostManager.GetSysService<IBDTrack>();
				List<NameValueModel> obj = new List<NameValueModel>();
				using (sysService as IDisposable)
				{
					obj = sysService.GetTrackBasicInfo("", token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
