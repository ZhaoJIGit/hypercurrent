using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.Service.Webapi.JieNor.Megi.Service.Webapi.Models;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.COM;
using JieNor.Megi.ServiceContract.SEC;
using JieNor.Megi.ServiceContract.SYS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace  JieNor.Megi.Service.Webapi.Controllers
{
	[RoutePrefix("api")]
	public class OrgController : ApiController
    {
		[HttpGet]
		[Route("org/GetOrgList")]
		public dynamic GetOrgList(string token, string name = "", int pageSize = 10, int pageIndex = 0)
		{
			HttpResponseMessage hrm = new HttpResponseMessage()
			{
				StatusCode = HttpStatusCode.Forbidden
			};
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				ICOMAccess accService = ServiceHostManager.GetSysService<ICOMAccess>();

				

				//MLogger.Log("arr_plancode:"+Newtonsoft.Json.JsonConvert.SerializeObject(arr_plancode));

				//List<BASMyHomeModel> list = new List<BASMyHomeModel>();
				IBASOrganisation sysService = ServiceHostManager.GetSysService<IBASOrganisation>();

				var list = sysService.GetOrgList(name,pageIndex,pageSize);
				var reList = list.ResultData.rows.Select(i => new {});

				List<object> list1 = new List<object>();
                foreach (var i in list.ResultData.rows)
                {

					List<PlanModel> arr_plancode = new List<PlanModel>();
					arr_plancode = accService.GetPlanByEmail(i.MEmailAddress).ResultData;

					//标准版
					bool flag_normal = arr_plancode.Select(x => x.Code).Contains("NORMAL");
					//销售
					bool flag_sales = arr_plancode.Select(x => x.Code).Contains("SALES");
					//发票
					bool flag_invoice = arr_plancode.Select(x => x.Code).Contains("INVOICE");


					int type = i.MVersionID;
					if (flag_sales && flag_invoice)
					{
						type = 0;
					}
					else if (flag_sales&&!flag_invoice) {
						type = 2;
					}
					else if ((!flag_sales )&& flag_invoice)
					{
						type = 3;
					}


					var MOrgTypeID = "1";

					if (i.MIsPaid) {
						MOrgTypeID = "3";
						if (i.MExpiredDate<DateTime.Now) {
							MOrgTypeID = "5";
						}
					}
					else if (i.MExpiredDate>DateTime.Now&&!i.MIsPaid) {
						MOrgTypeID = "2";
					}

					list1.Add(new { i.MVersionID, Type = type, i.MIsDelete, i.MEmailAddress, MOrgTypeID, i.IsNew, i.MItemID, i.MCountryName, MExpiredDate = i.MExpiredDate.ToString("yyyy-MM-dd HH:mm:dd"), i.MName, i.MNumber, i.MOrganizationID, MCreateDate = i.MCreateDate.ToString("yyyy-MM-dd HH:mm:dd"), i.MMasterID });
				}
				return ResponseHelper.toJson(new { rows = list1, total = list.ResultData.total }, true, null, true);

				//return ResponseHelper.toJson(list.ResultData, true, null, true);


			}
		
			return hrm;
		}
		[HttpPost]
		[Route("org/ChangeOrg")]

		public HttpResponseMessage ChangeOrg(ChangeOption option)
		{
			if (!string.IsNullOrEmpty(option.token) && ResponseHelper.CheckTokenIsValid(option.token) == LoginStateEnum.Valid)
			{
				try
				{
					IBASOrganisation _user = ServiceHostManager.GetSysService<IBASOrganisation>();

					var list = _user.UpdateStatus(option.mItemID, option.status);

					return ResponseHelper.toJson(list.ResultData, true, null, true);
				}
				catch (Exception ex)
				{
					return ResponseHelper.toJson(null, false, ex.Message, true);
				}
			}
			HttpResponseMessage hrm = new HttpResponseMessage()
			{

				StatusCode = HttpStatusCode.Forbidden
			};
			return hrm;
		}
		[HttpPost]
		[Route("org/renew")]

		public HttpResponseMessage Renew(RenewOption option)
		{
			if (!string.IsNullOrEmpty(option.token) && ResponseHelper.CheckTokenIsValid(option.token) == LoginStateEnum.Valid)
			{
				try
				{
					IBASOrganisation _user = ServiceHostManager.GetSysService<IBASOrganisation>();

					var list = _user.Renew(option.mItemID, option.ExpiredDate.Date.AddDays(1));

					return ResponseHelper.toJson(list.ResultData, true, null, true);
				}
				catch (Exception ex)
				{
					return ResponseHelper.toJson(null, false, ex.Message, true);
				}
			}
			HttpResponseMessage hrm = new HttpResponseMessage()
			{

				StatusCode = HttpStatusCode.Forbidden
			};
			return hrm;
		}

		[HttpGet]
		[Route("org/GetOrderList")]
		public dynamic GetOrderList(string token, string orgId, int pageSize = 10, int pageIndex = 0)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				//List<BASMyHomeModel> list = new List<BASMyHomeModel>();
				ISYSOrder sysService = ServiceHostManager.GetSysService<ISYSOrder>();
				MLogger.Log("OrgController:1");

				var list = sysService.GetOrderList(orgId);
				var reList = list.ResultData.rows.Select(i => new { 
					//i.MIsDelete, 
					//i.IsNew, 
					i.MItemID, 
					i.MOrgID,
					i.MActualAmount,
					i.MAmount,
					//i.MPayAccountType,
					i.MPayType,
					i.MStatus,
					i.MSubmitTime,
					MPayTime = i.MPayTime==DateTime.MinValue||i.MPayTime==Convert.ToDateTime("1900-01-01 00:00") ?"": i.MPayTime?.ToString("yyyy-MM-dd HH:mm"), 
					i.MNumber, 
					MCreateDate = i.MSubmitTime.ToString("yyyy-MM-dd HH:mm") });

				return ResponseHelper.toJson(new { rows = reList, total = list.ResultData.total }, true, null, true);

			}
			HttpResponseMessage hrm = new HttpResponseMessage()
			{

				StatusCode = HttpStatusCode.Forbidden
			};
			return hrm;
		}


	}
}
