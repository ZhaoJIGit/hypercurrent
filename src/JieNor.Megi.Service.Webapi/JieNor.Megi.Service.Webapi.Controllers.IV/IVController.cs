using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.IV.Verification;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers.IV
{
	public class IVController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetReceiveList(string token, IVReceiveListFilterModel filter)
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
					if (!AccessHelper.HaveAccess("BankAccount" + text, token))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
				}
				List<IVReceiveModel> list = new List<IVReceiveModel>();
				IIVReceive sysService = ServiceHostManager.GetSysService<IIVReceive>();
				using (sysService as IDisposable)
				{
					list = sysService.GetReceiveListByFilter(filter, token).ResultData;
				}
				if (list == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				list.ForEach(delegate(IVReceiveModel x)
				{
					x.MReference = (string.IsNullOrWhiteSpace(x.MReference) ? "" : x.MReference.Replace('"', '\''));
					if (x.ReceiveEntry != null)
					{
						x.ReceiveEntry.ForEach(delegate(IVReceiveEntryModel y)
						{
							y.MDesc = (string.IsNullOrWhiteSpace(y.MDesc) ? "" : y.MDesc.Replace('"', '\''));
						});
					}
				});
				return ResponseHelper.toJson(list, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetPaymentList(string token, IVPaymentListFilterModel filter)
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
					if (!AccessHelper.HaveAccess("BankAccount" + text, token))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
				}
				List<IVPaymentModel> list = new List<IVPaymentModel>();
				IIVPayment sysService = ServiceHostManager.GetSysService<IIVPayment>();
				using (sysService as IDisposable)
				{
					list = sysService.GetPaymentListIncludeEntry(filter, token).ResultData;
				}
				if (list == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				list.ForEach(delegate(IVPaymentModel x)
				{
					x.MReference = (string.IsNullOrWhiteSpace(x.MReference) ? "" : x.MReference.Replace('"', '\''));
					if (x.PaymentEntry != null)
					{
						x.PaymentEntry.ForEach(delegate(IVPaymentEntryModel y)
						{
							y.MDesc = (string.IsNullOrWhiteSpace(y.MDesc) ? "" : y.MDesc.Replace('"', '\''));
						});
					}
				});
				return ResponseHelper.toJson(list, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetExpenseList(string token, IVExpenseListFilterModel filter)
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
				List<IVExpenseModel> list = new List<IVExpenseModel>();
				IIVExpense sysService = ServiceHostManager.GetSysService<IIVExpense>();
				using (sysService as IDisposable)
				{
					list = sysService.GetExpenseListIncludeEntry(filter, token).ResultData;
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
		public HttpResponseMessage GetVerificationIncludeBillList(string token, IVVerificationFilterModel filter)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				List<IVVerifiactionDocMapModel> list = new List<IVVerifiactionDocMapModel>();
				IIVVerification sysService = ServiceHostManager.GetSysService<IIVVerification>();
				using (sysService as IDisposable)
				{
					list = sysService.GetVerificationIncludeBillList(filter, token).ResultData;
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
	}
}
