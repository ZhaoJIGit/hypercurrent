using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.PA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class SalaryController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetSalaryList(string token)
		{
			if (!string.IsNullOrEmpty(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				new PAPayRunListFilterModel();
				return ResponseHelper.toJson(GetPayRunList(token), false, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}

		private List<PAPayRunListModel> GetPayRunList(string token)
		{
			List<PAPayRunListModel> list = new List<PAPayRunListModel>();
			IPASalaryPayment sysService = ServiceHostManager.GetSysService<IPASalaryPayment>();
			PAPayRunListFilterModel filter = new PAPayRunListFilterModel();
			using (sysService as IDisposable)
			{
				return sysService.GetPayRunList(filter, token).ResultData;
			}
		}

		private List<PAPayRunListModel> GetPayRunList(PASalaryPaymentListFilterModel filter, string token)
		{
			List<PAPayRunListModel> list = new List<PAPayRunListModel>();
			IPASalaryPayment sysService = ServiceHostManager.GetSysService<IPASalaryPayment>();
			PAPayRunListFilterModel pAPayRunListFilterModel = new PAPayRunListFilterModel();
			pAPayRunListFilterModel.StartDate = filter.StartDate;
			pAPayRunListFilterModel.EndDate = filter.EndDate;
			pAPayRunListFilterModel.IsMorePayPunList = true;
			pAPayRunListFilterModel.PageIndex = filter.PageIndex;
			pAPayRunListFilterModel.PageSize = filter.PageSize;
			using (sysService as IDisposable)
			{
				DataGridJson<PAPayRunListModel> resultData = sysService.GetPayRunListPage(pAPayRunListFilterModel, token).ResultData;
				return (resultData != null) ? resultData.rows : list;
			}
		}

		[HttpPost]
		public HttpResponseMessage GetSalaryDetailList(string token, PASalaryPaymentListFilterModel filter)
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
						text = "View";
						break;
					default:
						text = "View";
						break;
					}
					if (!AccessHelper.HaveAccess("PayRun" + text, token))
					{
						return ResponseHelper.toJson(null, false, "没有使用这个功能的权限！", true);
					}
				}
				List<PAPayRunListModel> payRunList = GetPayRunList(filter, token);
				List<PASalaryPaymentListModel> list = new List<PASalaryPaymentListModel>();
				IPASalaryPayment sysService = ServiceHostManager.GetSysService<IPASalaryPayment>();
				using (sysService as IDisposable)
				{
					if (payRunList != null && payRunList.Count() > 0)
					{
						foreach (PAPayRunListModel item in payRunList)
						{
							filter.PayRunID = item.MID;
							List<PASalaryPaymentListModel> rows = sysService.GetSalaryPaymentList(filter, token).ResultData.rows;
							if (rows != null)
							{
								foreach (PASalaryPaymentListModel item2 in rows)
								{
									item2.MDate = item.MDate;
								}
								list.AddRange(rows);
							}
						}
					}
					if (!list.Any())
					{
						list.Add(new PASalaryPaymentListModel());
					}
					list[0].MultiLangGroupNameList = sysService.GetSalaryHeader(token).ResultData;
				}
				return ResponseHelper.toJson(list, true, null, true);
			}
			return ResponseHelper.toJson(null, true, null, true);
		}
	}
}
