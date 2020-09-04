using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Service.Web.SEC;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Main.Web
{
    /// <summary>
    /// RegisterHandler 的摘要说明
    /// </summary>
    public class RegisterHandler : IHttpHandler
    {


        public void ProcessRequest(HttpContext context)
        {
            ISECUser _user = new SECUserService();

            context.Response.ContentType = "text/plain";
            string re = "";
            SECUserModel info = new SECUserModel();

            info.MEmailAddress = context.Request.Form["MEmailAddress"];
            info.ProductCode = context.Request.Form["ProductCode"];
            info.MMobilePhone = context.Request.Form["MMobilePhone"];
            info.MFirstName = context.Request.Form["MFirstName"];
            info.MLastName = context.Request.Form["MLastName"];
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            re = serializer.Serialize(info);
            //if (string.IsNullOrWhiteSpace(info.MEmailAddress) || string.IsNullOrWhiteSpace(info.ProductCode))
            //{
            //    JavaScriptSerializer serializer = new JavaScriptSerializer();
            //    re = serializer.Serialize(new
            //    {
            //        Success = false,
            //        Message = "邮箱和产品Code不能为空"
            //    });
            //}
            //else
            //{
            //    info.MLCID = CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null);


            //    MActionResult<OperationResult> result = _user.Register(info, null);
            //    MLogger.Log(result.Success.ToString() + ":" + result.Message);

            //    if (result.ResultData.Success)
            //    {
            //        JavaScriptSerializer serializer = new JavaScriptSerializer();
            //        re = serializer.Serialize(new
            //        {
            //            Success = true,
            //            Code = "200",
            //            Message = "创建成功"
            //        });

            //    }
            //    else
            //    {
            //        JavaScriptSerializer serializer = new JavaScriptSerializer();
            //        re = serializer.Serialize(new
            //        {
            //            Success = false,
            //            Code = result.ResultData.Code,//20001  已注册
            //            Message = result.ResultData.Message
            //        });

            //    }

            //}

            context.Response.Write(re);


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}