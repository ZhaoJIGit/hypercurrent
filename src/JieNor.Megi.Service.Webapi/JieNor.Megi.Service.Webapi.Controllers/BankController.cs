using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ExportUtility.Converter;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class BankController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetTransactionList(string token, IVAccountTransactionsListFilterModel filter)
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
				if (string.IsNullOrWhiteSpace(filter.Sort))
				{
					filter.Sort = "MBizDate";
				}
				List<TransactionListRowModel> list = new List<TransactionListRowModel>();
				IIVTransactions sysService = ServiceHostManager.GetSysService<IIVTransactions>();
				using (sysService as IDisposable)
				{
					DateTime value;
					if (filter != null && filter.StartDate.HasValue)
					{
						value = filter.StartDate.Value;
						int year = value.Year;
						value = filter.StartDate.Value;
						int month = value.Month;
						value = filter.StartDate.Value;
						filter.StartDate = new DateTime(year, month, value.Day);
					}
					if (filter != null && filter.EndDate.HasValue)
					{
						value = filter.EndDate.Value;
						int year2 = value.Year;
						value = filter.EndDate.Value;
						int month2 = value.Month;
						value = filter.EndDate.Value;
						filter.EndDate = new DateTime(year2, month2, value.Day);
					}
					IVAccountTransactionsExportModel resultData = sysService.GetTransactionListForExport(filter, token).ResultData;
					TransationConverter transationConverter = new TransationConverter(token);
					list.AddRange(transationConverter.ToExportDataList(resultData));
				}
				if (list == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				list.ForEach(delegate(TransactionListRowModel x)
				{
					x.MDesc = (string.IsNullOrWhiteSpace(x.MDesc) ? "" : x.MDesc.Replace('"', '\''));
					x.MDescription = (string.IsNullOrWhiteSpace(x.MDescription) ? "" : x.MDescription.Replace('"', '\''));
					x.MReference = (string.IsNullOrWhiteSpace(x.MReference) ? "" : x.MReference.Replace('"', '\''));
				});
				return ResponseHelper.toJson(list, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		[HttpPost]
		public HttpResponseMessage GetBankAccountList(string token, IVVerificationFilterModel filter, bool showOrgName = false)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				if (filter != null)
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
							text = "View";
							break;
						default:
							text = "View";
							break;
						}
						if (!AccessHelper.HaveAccess(new List<string>
						{
							"BankAccount" + text,
							"Invoice_Sales" + text
						}, token, "and"))
						{
							return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
						}
					}
				}
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				List<NameValueModel> list2 = new List<NameValueModel>();
				IBDBankAccount sysService = ServiceHostManager.GetSysService<IBDBankAccount>();
				using (sysService as IDisposable)
				{
					list2 = sysService.GetSimpleBankAccountList(filter.MOrgIDs, token).ResultData;
				}
				if (list == null)
				{
					string message = "获取数据失败";
					return ResponseHelper.toJson(null, true, message, true);
				}
				if (list2 != null)
				{
					foreach (NameValueModel item in list2)
					{
						list.Add(new KeyValuePair<string, string>(item.MTag, (!showOrgName) ? item.MName : $"{item.MName}({item.MValue})"));
					}
				}
				return ResponseHelper.toJson(list, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
