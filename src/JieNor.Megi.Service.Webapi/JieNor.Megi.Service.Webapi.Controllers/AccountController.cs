using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.SEC;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class AccountController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage ChangLang(string token, string langId)
		{
			ContextHelper.ChangeLang(langId, token);
			return ResponseHelper.toJson(null, true, null, true);
		}

		[HttpGet]
		public HttpResponseMessage SignIn(string email, string password, string langId = "0x7804")
		{
			try
			{
				SECLoginModel sECLoginModel = new SECLoginModel();
				sECLoginModel.Email = email;
				sECLoginModel.Password = password;
				SECLoginResultModel sECLoginResultModel = new SECLoginResultModel
				{
					IsSuccess = false
				};
				ISECUserAccount sysService = ServiceHostManager.GetSysService<ISECUserAccount>();
				using (sysService as IDisposable)
				{
					sECLoginResultModel = sysService.Login(sECLoginModel, null).ResultData;
					sECLoginResultModel.MFirstName = "";
					sECLoginResultModel.MLastName = "";
					sECLoginResultModel.MUserName = "";
					ContextHelper.ChangeLang(langId, sECLoginResultModel.MAccessToken);
				}
				return ResponseHelper.toJson(sECLoginResultModel, true, "", true);
			}
			catch (Exception ex)
			{
				return ResponseHelper.toJson(null, false, ex.Message, true);
			}
		}

		[HttpGet]
		public HttpResponseMessage CheckTokenIsVaild(string token)
		{
			try
			{
				return ResponseHelper.toJson(new
				{
					status = (int)ResponseHelper.CheckTokenIsValid(token)
				}, true, null, false);
			}
			catch (Exception ex)
			{
				return ResponseHelper.toJson(null, false, ex.Message, true);
			}
		}
	}
}
