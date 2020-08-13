using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.SEC;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Main.Web.Controllers
{
    public class AccountController : MainControllerBase
    {
        private ISECUser _user;

        private IBASCountryRegion _countryRegion;

        private ISECSendLinkInfo _send;

        private IBASOrganisation _org;

        private ISECPermission _perm;

        public AccountController(ISECUser user, IBASCountryRegion countryRegion, ISECSendLinkInfo send, ISECPermission perm, IBASOrganisation org)
        {
            _user = user;
            _countryRegion = countryRegion;
            _send = send;
            _org = org;
            _perm = perm;
        }

        [HttpGet]
        public ActionResult Register(string token)
        {

            ViewBag.options = selectCountryOptions();
            base.ViewData["userInfo"] = new SECInviteUserInfoModel();
            base.ViewData["orgName"] = string.Empty;
            if (!string.IsNullOrEmpty(token))
            {
                string linkExpireUrl = $"{ServerHelper.LoginServer}/LinkExpire/Expire";
                try
                {
                    string[] str = DESEncrypt.Decrypt(token).Split('|');
                    if (str.Length == 3)
                    {
                        if (!_send.IsValidLink(str[2], null).ResultData)
                        {
                            return Redirect(linkExpireUrl);
                        }
                        SECInviteUserInfoModel userInfo = _perm.GetUserInviteInfo(str[0], null).ResultData;
                        if (userInfo != null && !userInfo.MIsTemp)
                        {
                            ServerHelper.DefaultUserEmail = userInfo.MEmail;
                            return Redirect($"{ServerHelper.LoginServer}?token={token}");
                        }
                        BASOrganisationModel sysOrgModel = _org.GetModelById(str[1], null).ResultData;
                        base.ViewData["userID"] = str[0];
                        base.ViewData["orgId"] = str[1];
                        base.ViewData["orgName"] = sysOrgModel.MName;
                        base.ViewData["userInfo"] = userInfo;
                        base.ViewData["SendLinkID"] = str[2];
                    }
                }
                catch
                {
                    return Redirect(linkExpireUrl);
                }
            }
            return base.View();
        }

        public ActionResult JoinSuccess(string id)
        {
            BASOrganisationModel sysOrgModel = _org.GetModelById(id, null).ResultData;
            if (sysOrgModel == null)
            {
                return Redirect("/Account/Success");
            }

            ViewBag.OrgName = sysOrgModel.MName;
            return base.View();
        }

        public ActionResult Success(string id)
        {
            string email = System.Web.HttpContext.Current.Request["email"];
            if (!string.IsNullOrEmpty(email))
            {
                ServerHelper.DefaultUserEmail = email;
            }
            if (string.IsNullOrEmpty(id))
            {
                return base.View();
            }
            return Redirect("/Account/JoinSuccess/" + id);
        }

        public ActionResult RegisterFail()
        {

            ViewBag.ErrorTitle = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.User, "registerFailTitle", "Sorry, registered Failed!");
            ViewBag.ErrorDetail = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.User, "registerFailDetail", "This email has been registered, please use the other email to register!");

            return base.View("Error");
        }

        public JsonResult UserRegister(SECUserModel info)
        {
            info.MLCID = CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null);
            MActionResult<OperationResult> result = _user.Register(info, null);
            return Json(result);
        }

        [HttpPost]
        public JsonResult Register(SECUserModel info)
        {
           
            //SECUserModel info = new SECUserModel();

            //info.MEmailAddress = sECUser.MEmailAddress;
            //info.MPassWord = sECUser.MPassWord;
            //info.MMobilePhone = sECUser.MMobilePhone;
            //info.MFirstName = sECUser.MFirstName;
            //info.MLastName = sECUser.MLastName;
            //info.PlanCode = sECUser.ProductCode;
            //info.Payment = "";


            info.MLCID = CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null);

            MLogger.Log(info.ProductCode);

            MActionResult<OperationResult> result = _user.Register(info, null);

            if (result.Success)
                return Json(new
                {
                    Success = true,
                    Message = "创建成功"
                }, "application/json");

            return Json(new
            {
                Success = false,
                Message = result.Message
            }, "application/json");
        }

        public static MvcHtmlString selectCountryOptions()
        {
            IBASCountryRegion cr = ServiceHostManager.GetSysService<IBASCountryRegion>();
            using (cr as IDisposable)
            {
                List<BASCountryRegionModel> list = cr.GetModelList(null, false, null).ResultData;
                StringBuilder builder = new StringBuilder();
                if (list != null)
                {
                    foreach (BASCountryRegionModel item2 in list)
                    {
                        foreach (MultiLanguageFieldList item3 in item2.MultiLanguage)
                        {
                            if (item3.MFieldName.EqualsIgnoreCase("MName"))
                            {
                                builder.Append($"<option value=\"{item2.MItemID}\">{item3.MMultiLanguageValue}</option>");
                            }
                        }
                    }
                }
                return new MvcHtmlString(builder.ToString());
            }
        }
    }
    public class SECUserViewModel
    {
        public string ProductCode { get; set; }
        public string Payment { get; set; }


        public string MUserID
        {
            get;
            set;
        }


        public string MEmailAddress
        {
            get;
            set;
        }


        public string MPassWord
        {
            get;
            set;
        }


        public string MMobilePhone
        {
            get;
            set;
        }


        public string MQQNumber
        {
            get;
            set;
        }


        public string MAppID
        {
            get;
            set;
        }


        public DateTime MLastLoginDate
        {
            get;
            set;
        }


        public string MLastLoginOrgID
        {
            get;
            set;
        }


        public string MLastLoginAppID
        {
            get;
            set;
        }


        public string MLastLoginLCID
        {
            get;
            set;
        }


        public bool MIsTemp
        {
            get;
            set;
        }


        public string MFirstName
        {
            get;
            set;
        }

        public string MLastName
        {
            get;
            set;
        }


        public string MCurPassWord
        {
            get;
            set;
        }


        public string MLastLoginOrgName
        {
            get;
            set;
        }


        public string SendLinkID
        {
            get;
            set;
        }


        public bool MPublicProfile
        {
            get;
            set;
        }


        public string MProfileImage
        {
            get;
            set;
        }


        public SECUserlModel SECUserLModel
        {
            get;
            set;
        }


        public bool MIsChangeEmail
        {
            get;
            set;
        }


        public bool MInitBalanceOver
        {
            get;
            set;
        }


        public string DefaultEmail
        {
            get;
            set;
        }


        public string MLCID
        {
            get;
            set;
        }


        public int MOrgListShowType
        {
            get;
            set;
        }

        public string MRole
        {
            get;
            set;
        }

        public string MPosition
        {
            get;
            set;
        }


        public string[] Position
        {
            get;
            set;
        }


        public bool MUserIsActive
        {
            get;
            set;
        }

        public bool MIsArchive
        {
            get;
            set;
        }


        public string MStatus
        {
            get;
            set;
        }


        public bool MIsHadAddOrgAuth
        {
            get;
            set;
        }


    }
}
