using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class ImportController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetTemplateData(string token, ImportTypeEnum templateType)
		{
			try
			{
				if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
				{
					if (!CheckAccess(token, templateType))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
					templateType = ((templateType == ImportTypeEnum.ExpenseSmart) ? ImportTypeEnum.Expense : templateType);
					ImportTemplateModel importTemplateModel = new ImportTemplateModel();
					IIOImport sysService = ServiceHostManager.GetSysService<IIOImport>();
					using (sysService as IDisposable)
					{
						importTemplateModel = sysService.GetTemplateModel(templateType, token).ResultData;
					}
					if (importTemplateModel == null)
					{
						string message = "获取数据失败";
						return ResponseHelper.toJson(null, true, message, true);
					}
					return ResponseHelper.toJson(importTemplateModel, true, null, true);
				}
				return ResponseHelper.toJson(null, true, null, true);
			}
			catch (Exception ex)
			{
				LogHelper.WriteLog("ImportController", "GetTemplateData", ex.Message);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		private bool CheckAccess(string token, ImportTypeEnum templateType)
		{
			string accessKey = "";
			switch (templateType)
			{
			case ImportTypeEnum.Invoice:
				accessKey = "Invoice_SalesChange";
				break;
			case ImportTypeEnum.Expense:
				accessKey = "ExpenseChange";
				break;
			case ImportTypeEnum.ExpenseSmart:
				accessKey = "General_LedgerChange";
				break;
			case ImportTypeEnum.Payment:
				accessKey = "PaymentChange";
				break;
			case ImportTypeEnum.Purchase:
				accessKey = "Invoice_PurchasesChange";
				break;
			case ImportTypeEnum.Receive:
				accessKey = "ReceiveChange";
				break;
			case ImportTypeEnum.Voucher:
				accessKey = "VoucherChange";
				break;
			}
			if (!AccessHelper.HaveAccess(accessKey, token))
			{
				return false;
			}
			return true;
		}

		[HttpPost]
		public HttpResponseMessage GetTemplateFieldConfig(string token, ImportTypeEnum templateType)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				if (!CheckAccess(token, templateType))
				{
					return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
				}
				List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
				IIOImport sysService = ServiceHostManager.GetSysService<IIOImport>();
				using (sysService as IDisposable)
				{
					list = sysService.GetTemplateConfig(templateType, token).ResultData;
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
		public HttpResponseMessage ImportTemplateData(string token, IOImportDataModel importData)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				importData.EffectiveData = importData.EffectiveDataSet.Tables[0];
				if (!ValidateImportDataAuth(importData.TemplateType, importData.EffectiveData, token))
				{
					return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
				}
				ImportResult importResult = new ImportResult();
				IIOImport sysService = ServiceHostManager.GetSysService<IIOImport>();
				using (sysService as IDisposable)
				{
					importResult = sysService.ImportData(importData.TemplateType, importData, token).ResultData;
				}
				if (importResult == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				return ResponseHelper.toJson(importResult, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		private bool ValidateImportDataAuth(ImportTypeEnum templateType, DataTable dt, string token)
		{
			List<string> list = new List<string>();
			switch (templateType)
			{
			case ImportTypeEnum.Expense:
			{
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					if (flag2)
					{
						break;
					}
					if (!flag)
					{
						string value = dt.Rows[i]["MPayMentDate"].ToString();
						string value2 = dt.Rows[i]["MBankAccount"].ToString();
						string value3 = dt.Rows[i]["MPaidAmount"].ToString();
						if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2) || !string.IsNullOrEmpty(value3))
						{
							flag = true;
						}
					}
					else
					{
						string value4 = dt.Rows[i]["MDebitAccount"].ToString();
						string value5 = dt.Rows[i]["MCreditAccount"].ToString();
						string value6 = dt.Rows[i]["MTaxAccount"].ToString();
						if (!string.IsNullOrEmpty(value4) || !string.IsNullOrEmpty(value5) || !string.IsNullOrEmpty(value6))
						{
							flag2 = true;
						}
					}
				}
				if (flag)
				{
					list.Add("ExpenseApprove");
					list.Add("BankAccountChange");
				}
				if (flag2)
				{
					list.Add("ExpenseApprove");
					list.Add("General_LedgerChange");
				}
				break;
			}
			case ImportTypeEnum.ExpenseSmart:
				list.Add("General_LedgerChange");
				break;
			}
			if (list.Count > 0)
			{
				return AccessHelper.HaveAccess(list, token, "and");
			}
			return true;
		}
	}
}
