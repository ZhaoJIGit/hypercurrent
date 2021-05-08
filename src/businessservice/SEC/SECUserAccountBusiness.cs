using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.SEC;

namespace JieNor.Megi.BusinessService.SEC
{
    public class SECUserAccountBusiness : ISECUserAccountBusiness
    {
        private readonly SECUserRepository dal = new SECUserRepository();

        public MContext GetMContextByOrgID(MContext MContext)
        {
            BASMyHomeBusiness bASMyHomeBusiness = new BASMyHomeBusiness();
            return bASMyHomeBusiness.GetMContextByOrgID(MContext);
        }

        public SECUserModel GetUserModel(string email, string password)
        {
            return dal.GetModel(email, password);
        }

        [NoAuthorization]
        public SECLoginResultModel Login(MContext ctx, SECLoginModel model)
        {
            SECUserModel model2 = dal.GetModel(ctx, model);
            SECLoginResultModel sECLoginResultModel = new SECLoginResultModel
            {
                IsSuccess = false
            };
            if (model.IsConsole)
            {
                if (!model2.IsConsole)
                {
                    sECLoginResultModel.Message = "Œﬁ»®∑√Œ ";
                    return sECLoginResultModel;
                }
            }
            MContext mContext;
            UserLoginRedirectEnum userLoginRedirectEnum;
            int num2;
            if (model2 != null)
            {
                sECLoginResultModel.IsSuccess = true;
                sECLoginResultModel.MFirstName = model2.MFirstName;
                sECLoginResultModel.MLastName = model2.MLastName;
                sECLoginResultModel.MMobilePhone = model2.MMobilePhone;
                sECLoginResultModel.MUserName = GlobalFormat.GetUserName(model2.MFirstName, model2.MLastName, null);
                MContext contextByUserId = ContextHelper.GetContextByUserId(model2.MItemID);
                mContext = (contextByUserId ?? new MContext());
                int mAccessTokenValidMinute = int.Parse(ContextHelper.ExpireTime ?? ContextHelper.DefaultExpireTime);
                string mAccessToken = AuthorizationManager.GenerateAccessToken();
                mContext.MUserID = model2.MItemID;
                mContext.MFirstName = model2.MFirstName;
                mContext.MLastName = model2.MLastName;
                mContext.MMobilePhone = model2.MMobilePhone;
                mContext.MRegDate = model2.MCreateDate;
                mContext.MEmail = model.Email;
                mContext.MUserIP = model.UserIp;
                mContext.MExistsOrg = ModelInfoManager.ExistsByFilter<SECOrgUserModel>(ctx, new SqlWhere().Equal("MUserID", model2.MItemID).Equal("MIsDelete", 0).GreaterThen("Length(ifnull(MOrgID,''))", 0));
                mContext.MAccessToken = mAccessToken;
                mContext.MBrowserTabIndex = UUIDHelper.GetGuid();
                sECLoginResultModel.MWhenILogIn = model2.MAppID;
                if (!string.IsNullOrEmpty(model.OrgId))
                {
                    sECLoginResultModel.MIsDeletedOrgID = !ModelInfoManager.ExistsByFilter<BASOrganisationModel>(ctx, new SqlWhere().Equal("MItemID", model.OrgId).Equal("MIsDelete", 0));
                    if (sECLoginResultModel.MIsDeletedOrgID)
                    {
                        model.OrgId = "";
                    }
                    else
                    {
                        mContext.MOrgID = model.OrgId;
                    }
                }
                int num = 1;
                int.TryParse(sECLoginResultModel.MWhenILogIn, out num);
                mContext.LoginRedirect = (string.IsNullOrEmpty(sECLoginResultModel.MWhenILogIn) ? 1 : num);
                if (!mContext.MExistsOrg)
                {
                    mContext.LoginRedirect = 1;
                }
                mContext.MLCID = (string.IsNullOrEmpty(model.MLCID) ? (string.IsNullOrEmpty(mContext.MLCID) ? mContext.MDefaultLocaleID : mContext.MLCID) : model.MLCID);
                mContext.MAccessTokenValidMinute = mAccessTokenValidMinute;
                mContext.MLogout = false;
                mContext.MOrgListShowType = model2.MOrgListShowType;
                model2.MLastLoginDate = mContext.DateNow;
                model2.MLastLoginLCID = mContext.MLCID;
                mContext.MUserCreateDate = model2.MCreateDate;
                mContext.MIsHadAddOrgAuth = model2.MIsHadAddOrgAuth;
                ContextHelper.MContext = mContext;
                sECLoginResultModel.MLastLoginAppCode = model2.MLastLoginAppID;
                sECLoginResultModel.MUserID = model2.MItemID;
                dal.InsertOrUpdate(ctx, model2, null);
                sECLoginResultModel.MAccessToken = mAccessToken;
                sECLoginResultModel.MRedirectURL = (string.IsNullOrEmpty(model.RedirectUrl) ? ContextHelper.GetRedirectUrl(mContext)[0] : model.RedirectUrl);
                sECLoginResultModel.MRelogin = model.Relogin;
                sECLoginResultModel.MExistsOrg = mContext.MExistsOrg;
                sECLoginResultModel.MBrowserTabIndex = mContext.MBrowserTabIndex;
                bool flag = mContext.MExpireDate.ToDayLastSecond() >= mContext.DateNow;
                if ((mContext.MExistsOrg && !string.IsNullOrEmpty(mContext.MOrgID) && !string.IsNullOrEmpty(model.OrgId) && mContext.MOrgID != model.OrgId) & flag)
                {
                    new BASOrganisationBusiness().ChangeOrgById(mContext, model.OrgId);
                    sECLoginResultModel.MOrgID = model.OrgId;
                }
                else if ((mContext.MExistsOrg && !string.IsNullOrEmpty(mContext.MOrgID)) & flag)
                {
                    new BASOrganisationBusiness().ChangeOrgById(mContext, mContext.MOrgID);
                    sECLoginResultModel.MOrgID = mContext.MOrgID;
                }
                sECLoginResultModel.MActiveLocaleIDS = mContext.MActiveLocaleIDS;
                if (!string.IsNullOrWhiteSpace(sECLoginResultModel.MOrgID))
                {
                    string mWhenILogIn = sECLoginResultModel.MWhenILogIn;
                    userLoginRedirectEnum = UserLoginRedirectEnum.Go;
                    if (mWhenILogIn == userLoginRedirectEnum.ToString() && mContext.MActiveLocaleIDS != null && mContext.MActiveLocaleIDS.Count != 0)
                    {
                        num2 = ((!mContext.MActiveLocaleIDS.Contains(mContext.MLCID)) ? 1 : 0);
                        goto IL_046b;
                    }
                }
                num2 = 0;
                goto IL_046b;
            }
            goto IL_057e;
            IL_046b:
            if (num2 != 0)
            {
                mContext.MLCID = mContext.MActiveLocaleIDS[0];
                ContextHelper.MContext = mContext;
            }
            if (!string.IsNullOrWhiteSpace(sECLoginResultModel.MOrgID))
            {
                BASOrganisationModel organisationInSysDB = new BASOrganisationBusiness().GetOrganisationInSysDB(ctx, sECLoginResultModel.MOrgID);
                if (organisationInSysDB == null || organisationInSysDB.MExpiredDate.ToDayLastSecond() < mContext.DateNow)
                {
                    SECLoginResultModel sECLoginResultModel2 = sECLoginResultModel;
                    userLoginRedirectEnum = UserLoginRedirectEnum.My;
                    sECLoginResultModel2.MWhenILogIn = userLoginRedirectEnum.ToString();
                    sECLoginResultModel.MRedirectURL = ServerHelper.MyServer;
                    mContext.LoginRedirect = 1;
                }
                else
                {
                    SECUserBusiness sECUserBusiness = new SECUserBusiness();
                    OperationResult operationResult = sECUserBusiness.ValidateCreateOrgAuth(mContext, model2.MItemID, sECLoginResultModel.MOrgID);
                    if (!operationResult.Success)
                    {
                        SECLoginResultModel sECLoginResultModel3 = sECLoginResultModel;
                        userLoginRedirectEnum = UserLoginRedirectEnum.My;
                        sECLoginResultModel3.MWhenILogIn = userLoginRedirectEnum.ToString();
                        sECLoginResultModel.MRedirectURL = ServerHelper.MyServer;
                        mContext.LoginRedirect = 1;
                    }
                }
            }
            sECLoginResultModel.MLocaleID = mContext.MLCID;
            goto IL_057e;
            IL_057e:
            return sECLoginResultModel;
        }


    }
}
