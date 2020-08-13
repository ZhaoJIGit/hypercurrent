using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Mail;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Hosting;

namespace JieNor.Megi.BusinessService.SEC
{
	public class SECUserBusiness : APIBusinessBase<SECUserModel>, ISECUserBusiness, IDataContract<SECUserModel>, IBasicBusiness<SECUserModel>
	{
		private readonly string _temp_email = @"
<link rel='stylesheet' href='https://use.typekit.net/eiv7tcd.css'>
<style type='text/css'>
    h3 {{font-family: omnes-pro, Arial, 'Helvetica Neue', Helvetica, sans-serif; font-style: normal; font-weight: 600; font-size: 22px;line-height: 1.6; color: #444;}}
    p {{font-family: omnes-pro, Arial, 'Helvetica Neue', Helvetica, sans-serif; font-style: normal; font-weight: 500; font-size: 16px;line-height: 1.6; color: #444; padding-bottom: 1px;}}
    .intro {{font-family: omnes-pro, Arial, 'Helvetica Neue', Helvetica, sans-serif; font-style: normal; font-weight: 600; font-size: 16px;line-height: 1.6; color: #444;}}
    .activatelnk {{font-family: omnes-pro, Arial, 'Helvetica Neue', Helvetica, sans-serif; font-style:normal; font-weight: bold; font-size: 16px;line-height: 1.6; color: #1C0C5C;}}
    a {{font-family: omnes-pro, Arial, 'Helvetica Neue', Helvetica, sans-serif; font-style:normal; font-weight: bold; font-size: 16px;line-height: 1.6; color: #1C0C5C;}}
</style>
    <h3>{0}</h3>
    <p class='intro'>{1}</p>
    <p>{2}<br>{3}
    </p>
    <p>{4}.</p>
    <p class='activatelnk'>{5}</p>
    <p>- {6}</p>
    <hr>
    <p>{7}</p>
    <p>{8}</p>
";

		private readonly SECUserRepository dal = new SECUserRepository();

		private BasProfileInfoRepository userLDal = new BasProfileInfoRepository();

		protected override DataGridJson<SECUserModel> OnGet(MContext ctx, GetParam param)
		{
			return dal.Get(ctx, param);
		}

		[NoAuthorization]
		public OperationResult Register(MContext ctx, SECUserModel model)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			operationResult = CheckRegister(ctx, model);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			model.MEmailAddress = model.MEmailAddress.Trim().ToLower();
			model.MFirstName = model.MFirstName.Trim();
			model.MLastName = model.MLastName.Trim();
			model.MMobilePhone = model.MMobilePhone?.Trim();
			MLogger.Log("注册结果：" + (string.IsNullOrEmpty(model.MItemID) || string.IsNullOrEmpty(model.MOrgID)));
			return (string.IsNullOrEmpty(model.MItemID) || string.IsNullOrEmpty(model.MOrgID)) ? 
				SendRegisterEmail(ctx, model) : dal.JoinToOrg(ctx, model);
		}

		private OperationResult CheckRegister(MContext ctx, SECUserModel model)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			if (string.IsNullOrWhiteSpace(model.MEmailAddress))
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "MEmailError", "Email cannot be empty!");
				operationResult.Code = "10003";
				return operationResult;
			}
			if (string.IsNullOrWhiteSpace(model.MFirstName) || model.MFirstName.Trim().Length > 100)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "MFirstNameError", "First name cannot be empty or more than 100 characters");
				operationResult.Code = "10003";
				return operationResult;
			}
			if (string.IsNullOrWhiteSpace(model.MLastName) || model.MLastName.Trim().Length > 200)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "MLastNameError", "Last name cannot be empty or more than 200 characters");
				operationResult.Code = "10003";
				return operationResult;
			}
			if (!string.IsNullOrWhiteSpace(model.MMobilePhone) && model.MMobilePhone.Trim().Length > 20)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "MMobilePhoneError", "Phone number cannot be empty or more than 20 characters");
				operationResult.Code = "10003";
				return operationResult;
			}
			operationResult.Success = true;
			return operationResult;
		}

