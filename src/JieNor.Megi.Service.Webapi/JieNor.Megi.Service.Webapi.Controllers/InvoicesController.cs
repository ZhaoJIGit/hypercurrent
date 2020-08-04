using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ExportUtility.Converter;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class InvoicesController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetInvoviceList(string token, IVInvoiceListFilterModel filter)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				DataGridJson<IVInvoiceListModel> dataGridJson = new DataGridJson<IVInvoiceListModel>();
				IIVInvoice sysService = ServiceHostManager.GetSysService<IIVInvoice>();
				using (sysService as IDisposable)
				{
					dataGridJson = sysService.GetInvoiceList(filter, token).ResultData;
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
		public HttpResponseMessage GetInvoiceEditList(string token, IVInvoiceListFilterModel filter)
		{
			List<IVInvoiceModel> list = new List<IVInvoiceModel>();
			List<IVInvoiceListModel> list2 = new List<IVInvoiceListModel>();
			IIVInvoice sysService = ServiceHostManager.GetSysService<IIVInvoice>();
			using (sysService as IDisposable)
			{
				list2 = sysService.GetInvoiceList(filter, token).ResultData.rows;
				if (list2 != null)
				{
					foreach (IVInvoiceListModel item in list2)
					{
						IVInvoiceModel resultData = sysService.GetInvoiceEditModel(item.MID, filter.MType, token).ResultData;
						if (resultData != null && !string.IsNullOrWhiteSpace(resultData.MID))
						{
							list.Add(resultData);
						}
					}
				}
			}
			return ResponseHelper.toJson(list, true, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetInvoiceExportList(string token, IVInvoiceListFilterModel filter)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				string apiModule = filter.ApiModule;
				if (!string.IsNullOrEmpty(apiModule))
				{
					string text = "";
					switch (apiModule)
					{
					case "CreatePivotable":
						text = "Change";
						break;
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
					if (!AccessHelper.HaveAccess((filter.MType == "Invoice_Sale") ? ("Invoice_Sales" + text) : ("Invoice_Purchases" + text), token))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
				}
				List<InvoiceListRowModel> list = new List<InvoiceListRowModel>();
				IIVInvoice sysService = ServiceHostManager.GetSysService<IIVInvoice>();
				using (sysService as IDisposable)
				{
					List<IVInvoiceModel> resultData = sysService.GetInvoiceListForExport(filter, token).ResultData;
					InvoiceConverter invoiceConverter = new InvoiceConverter(token)
					{
						IsFromExcel = filter.IsFromExcel
					};
					foreach (IVInvoiceModel item in resultData)
					{
						list.AddRange(invoiceConverter.ToExportData(item));
					}
				}
				if (list == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				foreach (InvoiceListRowModel item2 in list)
				{
					item2.MDesc = item2.MDesc;
					item2.MReference = item2.MReference;
				}
				return ResponseHelper.toJson(list, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
