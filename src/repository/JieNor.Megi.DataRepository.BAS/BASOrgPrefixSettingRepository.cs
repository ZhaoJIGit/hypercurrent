using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FA;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASOrgPrefixSettingRepository : DataServiceT<BASOrgPrefixSettingModel>
	{
		public FAFixAssetsTypeRepository FixAssetsType = new FAFixAssetsTypeRepository();

		private BASOrganisationRepository Organisation = new BASOrganisationRepository();

		private BASOrgInitSettingRepository initSetting = new BASOrgInitSettingRepository();

		public OperationResult UpdateOrgPrefixSettingModel(MContext ctx, BASOrgPrefixSettingModel model)
		{
			if (model.MPrefixModule == "FixAssets")
			{
				return UpdateFixAssetsOrgPrefixSettingModel(ctx, model);
			}
			if (model.MPrefixModule == "GeneralLedger")
			{
				return UpdateVoucherOrgPrefixSettingModel(ctx, model);
			}
			return new OperationResult();
		}

		public OperationResult UpdateFixAssetsOrgPrefixSettingModel(MContext ctx, BASOrgPrefixSettingModel model)
		{
			OperationResult operationResult = new OperationResult();
			model.MPrefixModule = "FixAssets";
			if (model.ActionType == 0)
			{
				try
				{
					int mConversionYear = model.MConversionYear;
					int mConversionMonth = model.MConversionMonth;
					DateTime dateTime = new DateTime(mConversionYear, mConversionMonth, 1);
					BASOrganisationModel orgModel = Organisation.GetOrgModel(ctx);
					if (orgModel.MFABeginDate != DateTime.MinValue)
					{
						operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "FADateBegining", "固定资产已启用,请重新刷新系统！");
						operationResult.Success = false;
						return operationResult;
					}
					DateTime mGLBeginDate = ctx.MGLBeginDate;
					if (mGLBeginDate > dateTime)
					{
						operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "FASmallGLDate", "固定资产启用日期小于总账启用日期！");
						operationResult.Success = false;
						return operationResult;
					}
					string roleModelId = initSetting.GetRoleModelId(ctx);
					SqlWhere sqlWhere = new SqlWhere().In("MItemID", new string[2]
					{
						"Fixed_Assets",
						"Fixed_Assets_Reports"
					});
					List<CommandInfo> collection = SECPermissionRepository.AddAllPermissionToRole(ctx, string.Empty, roleModelId, sqlWhere, ModuleEnum.GL);
					List<string> list = new List<string>();
					List<CommandInfo> sysAdminAddGLPermissionCmds = initSetting.GetSysAdminAddGLPermissionCmds(ctx, out list, roleModelId, "Fixed_Assets");
					MultiDBCommand[] array = new MultiDBCommand[2];
					List<CommandInfo> organisationCmdInfo = GetOrganisationCmdInfo(ctx, dateTime);
					organisationCmdInfo.AddRange(collection);
					organisationCmdInfo.AddRange(sysAdminAddGLPermissionCmds);
					array[0] = new MultiDBCommand(ctx)
					{
						DBType = SysOrBas.Sys,
						CommandList = organisationCmdInfo
					};
					List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BASOrgPrefixSettingModel>(ctx, model, null, true);
					insertOrUpdateCmd.AddRange(GetOrganisationCmdInfo(ctx, dateTime));
					insertOrUpdateCmd.AddRange(FixAssetsType.GetInitAssetsTypeCommands(ctx));
					insertOrUpdateCmd.AddRange(collection);
					insertOrUpdateCmd.AddRange(sysAdminAddGLPermissionCmds);
					array[1] = new MultiDBCommand(ctx)
					{
						DBType = SysOrBas.Bas,
						CommandList = insertOrUpdateCmd
					};
					if (DbHelperMySQL.ExecuteSqlTran(ctx, array))
					{
						operationResult.Success = true;
						ctx.MFABeginDate = dateTime;
						ContextHelper.MContext = ctx;
						SECPermissionRepository.GetUserPermission(ctx, "");
						foreach (string item in list)
						{
							SECPermissionRepository.GetUserPermission(ctx, item);
						}
					}
					else
					{
						operationResult.Success = false;
						operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "BeginFAFail", "启用固定资产失败！");
					}
				}
				catch (Exception)
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "BeginFAFail", "启用固定资产失败！");
				}
			}
			else
			{
				BASOrgPrefixSettingModel orgPrefixSettingModel = GetOrgPrefixSettingModel(ctx, "FixAssets");
				orgPrefixSettingModel.MPrefixName = model.MPrefixName;
				orgPrefixSettingModel.MNumberCount = model.MNumberCount;
				orgPrefixSettingModel.MStartIndex = model.MStartIndex;
				List<CommandInfo> insertOrUpdateCmd2 = ModelInfoManager.GetInsertOrUpdateCmd<BASOrgPrefixSettingModel>(ctx, orgPrefixSettingModel, null, true);
				if (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmd2) > 0)
				{
					operationResult.Success = true;
				}
				else
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "SaveBeginFAFail", "保存固定资产信息失败！");
				}
			}
			return operationResult;
		}

		public OperationResult UpdateVoucherOrgPrefixSettingModel(MContext ctx, BASOrgPrefixSettingModel model)
		{
			OperationResult operationResult = new OperationResult();
			model.MPrefixModule = "GeneralLedger";
			BASOrgPrefixSettingModel orgPrefixSettingModel = GetOrgPrefixSettingModel(ctx, "GeneralLedger");
			orgPrefixSettingModel.MNumberCount = model.MNumberCount;
			orgPrefixSettingModel.MFillBlankChar = model.MFillBlankChar;
			orgPrefixSettingModel.MPrefixModule = "GeneralLedger";
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BASOrgPrefixSettingModel>(ctx, orgPrefixSettingModel, new List<string>
			{
				"MNumberCount",
				"MFillBlankChar"
			}, true);
			List<CommandInfo> updateVoucherNumberCmds = new GLVoucherRepository().GetUpdateVoucherNumberCmds(ctx, model);
			insertOrUpdateCmd.AddRange(updateVoucherNumberCmds);
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmd) > 0);
			if (operationResult.Success)
			{
				ctx.MVoucherNumberFilledChar = model.MFillBlankChar;
				ctx.MVoucherNumberLength = model.MNumberCount;
				ContextHelper.MContext = ctx;
				ContextHelper.UpdateMContextByKeyField("MOrgID", ctx.MOrgID, "MVoucherNumberFilledChar", model.MFillBlankChar ?? "", true);
				ContextHelper.UpdateMContextByKeyField("MOrgID", ctx.MOrgID, "MVoucherNumberLength", model.MNumberCount, true);
			}
			return operationResult;
		}

		public List<CommandInfo> GetOrganisationCmdInfo(MContext ctx, DateTime beginDate)
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<BASOrganisationModel>(ctx, new BASOrganisationModel
			{
				MItemID = ctx.MOrgID,
				MFABeginDate = beginDate
			}, new List<string>
			{
				"MFABeginDate"
			}, true);
		}

		public BASOrgPrefixSettingModel GetOrgPrefixSettingModel(MContext ctx, string prefixModule)
		{
			string sql = " SELECT * FROM t_bas_orgprefixsetting WHERE MOrgId = @MOrgID AND MPrefixModule=@MModule AND MIsDelete = 0  ";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MModule", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = prefixModule;
			BASOrgPrefixSettingModel bASOrgPrefixSettingModel = ModelInfoManager.GetDataModel<BASOrgPrefixSettingModel>(ctx, sql, array) ?? new BASOrgPrefixSettingModel
			{
				MOrgID = ctx.MOrgID
			};
			bASOrgPrefixSettingModel.MFABeginDate = ctx.MFABeginDate;
			return bASOrgPrefixSettingModel;
		}
	}
}
