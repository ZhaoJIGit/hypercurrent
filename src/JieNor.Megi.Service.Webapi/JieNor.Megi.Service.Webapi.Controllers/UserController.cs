using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Service.Webapi.Helper;
using JieNor.Megi.Service.Webapi.JieNor.Megi.Service.Webapi.Models;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
    [RoutePrefix("api")]

    public class UserController : ApiController
    {
        [HttpGet]
        [Route("user/GetUserList")]

        public HttpResponseMessage GetUserList([FromUri] UserOption option)
        {
            if (!string.IsNullOrEmpty(option.token) && ResponseHelper.CheckTokenIsValid(option.token) == LoginStateEnum.Valid)
            {
                try
                {
                    ISECUser _user = ServiceHostManager.GetSysService<ISECUser>();

                    var list = _user.GetUserList(option.email,option.name,option.pageIndex,option.pageSize);
                    var reList = list.ResultData.rows.Select(i=>new { i.MIsDelete, i.IsNew,i.MItemID, MIsArchive=i.MIsActive,i.MFirstName,i.MLastName,i.MEmailAddress, MCreateDate=i.MCreateDate.ToString("yyyy-MM-dd HH:mm:dd"),i.MUserID  });

                    return ResponseHelper.toJson(new { rows= reList , total = list.ResultData.total}, true, null, true);
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
        [Route("user/ChangeUser")]

        public HttpResponseMessage ChangeUser(ChangeOption option)
        {
            if (!string.IsNullOrEmpty(option.token) && ResponseHelper.CheckTokenIsValid(option.token) == LoginStateEnum.Valid)
            {
                try
                {
                    ISECUser _user = ServiceHostManager.GetSysService<ISECUser>();

                    var list = _user.UpdateStatus(option.mItemID, option.status);

                    return ResponseHelper.toJson(list.ResultData, true, null, true);
                }
                catch (Exception ex)
                {
                    return ResponseHelper.toJson(null, false, ex.Message, true);
                }
            }
            HttpResponseMessage hrm = new HttpResponseMessage() {
            
             StatusCode=HttpStatusCode.Forbidden};
            return hrm;// ResponseHelper.toJson(null, false, "token无效", true);
        }

    }
}
