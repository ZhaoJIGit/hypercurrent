using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class EmployeeController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetEmployeeList(string token, BDEmployeesListFilterModel filter)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				DataGridJson<BDEmployeesListModel> dataGridJson = new DataGridJson<BDEmployeesListModel>();
				IBDEmployees sysService = ServiceHostManager.GetSysService<IBDEmployees>();
				using (sysService as IDisposable)
				{
					dataGridJson = sysService.GetBDEmployeesPageList(filter, token).ResultData;
				}
				if (dataGridJson == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				return ResponseHelper.toJson(dataGridJson.rows, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
