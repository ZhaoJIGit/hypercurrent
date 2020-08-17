using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.AutoManager;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.REG;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using JieNor.Megi.Tools;
using System.Linq;
using JieNor.Megi.EntityModel.BD.AccountItem;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
    public class SetupController : GoControllerBase
    {
        private IBASOrganisation _org = null;

        private IBDAccount _acct = null;
        private IREGGlobalization _glob = null;

        private IBASMyHome _myHome = null;

        private IBDAccountMatchLog _matchLog;

        public SetupController(IBASOrganisation org, IBDAccount acct, IREGGlobalization glob, IBASMyHome myHome, IBDAccountMatchLog matchLog)
        {
            _org = org;
            _acct = acct;
            _glob = glob;
            _myHome = myHome;
            _matchLog = matchLog;
        }

        public ActionResult OrgSetting(int? id, int? version)
        {


            string orgId = (id.HasValue && id == 0) ? string.Empty : base.MContext.MOrgID;
            SetWizardInfo(ModuleEnum.Sales, BASOrgScheduleTypeEnum.OrgSetting, orgId, version);
            return base.View();
        }

        public ActionResult FinancialSetting()
        {
            MContext mContext = ContextHelper.MContext;
            mContext.MAppID = "1";
            ContextHelper.MContext = mContext;
            SetWizardInfo(ModuleEnum.Sales, BASOrgScheduleTypeEnum.FinancialSetting, null, null);
            _org.UpdateRegProgress(BASOrgScheduleTypeEnum.FinancialSetting, null);
            base.ViewData["IsCurrencyEnabled"] = true;
            DateTime dateNow = base.MContext.DateNow;
            int year = dateNow.Year;
            dateNow = base.MContext.DateNow;
            DateTime dateTime = new DateTime(year, dateNow.Month, 1);
            dateTime = dateTime.AddDays(-1.0);
            base.ViewData["LastDayLastMonth"] = dateTime;
            //if (_003C_003Eo__7._003C_003Ep__0 == null)
            //{
            //	_003C_003Eo__7._003C_003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Lang", typeof(SetupController), new CSharpArgumentInfo[2]
            //	{
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            //	}));
            //}
            //_003C_003Eo__7._003C_003Ep__0.Target(_003C_003Eo__7._003C_003Ep__0, base.get_ViewBag(), mContext.MLCID);

            ViewBag.Lang = mContext.MLCID;
            return base.View();
        }

        public ActionResult TaxRateSetting()
        {
            SetWizardInfo(ModuleEnum.Sales, BASOrgScheduleTypeEnum.TaxRateSetting, null, null);
            _org.UpdateRegProgress(BASOrgScheduleTypeEnum.TaxRateSetting, null);
            return base.MView();
        }

        public ActionResult GLFinish()
        {
            SetWizardInfo(ModuleEnum.Sales, BASOrgScheduleTypeEnum.GLFinish, null, null);
            _org.UpdateRegProgress(BASOrgScheduleTypeEnum.GLFinish, null);
            return base.View();
        }

        public ActionResult CreateOrgSuccess()
        {
            _org.UpdateRegProgress(BASOrgScheduleTypeEnum.Success, null);
            BASOrganisationManager.ChangeOrgId(base.MContext.MOrgID);
            string str = base.HttpContext.Request.QueryString["bti"];
            string str2 = base.HttpContext.Request.QueryString["lang"];
            return Redirect("/?bti=" + str + "&lang=" + str2);
        }

        public ActionResult GLChartOfAccount()
        {
            var arr_plancode = AccessHelper.GetPlan().Select(x => x.Code);
            //标准版
            bool flag_normal = arr_plancode.Contains("NORMAL");
            //销售
            bool flag_sales = arr_plancode.Contains("SALES");
            //发票
            bool flag_invoice = arr_plancode.Contains("INVOICE");
            List<BDAccountGroupEditModel> resultData = _acct.GetBDAccountGroupList("", null).ResultData;
            base.ViewData["AccountGroup"] = resultData;
            base.ViewData["IsGLPermission"] = true;
            base.ViewData["AccountStandard"] = base.MContext.MAccountTableID;
            base.ViewData["IsCreate"] = true;
            //if (_003C_003Eo__11._003C_003Ep__0 == null)
            //{
            //	_003C_003Eo__11._003C_003Ep__0 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ExistMatchLog", typeof(SetupController), new CSharpArgumentInfo[2]
            //	{
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            //	}));
            //}
            //_003C_003Eo__11._003C_003Ep__0.Target(_003C_003Eo__11._003C_003Ep__0, base.get_ViewBag(), _matchLog.ExistsByFilter(new SqlWhere().Equal("MMatchResult", 3), null).ResultData);
            ViewBag.ExistMatchLog = _matchLog.ExistsByFilter(new SqlWhere().Equal("MMatchResult", 3), null).ResultData;


            if (base.MContext.MAccountTableID == "3")
            {
                OperationResult resultData2 = _acct.CheckCustomAccountIsFinish(null).ResultData;
                base.ViewData["IsCreate"] = resultData2.Success;
                if (resultData2.Success)
                {
                    resultData2 = _acct.CheckCustomAccountIsMatch(null).ResultData;
                    base.ViewData["IsMatch"] = resultData2.Success;
                }
                else
                {
                    base.ViewData["IsMatch"] = false;
                }
            }
            base.ViewData["SysVersion"] = base.MContext.MOrgVersionID;
            SetWizardInfo(ModuleEnum.GL, BASOrgScheduleTypeEnum.GLChartOfAccount, null, null);
            _org.UpdateRegProgress(BASOrgScheduleTypeEnum.GLChartOfAccount, null);

            if (!flag_normal)
            {
                return base.RedirectToAction("GLOpeningBalance");
            }
            else
            {
                return base.View();
            }
        }

        public ActionResult GLImpartInitBillBalance()
        {
            SetWizardInfo(ModuleEnum.GL, BASOrgScheduleTypeEnum.GLChartOfAccount, null, null);
            return base.View();
        }

        public ActionResult GLOpeningBalance()
        {
            var arr_plancode = AccessHelper.GetPlan().Select(x => x.Code);
            //标准版
            bool flag_normal = arr_plancode.Contains("NORMAL");
            //销售
            bool flag_sales = arr_plancode.Contains("SALES");
            //发票
            bool flag_invoice = arr_plancode.Contains("INVOICE");

            MContext mContext = ContextHelper.MContext;
            base.ViewData["ForbidEdit"] = Convert.ToInt32(mContext.MInitBalanceOver);
            ViewDataDictionary viewData = base.ViewData;
            DateTime dateTime = mContext.DateNow;
            object arg = dateTime.Year;
            dateTime = mContext.DateNow;
            viewData["Period"] = arg + "-" + dateTime.Month;
            ViewDataDictionary viewData2 = base.ViewData;
            dateTime = mContext.MGLBeginDate;
            viewData2["GLBeginMonth"] = dateTime.Month;
            base.ViewData["BaseCurrency"] = mContext.MBasCurrencyID;
            SetWizardInfo(ModuleEnum.GL, BASOrgScheduleTypeEnum.GLOpeningBalance, null, null);
            _org.UpdateRegProgress(BASOrgScheduleTypeEnum.GLOpeningBalance, null);
            base.ViewData["IsGLPermission"] = true;
            base.ViewData["AccountStandard"] = mContext.MAccountTableID;
            base.ViewData["IsCanEditBankBalance"] = true;

            if (!flag_normal)
            {
                MActionResult<TrialInitBalanceModel> data = _acct.TrialInitBalance(null);
                _acct.InitBalanceFinish(null);

                return base.RedirectToAction("GLFinish");
            }
            else
            {
                return base.View();
            }

        }

        public ActionResult GLSuccess()
        {
            _org.UpdateRegProgress(BASOrgScheduleTypeEnum.GLSuccess, null);
            BASOrganisationManager.ChangeOrgId(base.MContext.MOrgID);
            string str = base.HttpContext.Request.QueryString["bti"];
            string str2 = base.HttpContext.Request.QueryString["lang"];
            return Redirect("/?bti=" + str + "&lang=" + str2);
        }

        public JsonResult UpdateOrgDetail(BASOrgInfoModel model)
        {
            OperationResult operationResult = null;
            string orgId = model.MOrgID;
            if (string.IsNullOrWhiteSpace(model.MOrgID))
            {
                BASOrganisationModel bASOrganisationModel = new BASOrganisationModel();
                bASOrganisationModel.MName = model.MDisplayName;
                bASOrganisationModel.MLegalTradingName = model.MLagalTrading;
                bASOrganisationModel.MOrgTypeID = model.MOrgTypeID;
                bASOrganisationModel.MOrgBusiness = model.MOrgDesc;
                bASOrganisationModel.MMasterID = base.MContext.MUserID;
                bASOrganisationModel.MCountryID = model.MCountryID;
                bASOrganisationModel.MStateID = model.MStateID;
                bASOrganisationModel.MCityID = model.MCityID;
                bASOrganisationModel.MStreet = model.MStreet;
                bASOrganisationModel.MPostalNo = model.MPostalNo;
                bASOrganisationModel.MVersionID = model.MVersionID;
                MContext mContext = ContextHelper.MContext;
                mContext.MOrgVersionID = model.MVersionID;
                ContextHelper.MContext = mContext;
                operationResult = _myHome.OrgRegisterForTry(bASOrganisationModel, null).ResultData;
                orgId = operationResult.ObjectID;
            }
            else
            {
                operationResult = _org.UpdateOrgInfo(model, null).ResultData;
            }

            if (operationResult != null && !operationResult.Success)
            {
                return base.Json(operationResult);
            }
            BASOrganisationManager.ChangeOrgId(orgId);
            _org.UpdateRegProgress(BASOrgScheduleTypeEnum.OrgSetting, null);
            return base.Json(operationResult);
        }

        private void SetWizardInfo(ModuleEnum module, BASOrgScheduleTypeEnum progress, string orgId = null, int? version = default(int?))
        {
            //if (_003C_003Eo__16._003C_003Ep__0 == null)
            //{
            //	_003C_003Eo__16._003C_003Ep__0 = CallSite<Func<CallSite, object, ModuleEnum, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Module", typeof(SetupController), new CSharpArgumentInfo[2]
            //	{
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            //	}));
            //}
            //_003C_003Eo__16._003C_003Ep__0.Target(_003C_003Eo__16._003C_003Ep__0, base.get_ViewBag(), module);
            ViewBag.Module = module;


            //if (_003C_003Eo__16._003C_003Ep__1 == null)
            //{
            //	_003C_003Eo__16._003C_003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "OrgCode", typeof(SetupController), new CSharpArgumentInfo[2]
            //	{
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            //	}));
            //}
            //_003C_003Eo__16._003C_003Ep__1.Target(_003C_003Eo__16._003C_003Ep__1, base.get_ViewBag(), (orgId != null) ? orgId : base.MContext.MOrgID);
            ViewBag.OrgCode = (orgId != null) ? orgId : base.MContext.MOrgID;


            //if (_003C_003Eo__16._003C_003Ep__2 == null)
            //{
            //	_003C_003Eo__16._003C_003Ep__2 = CallSite<Func<CallSite, object, BASOrgScheduleTypeEnum, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "SetupProgress", typeof(SetupController), new CSharpArgumentInfo[2]
            //	{
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            //	}));
            //}
            //_003C_003Eo__16._003C_003Ep__2.Target(_003C_003Eo__16._003C_003Ep__2, base.get_ViewBag(), progress);
            ViewBag.SetupProgress = progress;

            //if (_003C_003Eo__16._003C_003Ep__3 == null)
            //{
            //	_003C_003Eo__16._003C_003Ep__3 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ModuleId", typeof(SetupController), new CSharpArgumentInfo[2]
            //	{
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            //	}));
            //}
            //_003C_003Eo__16._003C_003Ep__3.Target(_003C_003Eo__16._003C_003Ep__3, base.get_ViewBag(), (progress < (BASOrgScheduleTypeEnum)10) ? 1 : 2);
            ViewBag.ModuleId = (progress < (BASOrgScheduleTypeEnum)10) ? 1 : 2;

            //if (_003C_003Eo__16._003C_003Ep__4 == null)
            //{
            //	_003C_003Eo__16._003C_003Ep__4 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Version", typeof(SetupController), new CSharpArgumentInfo[2]
            //	{
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            //	}));
            //}
            //_003C_003Eo__16._003C_003Ep__4.Target(_003C_003Eo__16._003C_003Ep__4, base.get_ViewBag(), version.HasValue ? version.Value : 0);
            ViewBag.Version = version.HasValue ? version.Value : 0;


            if (version.HasValue)
            {
                //if (_003C_003Eo__16._003C_003Ep__5 == null)
                //{
                //	_003C_003Eo__16._003C_003Ep__5 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Version", typeof(SetupController), new CSharpArgumentInfo[2]
                //	{
                //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                //	}));
                //}
                //_003C_003Eo__16._003C_003Ep__5.Target(_003C_003Eo__16._003C_003Ep__5, base.get_ViewBag(), version.Value);

                ViewBag.Version = version.Value;
            }
            else if (!string.IsNullOrWhiteSpace(ContextHelper.MContext.MOrgID))
            {
                //if (_003C_003Eo__16._003C_003Ep__6 == null)
                //{
                //	_003C_003Eo__16._003C_003Ep__6 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Version", typeof(SetupController), new CSharpArgumentInfo[2]
                //	{
                //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                //		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                //	}));
                //}
                //_003C_003Eo__16._003C_003Ep__6.Target(_003C_003Eo__16._003C_003Ep__6, base.get_ViewBag(), ContextHelper.MContext.MOrgVersionID);

                ViewBag.Version = ContextHelper.MContext.MOrgVersionID;
            }
        }
    }
}
