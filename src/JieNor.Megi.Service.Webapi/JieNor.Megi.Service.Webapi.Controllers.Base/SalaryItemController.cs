using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.GL;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers.Base
{
	public class SalaryItemController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetSalaryItemList(string token)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				List<GLTreeModel> obj = new List<GLTreeModel>();
				IGLCheckType sysService = ServiceHostManager.GetSysService<IGLCheckType>();
				using (sysService as IDisposable)
				{
					int type = 4;
					GLCheckTypeDataModel resultData = sysService.GetCheckTypeDataByType(type, false, token).ResultData;
					if (resultData != null)
					{
						obj = resultData.MDataList;
					}
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
