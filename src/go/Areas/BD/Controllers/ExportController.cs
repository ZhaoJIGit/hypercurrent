using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Go.HtmlHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class ExportController : GoControllerBase
	{
		private Dictionary<BizReportType, string> ExportAllParams
		{
			get
			{
				Dictionary<BizReportType, string> dictionary = new Dictionary<BizReportType, string>();
				if (HtmlSECMenu.HavePermission("Setting", "Export", ""))
				{
					dictionary.Add(BizReportType.InventoryItems, string.Empty);
				}
				if (HtmlSECMenu.HavePermission("Setting", "Export", ""))
				{
					dictionary.Add(BizReportType.ExpenseItem, string.Empty);
				}
				if (HtmlSECMenu.HavePermission("Contact", "Export", ""))
				{
					dictionary.Add(BizReportType.ContactList, string.Empty);
				}
				if (HtmlSECMenu.HavePermission("Contact", "Export", ""))
				{
					dictionary.Add(BizReportType.EmployeeList, string.Empty);
				}
				if (ContextHelper.MContext.MOrgVersionID != 1)
				{
					if (HtmlSECMenu.HavePermission("Invoice_Sales", "Export", ""))
					{
						dictionary.Add(BizReportType.InvoiceList, JsonConvert.SerializeObject(new
						{
							MStatus = 0,
							MType = "Invoice_Sale"
						}));
					}
					if (HtmlSECMenu.HavePermission("Invoice_Purchases", "Export", ""))
					{
						dictionary.Add(BizReportType.PurchaseList, JsonConvert.SerializeObject(new
						{
							MStatus = 0,
							MType = "Invoice_Purchase"
						}));
					}
					if (HtmlSECMenu.HavePermission("Expense", "Export", ""))
					{
						dictionary.Add(BizReportType.ExpenseList, JsonConvert.SerializeObject(new
						{
							BizObject = "Expense",
							MStatus = 0,
							MType = "Invoice_Sale"
						}));
					}
					if (HtmlSECMenu.HavePermission("BankAccount", "Export", ""))
					{
						dictionary.Add(BizReportType.TransactionList, JsonConvert.SerializeObject(new
						{
							StartDate = DateTime.MinValue,
							EndDate = DateTime.MaxValue,
							Sort = "MBizDate",
							Order = "desc"
						}));
					}
				}
				if (base.MContext.MRegProgress == 15)
				{
					if (HtmlSECMenu.HavePermission("BankAccount", "Export", ""))
					{
						dictionary.Add(BizReportType.Accounts, JsonConvert.SerializeObject(new
						{
							IsActive = true
						}));
					}
					if (HtmlSECMenu.HavePermission("General_Ledger", "Export", ""))
					{
						dictionary.Add(BizReportType.VoucherList, string.Empty);
					}
				}
				return dictionary;
			}
		}

		public ActionResult Index()
		{
			return base.View();
		}

		[HttpGet]
		public FileResult ExportAll()
		{
			Stream stream = ExportHelper.PackExports(ExportAllParams);
			string exportName = string.Format("{0} ({1}).zip", base.MContext.MOrgName, base.MContext.DateNow.ToString("yyyyMMdd"));
			return base.ExportReport(stream, exportName);
		}

		[NoAuthorization]
		public JsonResult Progress()
		{
			return base.Json(ExportHelper.ExportProgress);
		}
	}
}
