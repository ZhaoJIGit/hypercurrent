using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.Service.Webapi.ViewModel;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.RPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class AccountLedgerController : ApiController
	{
		public HttpResponseMessage GetAccountPerid(string token)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				IGLSettlement sysService = ServiceHostManager.GetSysService<IGLSettlement>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				using (sysService as IDisposable)
				{
					List<DateTime> resultData = sysService.GetSettledPeriodFromBeginDate(true, token).ResultData;
					if (resultData != null && resultData.Count() > 0)
					{
						resultData = (from x in resultData
						orderby x.Date
						select x).ToList();
						foreach (DateTime item in resultData)
						{
							string key = item.ToString("yyyyMM");
							string value = item.ToString("yyyy-MM");
							dictionary.Add(key, value);
						}
					}
				}
				return ResponseHelper.toJson(dictionary, true, null, true);
			}
			return ResponseHelper.toJson(null, false, null, true);
		}

		public HttpResponseMessage GetAccountList(string token)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				IBDAccount sysService = ServiceHostManager.GetSysService<IBDAccount>();
				List<BDAccountModel> obj = new List<BDAccountModel>();
				using (sysService as IDisposable)
				{
					obj = sysService.GetBaseBDAccountList(null, true, token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, false, null, true);
		}

		[HttpPost]
		[Compress]
		public HttpResponseMessage GetSubLedgerList(string token, RPTSubsidiaryLedgerFilterModel filter)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
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
					if (!AccessHelper.HaveAccess("Other_Reports" + text, token))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
				}
				GLBalanceListFilterModel gLBalanceListFilterModel = new GLBalanceListFilterModel();
				gLBalanceListFilterModel.AccountIDS = (string.IsNullOrWhiteSpace(filter.MAccountIDs) ? new List<string>() : filter.MAccountIDs.Split(',').ToList());
				gLBalanceListFilterModel.EndPeriod = Convert.ToInt32(filter.MEndPeroid);
				gLBalanceListFilterModel.StartPeriod = Convert.ToInt32(filter.MStartPeroid);
				gLBalanceListFilterModel.IncludeCheckType = filter.IncludeCheckType;
				gLBalanceListFilterModel.CheckTypeValueList = filter.CheckTypeValueList;
				gLBalanceListFilterModel.RequestIsNotFromFormula = true;
				gLBalanceListFilterModel.IsIncludeChildrenAccount = true;
				IGLExcel sysService = ServiceHostManager.GetSysService<IGLExcel>();
				new List<GLBalanceModel>();
				List<GLBalanceViewModel> o = new List<GLBalanceViewModel>();
				using (sysService as IDisposable)
				{
					o = ModelConvertHelper.CovertToBalanceViewModel(sysService.GetBalanceListByFilter(gLBalanceListFilterModel, token).ResultData);
				}
				GLBalanceListFilterModel gLBalanceListFilterModel2 = new GLBalanceListFilterModel();
				gLBalanceListFilterModel2.AccountIDS = (string.IsNullOrWhiteSpace(filter.MAccountIDs) ? new List<string>() : filter.MAccountIDs.Split(',').ToList());
				gLBalanceListFilterModel2.StartPeriod = Convert.ToInt32(filter.MStartPeroid);
				gLBalanceListFilterModel2.EndPeriod = Convert.ToInt32(filter.MEndPeroid);
				gLBalanceListFilterModel2.IsIncludeChildrenAccount = true;
				gLBalanceListFilterModel2.IncludeCheckType = filter.IncludeCheckType;
				gLBalanceListFilterModel2.CheckTypeValueList = filter.CheckTypeValueList;
				IGLExcel sysService2 = ServiceHostManager.GetSysService<IGLExcel>();
				new List<GLVoucherModel>();
				List<GLVoucherViewModel> o2 = new List<GLVoucherViewModel>();
				using (sysService2 as IDisposable)
				{
					o2 = ModelConvertHelper.ConvertToVoucherViewModel(sysService2.GetVoucherListByFilter(gLBalanceListFilterModel2, token).ResultData);
				}
				return ResponseHelper.toJson(new
				{
					BalanceList = MText.JsonEncode(o),
					VoucherList = MText.JsonEncode(o2)
				}, true, null, true);
			}
			return ResponseHelper.toJson(null, false, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetAccountBalance(string token, GLBalanceListFilterModel filter)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				string apiModule = filter.ApiModule;
				if (!string.IsNullOrEmpty(apiModule))
				{
					string text = "";
					switch (apiModule)
					{
					case "Export":
					case "Pivotable":
						text = "Other_ReportsExport";
						break;
					case "Function":
						text = "General_LedgerChange";
						break;
					default:
						text = "General_LedgerView";
						break;
					}
					if (!AccessHelper.HaveAccess(text, token))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
				}
				IGLExcel sysService = ServiceHostManager.GetSysService<IGLExcel>();
				List<GLBalanceModel> obj = new List<GLBalanceModel>();
				using (sysService as IDisposable)
				{
					obj = sysService.GetBalanceListByFilter(filter, token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, false, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetAccountBalanceByTrack(string token, GLBalanceListFilterModel filter)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				filter.Status = "1";
				IGLExcel sysService = ServiceHostManager.GetSysService<IGLExcel>();
				List<GLBalanceModel> obj = new List<GLBalanceModel>();
				using (sysService as IDisposable)
				{
					obj = sysService.GetBalanceListWithTrackByFilter(filter, token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, false, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetVoucherList(string token, GLBalanceListFilterModel filter)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
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
					if (!AccessHelper.HaveAccess("General_Ledger" + text, token))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
				}
				IGLExcel sysService = ServiceHostManager.GetSysService<IGLExcel>();
				List<GLVoucherModel> obj = new List<GLVoucherModel>();
				using (sysService as IDisposable)
				{
					obj = sysService.GetVoucherListByFilter(filter, token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, false, null, true);
		}

		public HttpResponseMessage GetCheckTypeList(string token)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				IGLCheckType sysService = ServiceHostManager.GetSysService<IGLCheckType>();
				List<GLCheckTypeModel> obj = new List<GLCheckTypeModel>();
				using (sysService as IDisposable)
				{
					obj = sysService.GetModelList(null, false, token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, false, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetAccountBalanceReportData(string token, RPTAccountBalanceFilterModel filter)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				if (!AccessHelper.HaveAccess("General_LedgerExport", token))
				{
					return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
				}
				IRPTAccountBalance sysService = ServiceHostManager.GetSysService<IRPTAccountBalance>();
				BizReportModel obj = null;
				using (sysService as IDisposable)
				{
					obj = sysService.GetReportModel(filter, token).ResultData;
				}
				return ResponseHelper.toJson(obj, true, null, true);
			}
			return ResponseHelper.toJson(null, false, null, true);
		}
	}
}