		private OperationResult SendRegisterEmail(MContext ctx, SECUserModel model)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			SECSendLinkInfoBusiness sECSendLinkInfoBusiness = new SECSendLinkInfoBusiness();
			string empty = string.Empty;
			string text = ConfigurationManager.AppSettings["ServiceEmail"];
			SECUserModel userModel = GetUserModel(ctx, model.MEmailAddress);
			if (userModel?.MIsTemp ?? true)
			{
				string guid = UUIDHelper.GetGuid();
				string text2 = DESEncrypt.Encrypt(guid);
				//empty = "<h3><i><span style='font-size: 20pt;font-family: 'Arial Unicode MS', sans-serif;'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "ActivateYourMegiAccount", "Activate your Megi account") + "</span></i></h3><br>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "Hi", "Hi") + " " + GlobalFormat.GetUserName(model.MFirstName, model.MLastName, ctx) + ":<br><br>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "YouAreAlmostThereSimply", "You’re almost there, simply click this link to activate your account and start your free trial") + ":<br><br><a href='" + ServerHelper.LoginServer + "/Password/Create/" + text2 + "' target= '_blank'>" + ServerHelper.LoginServer + "/Password/Create/" + text2 + "</a><br><br><i>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TheMegiTeam", "The Megi Team") + "</i><br><br><br><br><span style='font-size:12px'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NeedHelp", "Need help? Contact us at ") + "<a href='mailto:" + text + "'>" + text + "</a></span>";

				empty = string.Format(_temp_email,
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Register_Email_Template_Title", "Welcome to Hypercurrent"),
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Register_Email_Template_Intro", "You're almost ready to use your new Hypercurrent account. Just one more step and you'll be on your way to better &amp; easier accounting."),
					$"{COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Register_Email_Template_Greeting", "Welcome")},{GlobalFormat.GetUserName(model.MFirstName, model.MLastName, ctx)}.",
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Register_Email_Template_Message", "Thank you for signing up to Hypercurrent Accounting. Before you can start using your account, you need to verify your email address and create a secure password. Once you've done that your account will be activated. Remember that you only need one account,  even when you have access to several organisations."),
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Register_Email_Template_LinkTips", "To complete your the activation, click on the unique link below. Alternatively, you can copy then paste the link into your browser."),
					"<a href='" + ServerHelper.LoginServer + "/Password/Create/" + text2 + "' target='_blank' > " + ServerHelper.LoginServer + "/Password/Create/" + text2 + "</a>",
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Register_Email_Template_Signature", "- The Hypercurrent Team"),
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Register_Email_Template_Footer", "Having trouble activating your new account? Contact our support team at <a href=\"mailto: support@hypercu.cn\">support@hypercu.cn</a> or go to <a href=\"https://hypercu.cn/support\" target=\"_blank\">Hypercurrent Support Centre</a>."),
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Register_Email_Template_Already", "If you already have a Hypercurrent account, <a href=\"https://login.hypercu.cn\" target=\"_blank\">log in here</a>.")
					);

				var sECSendLinkInfoModel = new SECSendLinkInfoModel
				{
					MItemID = guid,
					MLinkType = 1,
					MSendDate = DateTime.Now,
					MEmail = model.MEmailAddress,
					MFirstName = model.MFirstName,
					MLastName = model.MLastName,
					MPhone = model.MMobilePhone,
					PlanCode = model.ProductCode
				};
				MLogger.Log("ProductCode:" + sECSendLinkInfoModel.PlanCode);
				sECSendLinkInfoBusiness.InsertLink(ctx, sECSendLinkInfoModel);
				operationResult.Success = true;
			}
			else
			{
				empty = "<h3><i><span style='font-size: 20pt;font-family: 'Arial Unicode MS', sans-serif;'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "YouAreAlreadyActive", "You're already active!") + "</span></i></h3>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "Hi", "Hi") + " " + GlobalFormat.GetUserName(model.MFirstName, model.MLastName, ctx) + ":<br><br>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "SomeoneHasTried", "Someone has tried to sign up to Megi with this email address, but you already have a user account.") + "<br>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "YouCanLogStraight", "You can log straight into Megi at ") + "<a href='" + ServerHelper.LoginServer + "'>" + ServerHelper.LoginServer + "</a>.<br><br>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "IfYouForgotPassword", "If you've forgotten your password you can use the \"Forgot your password?\" link at the bottom of the page.<br>") + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "IfYouNotMakeRequest", "If you didn't make this request please let us know by clicking on one of the links at the bottom of the email.") + "<br><br><i>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TheMegiTeam", "The Megi Team") + "</i><br><br><br><br><span style='font-size:12px'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NeedHelp", "Need help? Contact us at ") + "<a href='mailto:" + text + "'>" + text + "</a></span>";
				operationResult.Code = "20001";
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "registerFailDetail", "This email has been registered, please use the other email to register!");
			}

			string mEmailAddress = model.MEmailAddress;
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "ActivateYourAccount", "Activate your account");
			List<string> list = new List<string>();
			//string arg = HostingEnvironment.MapPath("~/App_Data/Reg/");
			//list.Add($"{arg}Organization setup guide.pdf");
			//list.Add($"{arg}Organization setup guide_CN.pdf");
			SendMail.SendSMTPEMail(mEmailAddress, text3, empty, list, "Hypercurrent");
			return operationResult;
		}

		[NoAuthorization]
		public OperationResult SureRegister(MContext ctx, SECAccountModel model)
		{
			return dal.SureRegister(ctx, model);
		}

		[NoAuthorization]
		public OperationResult PutNewPwd(MContext ctx, SECUserModel model)
		{
			return dal.PutNewPwd(ctx, model);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		[NoAuthorization]
		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, SECUserModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<SECUserModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public SECUserModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public SECUserModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<SECUserModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<SECUserModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		[NoAuthorization]
		public bool IsExistEmail(MContext ctx, string email)
		{
			SqlWhere filter = new SqlWhere().Equal("MEmailAddress", email).Equal("MIsDelete", 0).Equal("MIsTemp", 0);
			return dal.ExistsByFilter(ctx, filter);
		}

		public SECUserModel GetModelByEmail(MContext ctx, string email)
		{
			return dal.GetModelByEmail(ctx, email);
		}

		public SECUserModel GetUserModel(MContext ctx, string email)
		{
			return dal.GetModelByEmail(email);
		}

		public SECUserlModel GetMulitLangModel(MContext ctx, SqlWhere filter)
		{
			return userLDal.GetModelByEmail(ctx, ctx.MEmail);
		}

		public OperationResult UpdateUserMulitLangModel(MContext ctx, SECUserModel model)
		{
			OperationResult operationResult = userLDal.Update(ctx, model.SECUserLModel);
			dal.UpdateImage(ctx, model);
			if (operationResult.Success)
			{
				ctx.MFirstName = model.SECUserLModel.MFristName;
				ctx.MLastName = model.SECUserLModel.MLastName;
				ContextHelper.MContext = ctx;
			}
			return operationResult;
		}

		public OperationResult UpdateOrgListShowType(MContext ctx, int type)
		{
			OperationResult operationResult = new OperationResult();
			bool isSys = ctx.IsSys;
			operationResult.Success = true;
			try
			{
				ctx.IsSys = true;
				SECUserModel dataModel = dal.GetDataModel(ctx, ctx.MUserID, false);
				if (dataModel != null)
				{
					dataModel.MOrgListShowType = type;
					List<string> list = new List<string>();
					dal.InsertOrUpdate(ctx, dataModel, "MOrgListShowType");
					ctx.MOrgListShowType = type;
				}
			}
			catch
			{
			}
			finally
			{
				ctx.IsSys = isSys;
			}
			return operationResult;
		}

		public OperationResult ValidateCreateOrgAuth(MContext ctx, int orgType)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = true;
			string text = ConfigurationManager.AppSettings["NeedAuthOrgType"];
			if (string.IsNullOrWhiteSpace(text))
			{
				return operationResult;
			}
			if (text == Convert.ToString(orgType))
			{
				BASAccountRepository bASAccountRepository = new BASAccountRepository();
				SECUserModel userModel = bASAccountRepository.GetUserModel(ctx.MUserID);
				if (!userModel.MIsHadAddOrgAuth)
				{
					operationResult.Success = false;
					string arg = (orgType == 0) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.My, "StardardVersion", "标准版") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.My, "SmartVersion", "记账版");
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.My, "NoAuthCreateOrg", "您没有创建{0}组织的权限，请联系美记实施顾问或者您的会计师开通权限！");
					operationResult.Message = string.Format(text2, arg);
				}
			}
			return operationResult;
		}

		public OperationResult ValidateCreateOrgAuth(MContext ctx, string userId, string orgID)
		{
			BASAccountRepository bASAccountRepository = new BASAccountRepository();
			SECUserModel userModel = bASAccountRepository.GetUserModel(userId);
			BASOrganisationRepository bASOrganisationRepository = new BASOrganisationRepository();
			BASOrganisationModel dataModel = bASOrganisationRepository.GetDataModel(ctx, orgID, false);
			return ValidateCreateOrgAuth(ctx, dataModel.MVersionID);
		}
	}
}
