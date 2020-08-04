//using JieNor.Megi.Core.Context;
//using JieNor.Megi.Core.DBUtility;
//using JieNor.Megi.DataModel.SEC;
//using JieNor.Megi.EntityModel.Context;
//using JieNor.Megi.ServiceContract.SEC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;

//namespace JieNor.Megi.Main.Web.Controllers
//{
//    public class RegisterController : ApiController
//    {
//        private ISECUser _user;

//        public RegisterController(ISECUser user)
//        {
//            _user = user;
//        }

//        public RegisterResult Post(SECUserModel info)
//        {
//            info.MLCID = CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null);
//            MActionResult<OperationResult> result = _user.Register(info, null);

//            var response = new RegisterResult
//            {

//            };
//            return response;
//        }
//    }

//    public class RegisterResult
//    {
//        public bool Success { get; set; }

//        public string Message { get; set; }
//    }
//}