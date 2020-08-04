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
	public class ExpenseItemController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetExpenseItemList(string token, BDExpenseItemListFilterModel filter)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				DataGridJson<BDExpenseItemModel> dataGridJson = new DataGridJson<BDExpenseItemModel>();
				filter.Equal("MIsActive", 1);
				IBDExpenseItem sysService = ServiceHostManager.GetSysService<IBDExpenseItem>();
				using (sysService as IDisposable)
				{
					dataGridJson = sysService.GetPageList(filter, token).ResultData;
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
