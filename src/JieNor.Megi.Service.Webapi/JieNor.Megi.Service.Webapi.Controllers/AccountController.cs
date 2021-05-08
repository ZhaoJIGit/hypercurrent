using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
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
                sECLoginModel.Password = MD5Service.MD5Encrypt(password);
                sECLoginModel.IsConsole = true;
                SECLoginResultModel sECLoginResultModel = new SECLoginResultModel
                {
                    IsSuccess = false
                };
                ISECUserAccount sysService = ServiceHostManager.GetSysService<ISECUserAccount>();
                using (sysService as IDisposable)
                {
                    sECLoginResultModel = sysService.Login(sECLoginModel, null).ResultData;
                    //sECLoginResultModel.MFirstName = sECLoginResultModel.MFirstName;
                    //sECLoginResultModel.MLastName = sECLoginResultModel.MLastName;
                    //sECLoginResultModel.MUserName = sECLoginResultModel.MUserName;

                    ContextHelper.ChangeLang(langId, sECLoginResultModel.MAccessToken);
                }
                if (!sECLoginResultModel.IsSuccess)
                {
                    if (!string.IsNullOrWhiteSpace(sECLoginResultModel.Message))
                    {
                        return ResponseHelper.toJson(null, false, sECLoginResultModel.Message, true);
                    }
                    return ResponseHelper.toJson(null, false, "用户名或密码错误", true);
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


        [HttpPost]
        public HttpResponseMessage Register(SECUserModel info)
        {
            if (string.IsNullOrWhiteSpace(info.MEmailAddress) || string.IsNullOrWhiteSpace(info.ProductCode))
            {
                return ResponseHelper.toJson(new { Code = "201" }, false, "邮箱和产品Code不能为空", true);
            }
            info.MLCID = CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null);

            ISECUser _user = ServiceHostManager.GetSysService<ISECUser>();

            MActionResult<OperationResult> result = _user.Register(info, null);
            MLogger.Log(result.Success.ToString() + ":" + result.Message);

            if (result.ResultData.Success)
            {
                return ResponseHelper.toJson(new { Code = "200" }, true, result.ResultData.Message, true);
            }
            else
            {
                return ResponseHelper.toJson(new { Code = result.ResultData.Code }, false, result.ResultData.Message, true);

            }

        }
    }
}
