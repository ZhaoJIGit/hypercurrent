using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Mail;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASAccountRepository
	{
		private SECUserRepository dalUser = new SECUserRepository();

		public OperationResult UpdateAccountData(MContext context, SECUserModel model, int type)
		{
			bool isSys = context.IsSys;
			OperationResult result = new OperationResult();
			try
			{
				context.IsSys = true;
				if (PreUpdateData(context, result, model, type))
				{
					return result;
				}
				if (type == 1)
				{
					model.MIsChangeEmail = true;
					UpdateData(context, model, 11);
					return SendSureEmail(context, model);
				}
				result = UpdateData(context, model, type);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				context.IsSys = isSys;
			}
			return result;
		}

		public OperationResult LoginForUpdateEmail(string[] token)
		{
			OperationResult result = new OperationResult();
			if (token == null || token.Length < 2)
			{
				return result;
			}
			SECUserModel sECUserModel = new SECUserModel();
			sECUserModel.MIsChangeEmail = false;
			sECUserModel.MItemID = token[0];
			sECUserModel.MEmailAddress = token[1];
			result = UpdateData(new MContext(), sECUserModel, 1);
			if (result.Success)
			{
				SECSendLinkInfoRepository.DeleteLink(token[2]);
			}
			return result;
		}

		private OperationResult UpdateData(MContext context, SECUserModel model, int type)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(GetUserUpdateSql(model, type));
			if (DbHelperMySQL.ExecuteSqlTran(context, list) > 0)
			{
				List<string> orgIdsByOrgUser = GetOrgIdsByOrgUser(model.MItemID);
				using (List<string>.Enumerator enumerator = orgIdsByOrgUser.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = context.MOrgID = enumerator.Current;
						DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
						dynamicDbHelperMySQL.ExecuteSqlTran(list);
					}
				}
				operationResult.Message = COMMultiLangRepository.GetText(context.MLCID, LangModule.My, "MyAccountSeetingsUpdateSuccess", "My Account Seetings Update Success.");
				return operationResult;
			}
			operationResult.VerificationInfor.Add(new BizVerificationInfor
			{
				Level = AlertEnum.Error,
				Message = COMMultiLangRepository.GetText(context.MLCID, LangModule.My, "MyAccountSeetingsUpdateFailed", "My Account Seetings Update Failed")
			});
			return operationResult;
		}

		private bool PreUpdateData(MContext context, OperationResult result, SECUserModel model, int type)
		{
			bool result2 = false;
			SECUserModel userModelByKey = GetUserModelByKey(model.MItemID);
			if (userModelByKey == null)
			{
				return true;
			}
			switch (type)
			{
			case 1:
				if (MD5Service.MD5Encrypt(model.MPassWord) != userModelByKey.MPassWord)
				{
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = COMMultiLangRepository.GetText(context.MLCID, LangModule.My, "PasswordIncorrect", "Your password was entered incorrectly")
					});
					result2 = true;
				}
				if (dalUser.ExistsByFilter(context, new SqlWhere().AddDeleteFilter("MIsDelete", SqlOperators.Equal, false).Equal("MEmailAddress", model.MEmailAddress.ToLower())))
				{
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = COMMultiLangRepository.GetText(context.MLCID, LangModule.My, "AddressBeingUsed", "this new address is being used by another account")
					});
					result2 = true;
				}
				break;
			case 2:
				if (MD5Service.MD5Encrypt(model.MCurPassWord) != userModelByKey.MPassWord)
				{
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = COMMultiLangRepository.GetText(context.MLCID, LangModule.My, "PasswordIncorrect", "Your password was entered incorrectly")
					});
					result2 = true;
				}
				else
				{
					model.MPassWord = MD5Service.MD5Encrypt(model.MPassWord);
				}
				break;
			case 3:
			{
				context.MAppID = model.MAppID;
				int num = 1;
				int.TryParse(model.MAppID, out num);
				context.LoginRedirect = num;
				ContextHelper.UpdateMContextByKeyField("_id", context.MUserID, "LoginRedirect", num, true);
				break;
			}
			}
			return result2;
		}

		private CommandInfo GetUserUpdateSql(SECUserModel model, int type)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_Sec_User set ");
			switch (type)
			{
			case 1:
				stringBuilder.Append(" MEmailAddress =@MEmailAddress , MIsChangeEmail = @IsChangeEmail ");
				break;
			case 11:
				stringBuilder.Append(" MIsChangeEmail =@IsChangeEmail ");
				break;
			case 2:
				stringBuilder.Append(" MPassWord =@MPassWord ");
				break;
			case 3:
				stringBuilder.Append(" MAppID =@MAppID ");
				break;
			}
			stringBuilder.Append(" where MItemID=@MItemID and MIsDelete = 0 and MIsActive = 1 ");
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEmailAddress", MySqlDbType.VarChar, 50),
				new MySqlParameter("@MPassWord", MySqlDbType.VarChar, 100),
				new MySqlParameter("@MAppID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@IsChangeEmail", MySqlDbType.Bit, 1)
			};
			array[0].Value = model.MItemID;
			array[1].Value = model.MEmailAddress;
			array[2].Value = model.MPassWord;
			array[3].Value = model.MAppID;
			array[4].Value = model.MIsChangeEmail;
			return new CommandInfo(stringBuilder.ToString(), array);
		}

		public SECUserModel GetUserModelByKey(string MItemID)
		{
			SECUserModel result = new SECUserModel();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select MItemID,MEmailAddress,MPassWord,MAppID,MLastLoginAppID ,MIsChangeEmail");
			stringBuilder.Append("  from T_Sec_User ");
			stringBuilder.Append(" where MItemID=@MItemID and MIsDelete = 0 and MIsActive = 1 ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = MItemID;
			DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<SECUserModel> list = ModelInfoManager.DataTableToList<SECUserModel>(dataSet.Tables[0]);
				result = list[0];
			}
			return result;
		}

		public SECUserModel GetUserModel(string id)
		{
			SECUserModel result = new SECUserModel();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select MItemID,MEmailAddress,MPassWord,MAppID,MLastLoginAppID ,MIsChangeEmail , MIsHadAddOrgAuth");
			stringBuilder.Append("  from T_Sec_User ");
			stringBuilder.Append(" where MItemID=@MItemID and MIsDelete = 0 and MIsActive = 1 and MIsTemp=0 ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = id;
			DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<SECUserModel> list = ModelInfoManager.DataTableToList<SECUserModel>(dataSet.Tables[0]);
				result = list[0];
			}
			return result;
		}

		private List<string> GetOrgIdsByOrgUser(string userid)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select distinct t3.MOrgID ");
			stringBuilder.AppendLine(" from T_Sec_User t1 ");
			stringBuilder.AppendLine(" join T_Sec_OrgUser t2 on t1.MItemID=t2.MUserID and t2.MIsDelete=0 and t2.MIsActive = 1 ");
			stringBuilder.AppendLine(" join T_SYS_OrgApp t3 on t2.MOrgID=t3.MOrgID and t3.MIsDelete=0 and t3.MIsActive = 1  ");
			stringBuilder.AppendLine(" where t1.MIsDelete=0 and t1.MIsActive = 1 and t1.MItemID=@UserID ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@UserID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = userid;
			DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				DataTable dataTable = dataSet.Tables[0];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					list.Add(dataTable.Rows[i]["MOrgID"].ToString());
				}
			}
			return list;
		}

		private OperationResult SendSureEmail(MContext ctx, SECUserModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<MailHelper> list = new List<MailHelper>();
			SECUserModel dataEditModel = ModelInfoManager.GetDataEditModel<SECUserModel>(ctx, model.MItemID, false, true);
			string guid = UUIDHelper.GetGuid();
			string token = DESEncrypt.Encrypt(model.MItemID + "|#|$" + model.MEmailAddress + "|#|$" + guid);
			list.Add(GetSendNewMailContent(ctx, GetUserLastName(ctx, dataEditModel), model.MEmailAddress, token));
			list.Add(GetSendOldMailContent(ctx, GetUserLastName(ctx, dataEditModel), model.MEmailAddress, dataEditModel.MEmailAddress));
			SendMail.SendSMTPEMail(list);
			SECSendLinkInfoModel sECSendLinkInfoModel = new SECSendLinkInfoModel();
			sECSendLinkInfoModel.MItemID = guid;
			sECSendLinkInfoModel.MSendDate = DateTime.Now;
			sECSendLinkInfoModel.MInvitationEmail = dataEditModel.MEmailAddress;
			sECSendLinkInfoModel.MLinkType = 3;
			sECSendLinkInfoModel.MEmail = model.MEmailAddress;
			sECSendLinkInfoModel.MFirstName = dataEditModel.MFirstName;
			sECSendLinkInfoModel.MLastName = dataEditModel.MLastName;
			sECSendLinkInfoModel.MPhone = "";
			SECSendLinkInfoRepository.InsertLink(sECSendLinkInfoModel.MItemID, model.MEmailAddress, DateTime.Now, model.MMobilePhone, sECSendLinkInfoModel.MFirstName, sECSendLinkInfoModel.MLastName, sECSendLinkInfoModel.MLinkType, sECSendLinkInfoModel.MInvitationEmail, sECSendLinkInfoModel.MInvitationOrgID);
			return new OperationResult
			{
				Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.My, "SureEmailSubmittedToServer", "the sure email have been submitted to the server to send.")
			};
		}

		private MailHelper GetSendNewMailContent(MContext ctx, string lastName, string newEmail, string token)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.My, "ConfirmNewAddress", "Confirm your new email address");
			string loginServer = ServerHelper.LoginServer;
			string arg = "<a href='" + loginServer + "?cgtkn=" + token + " ' target= '_blank'>" + loginServer + "?cgtkn=" + token + "</a><br />  ";
			string strBody = string.Format(FormatNewMailBody(), lastName, newEmail, arg);
			return SendMail.GetSendMailHelper(newEmail, text, strBody, "Megi", "");
		}

		private MailHelper GetSendOldMailContent(MContext ctx, string lastName, string newEmail, string oldEmail)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.My, "AddressChanged", "Your login address has been changed");
			string strBody = string.Format(FormatOldMailBody(), lastName, oldEmail, newEmail);
			return SendMail.GetSendMailHelper(oldEmail, text, strBody, "Megi", "");
		}

		private string FormatNewMailBody()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Hi {0}, <br/><br/>");
			stringBuilder.AppendLine("You're almost done changing your email address to {1}.<br/>");
			stringBuilder.AppendLine("All you need to do now is confirm by clicking this link:<br/><br/>");
			stringBuilder.AppendLine("{2}<br/><br/>");
			stringBuilder.AppendLine("If you didn't ask for this change, let us know by replying to this email.<br/><br/>");
			stringBuilder.AppendLine("Regards,<br/>");
			stringBuilder.AppendLine("The Megi Team ");
			return stringBuilder.ToString();
		}

		private string FormatOldMailBody()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Hi {0}, <br/><br/>");
			stringBuilder.AppendLine("A request has been made to change your Xero login from {1} to {2}.<br/><br/>");
			stringBuilder.AppendLine("If you didn't request this change, let us know by replying to this email.<br/><br/>");
			stringBuilder.AppendLine("Regards,<br/>");
			stringBuilder.AppendLine("The Megi Team ");
			return stringBuilder.ToString();
		}

		private string GetUserLastName(MContext ctx, SECUserModel model)
		{
			MultiLanguageFieldList multiLanguageFieldList = model.MultiLanguage.Find((MultiLanguageFieldList p) => p.MFieldName.Equals("MLastName"));
			if (multiLanguageFieldList == null)
			{
				return string.Empty;
			}
			MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.Find((MultiLanguageField p) => p.MLocaleID.Equals(ctx.MLCID, StringComparison.OrdinalIgnoreCase));
			if (multiLanguageField == null || string.IsNullOrWhiteSpace(multiLanguageField.MValue))
			{
				multiLanguageField = multiLanguageFieldList.MMultiLanguageField.Find((MultiLanguageField p) => p.MLocaleID.Equals(LangCodeEnum.EN_US, StringComparison.OrdinalIgnoreCase));
				if (multiLanguageField == null || string.IsNullOrWhiteSpace(multiLanguageField.MValue))
				{
					return string.Empty;
				}
				return multiLanguageField.MValue;
			}
			return multiLanguageField.MValue;
		}
	}
}
