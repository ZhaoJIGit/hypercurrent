using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.Service.Webapi.JieNor.Megi.Service.Webapi.Models;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	[RoutePrefix("api")]
	public class OrganizationController : ApiController
	{
		[HttpGet]
		[Route("Organization/GetOrgList")]
		public HttpResponseMessage GetOrgList(string token)
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				List<BASMyHomeModel> list = new List<BASMyHomeModel>();
				IBASMyHome sysService = ServiceHostManager.GetSysService<IBASMyHome>();
				using (sysService as IDisposable)
				{
					list = sysService.GetOrgInfoListByUserID(token).ResultData;
				}
				if (list != null && list.Count() != 0)
				{
					string text = ConfigurationManager.AppSettings["ShowOrgType"];
					switch ((!string.IsNullOrWhiteSpace(text)) ? int.Parse(text) : 0)
					{
					case 1:
						list = (from x in list
						where x.MIsBeta
						select x).ToList();
						break;
					case 2:
						list = (from x in list
						where !x.MIsBeta
						select x).ToList();
						break;
					}
					return ResponseHelper.toJson(list, true, null, true);
				}
				return ResponseHelper.toJson(list, true, null, true);
			}
			HttpResponseMessage hrm = new HttpResponseMessage()
			{

				StatusCode = HttpStatusCode.Forbidden
			};
			return hrm;
		}
		
		[HttpGet]
		[Route("Organization/ChangeOrg")]

		public HttpResponseMessage ChangeOrg(string token, string orgId, string langId = "0x7804")
		{
			if (!string.IsNullOrWhiteSpace(token) && ResponseHelper.CheckTokenIsValid(token) == LoginStateEnum.Valid)
			{
				if (string.IsNullOrWhiteSpace(orgId))
				{
					string message = "没有合法的组织";
					return ResponseHelper.toJson(null, false, message, true);
				}
				ContextHelper.ChangeLang(langId, token);
				IBASOrganisation sysService = ServiceHostManager.GetSysService<IBASOrganisation>();
				using (sysService as IDisposable)
				{
					sysService.ChangeOrgById(orgId, token);
				}
				return ResponseHelper.toJson(null, true, null, true);
			}
			HttpResponseMessage hrm = new HttpResponseMessage()
			{

				StatusCode = HttpStatusCode.Forbidden
			};
			return hrm;
		}
	}
}
