using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Log;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ExportUtility.Converter;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class ExpenseController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetExpenseList(string token, IVExpenseListFilterModel filter)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				DataGridJson<IVExpenseListModel> dataGridJson = new DataGridJson<IVExpenseListModel>();
				IIVExpense sysService = ServiceHostManager.GetSysService<IIVExpense>();
				using (sysService as IDisposable)
				{
					dataGridJson = sysService.GetExpenseList(filter, token).ResultData;
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

		[HttpPost]
		public HttpResponseMessage GetExpenseExportList(string token, IVExpenseListFilterModel filter)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				string apiModule = filter.ApiModule;
				if (!string.IsNullOrEmpty(apiModule))
				{
					string text = "";
					switch (apiModule)
					{
					case "Export":
					case "Pivotable":
						text = "Export";
						break;
					case "Function":
						text = "Change";
						break;
					default:
						text = "View";
						break;
					}
					if (!AccessHelper.HaveAccess("Expense" + text, token))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
				}
				try
				{
					List<ExpenseListRowModel> list = new List<ExpenseListRowModel>();
					IIVExpense sysService = ServiceHostManager.GetSysService<IIVExpense>();
					using (sysService as IDisposable)
					{
						List<IVExpenseModel> resultData = sysService.GetExpenseListForExport(filter, token).ResultData;
						ExpenseConverter expenseConverter = new ExpenseConverter(new OptLogListFilter
						{
							MContext = MContextManager.GetMContextByAccessToken(token, "Excel"),
							MPKID = string.Join(",", from f in resultData
							select f.MID),
							MBizObject = "ExpenseClaims"
						}, token);
						foreach (IVExpenseModel item in resultData)
						{
							list.AddRange(expenseConverter.ToExportData(item));
						}
					}
					if (list == null)
					{
						string message = "获取数据失败";
						return ResponseHelper.toJson(null, true, message, true);
					}
					return ResponseHelper.toJson(list, true, null, true);
				}
				catch (Exception)
				{
				}
				return ResponseHelper.toJson(null, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
