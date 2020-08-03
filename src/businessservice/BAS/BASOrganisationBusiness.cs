using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.PA;
using JieNor.Megi.BusinessService.PT;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.BusinessService.SYS;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASOrganisationBusiness : APIBusinessBase<BASOrganisationModel>, IBASOrganisationBusiness, IDataContract<BASOrganisationModel>
	{
		private List<REGFinancialModel> financialList;

		private GLSettlementModel currentSettlement;

		private BASOrganisationRepository dal = new BASOrganisationRepository();

		private REGGlobalizationBusiness _globalBiz = new REGGlobalizationBusiness();

		private FPSettingRepository _fpSettingRepository = new FPSettingRepository();

		protected override void OnGetBefore(MContext ctx, GetParam param)
		{
			param.ModifiedSince = DateTime.MinValue;
			param.IgnoreModifiedSince = true;
		}

		protected override DataGridJson<BASOrganisationModel> OnGet(MContext ctx, GetParam param)
		{
			DataGridJson<BASOrganisationModel> dataGridJson = null;
			bool isSys = ctx.IsSys;
			ctx.IsSys = true;
			if (param.Where == "MI")
			{
				dataGridJson = dal.GetMigrationOrgList(ctx, param);
			}
			else
			{
				ctx.IsSys = false;
				financialList = ModelInfoManager.GetDataModelList<REGFinancialModel>(ctx, new SqlWhere().In("MOrgID", new string[1]
				{
					ctx.MOrgID
				}), false, false);
				GLSettlementRepository gLSettlementRepository = new GLSettlementRepository();
				currentSettlement = gLSettlementRepository.GetCurrentSettlement(ctx);
				ctx.IsSys = true;
				dataGridJson = dal.Get(ctx, param);
			}
			ctx.IsSys = isSys;
			return dataGridJson;
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, BASOrganisationModel model)
		{
			SetOrgSubscriptionStatus(ctx, model);
			if (!ctx.MInitBalanceOver)
			{
				model.MCurrentAccountingPeriod = string.Empty;
			}
			else if (currentSettlement != null)
			{
				DateTime dateTime = new DateTime(currentSettlement.MYear, currentSettlement.MPeriod, 1).AddMonths(1);
				model.MCurrentAccountingPeriod = dateTime.Year + "-" + dateTime.Month.ToString().PadLeft(2, '0');
			}
			else if (ctx.MInitBalanceOver)
			{
				model.MCurrentAccountingPeriod = model.MConversionPeriod;
			}
			REGFinancialModel rEGFinancialModel = financialList.FirstOrDefault((REGFinancialModel f) => f.MOrgID == model.MItemID);
			if (rEGFinancialModel != null)
			{
				model.MCurrencyID = rEGFinancialModel.MCurrencyID;
				model.MTaxPayer = rEGFinancialModel.MTaxPayer;
				model.MTaxNo = rEGFinancialModel.MTaxNo;
			}
		}

		private void SetOrgSubscriptionStatus(MContext ctx, BASOrganisationModel orgModel)
		{
			if (orgModel.MExpiredDate < ctx.DateNow)
			{
				if (orgModel.MIsPaid)
				{
					orgModel.MSubscriptionStatus = 5;
				}
				else
				{
					orgModel.MSubscriptionStatus = 1;
				}
			}
			else if (orgModel.MIsPaid)
			{
				orgModel.MSubscriptionStatus = 3;
			}
			else if (orgModel.MIsBeta)
			{
				orgModel.MSubscriptionStatus = 4;
			}
			else
			{
				orgModel.MSubscriptionStatus = 2;
			}
		}

		public OperationResult CreateDemoCompany(MContext ctx)
		{
			return BASOrganisationRepository.CreateDemoCompany(ctx);
		}

		public BASOrganisationModel GetDemoOrg(MContext ctx)
		{
			return BASOrganisationRepository.GetDemoOrg(ctx);
		}

		public OperationResult Register(MContext ctx, BASOrganisationModel model)
		{
			try
			{
				ctx.IsSys = true;
				dal.InsertOrUpdate(ctx, model, null);
				AddDefaultAddress(ctx, model);
				AddDefaultContactDetail(ctx, model);
				return new OperationResult
				{
					Success = true,
					ObjectID = model.MItemID
				};
			}
			catch (Exception ex)
			{
				return new OperationResult
				{
					Success = false,
					Message = ex.Message
				};
			}
		}

		public OperationResult UpdateOrgInfo(MContext ctx, BASOrgInfoModel info)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			BASOrganisationModel bASOrganisationModel = new BASOrganisationModel();
			bASOrganisationModel.MItemID = info.MOrgID;
			bASOrganisationModel.MName = info.MDisplayName;
			bASOrganisationModel.MLegalTradingName = info.MLagalTrading;
			bASOrganisationModel.MOrgTypeID = info.MOrgTypeID;
			bASOrganisationModel.MOrgBusiness = info.MOrgDesc;
			bASOrganisationModel.MCountryID = info.MCountryID;
			bASOrganisationModel.MStateID = info.MStateID;
			bASOrganisationModel.MCityID = info.MCityID;
			bASOrganisationModel.MStreet = info.MStreet;
			bASOrganisationModel.MPostalNo = info.MPostalNo;
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BASOrganisationModel>(ctx, bASOrganisationModel, "MName,MLegalTradingName,MOrgTypeID,MOrgBusiness,MCountryID,MStateID,MCityID,MStreet,MPostalNo,MModifyDate".Split(',').ToList(), true));
			operationResult.Success = DbHelperMySQL.ExecuteSqlTran(ctx, new MultiDBCommand[2]
			{
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Sys,
					CommandList = list
				},
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Bas,
					CommandList = list
				}
			});
			ctx.MOrgName = info.MDisplayName;
			ctx.MLastLoginOrgName = info.MDisplayName;
			ctx.MLegalTradingName = info.MLagalTrading;
			ContextHelper.MContext = ctx;
			operationResult.ObjectID = info.MOrgID;
			return operationResult;
		}

		public BASOrgInfoModel GetOrgInfo(MContext ctx, ParamBase param)
		{
			ctx.IsSys = true;
			BASOrganisationModel dataModel = dal.GetDataModel(ctx, param.KeyIDs, false);
			if (dataModel == null)
			{
				return null;
			}
			BASOrgInfoModel bASOrgInfoModel = new BASOrgInfoModel();
			bASOrgInfoModel.MDisplayName = dataModel.MName;
			bASOrgInfoModel.MOrgID = dataModel.MItemID;
			bASOrgInfoModel.MLagalTrading = dataModel.MLegalTradingName;
			bASOrgInfoModel.MOrgDesc = dataModel.MOrgBusiness;
			bASOrgInfoModel.MOrgTypeID = dataModel.MOrgTypeID;
			bASOrgInfoModel.MCountryID = dataModel.MCountryID;
			bASOrgInfoModel.MStateID = dataModel.MStateID;
			bASOrgInfoModel.MCityID = dataModel.MCityID;
			bASOrgInfoModel.MStreet = dataModel.MStreet;
			bASOrgInfoModel.MPostalNo = dataModel.MPostalNo;
			SetAddressDetail(ctx, ref bASOrgInfoModel, param);
			SetContactDetail(ctx, ref bASOrgInfoModel, param);
			return bASOrgInfoModel;
		}

		public OperationResult UpdateRegSchedule(MContext ctx, BASOrganisationModel model)
		{
			ctx.IsSys = true;
			return BASOrganisationRepository.UpdateRegSchedule(ctx, model);
		}

		public OperationResult UpdateRegProgress(MContext ctx, BASOrgScheduleTypeEnum type)
		{
			ctx.IsSys = true;
			OperationResult operationResult = null;
			try
			{
				operationResult = BASOrganisationRepository.UpdateRegProgress(type, ctx);
				PTBizBusiness pTBizBusiness = new PTBizBusiness();
				PTVoucherBusiness pTVoucherBusiness = new PTVoucherBusiness();
				if (operationResult.Success)
				{
					ctx.MRegProgress = (int)type;
					switch (type)
					{
					case BASOrgScheduleTypeEnum.OrgSetting:
						ctx.MExistsOrg = true;
						break;
					case BASOrgScheduleTypeEnum.GLSuccess:
					{
						if (ctx.MOrgVersionID == 1)
						{
							ctx.IsSys = false;
							pTBizBusiness.InsertStandardSetting(ctx);
							pTVoucherBusiness.InsertDefaultData(ctx);
						}
						else
						{
							ctx.IsSys = false;
							PAPayItemBussiness pAPayItemBussiness = new PAPayItemBussiness();
							pAPayItemBussiness.InsertDefaultDataIfNotExist(ctx);
							PTSalaryListBusiness pTSalaryListBusiness = new PTSalaryListBusiness();
							pTBizBusiness.InsertStandardSetting(ctx);
							pTSalaryListBusiness.InsertStandardSetting(ctx);
							pTVoucherBusiness.InsertDefaultData(ctx);
							if (!ctx.MEnabledModules.Contains(ModuleEnum.Sales))
							{
								ctx.MEnabledModules.Add(ModuleEnum.Sales);
							}
							ctx.IsSys = true;
							BASOrganisationModel dataEditModel = ModelInfoManager.GetDataEditModel<BASOrganisationModel>(ctx, ctx.MOrgID, false, true);
							if (dataEditModel.MOriRegProgress > 5 && dataEditModel.MOriRegProgress < 15)
							{
								new BASOrgInitSettingBusiness().GLSetupSuccess(ctx);
							}
							ctx.IsSys = false;
						}
						List<CommandInfo> list = new List<CommandInfo>();
						List<CommandInfo> list2 = new List<CommandInfo>();
						BASOrgModuleModel bASOrgModuleModel = new BASOrgModuleModel();
						bASOrgModuleModel.MOrgID = ctx.MOrgID;
						int num = 2;
						string moduleId = bASOrgModuleModel.MModuleID = num.ToString();
						BASOrgModuleModel bASOrgModuleModel2 = bASOrgModuleModel;
						DateTime dateTime = ctx.DateNow;
						dateTime = dateTime.Date;
						dateTime = dateTime.AddDays(30.0);
						bASOrgModuleModel2.MExpiredDate = dateTime.AddSeconds(-1.0);
						List<BASOrgModuleModel> orgModuleList = GetOrgModuleList(ctx, moduleId);
						if (orgModuleList.Any())
						{
							List<CommandInfo> deleteCmd = ModelInfoManager.GetDeleteCmd<BASOrgModuleModel>(ctx, orgModuleList[0].MItemID);
							list.AddRange(deleteCmd);
							list2.AddRange(deleteCmd);
						}
						List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BASOrgModuleModel>(ctx, bASOrgModuleModel, null, true);
						list.AddRange(insertOrUpdateCmd);
						list2.AddRange(insertOrUpdateCmd);
						list2.AddRange(_fpSettingRepository.GetInitImportCofigCommandInfos(ctx));
						MultiDBCommand[] cmdArray = new MultiDBCommand[2]
						{
							new MultiDBCommand(ctx)
							{
								CommandList = list,
								DBType = SysOrBas.Sys
							},
							new MultiDBCommand(ctx)
							{
								CommandList = list2,
								DBType = SysOrBas.Bas
							}
						};
						DbHelperMySQL.ExecuteSqlTran(ctx, cmdArray);
						if (!ctx.MEnabledModules.Contains(ModuleEnum.GL))
						{
							ctx.MEnabledModules.Add(ModuleEnum.GL);
						}
						break;
					}
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = false;
				ContextHelper.MContext = ctx;
			}
			return operationResult;
		}

		public List<BASOrgModuleModel> GetOrgModuleList(MContext ctx, string moduleId = null)
		{
			SqlWhere sqlWhere = new SqlWhere().Equal("MOrgID", ctx.MOrgID);
			if (!string.IsNullOrWhiteSpace(moduleId))
			{
				sqlWhere.Equal("MModuleID", moduleId);
			}
			return ModelInfoManager.GetDataModelList<BASOrgModuleModel>(ctx, sqlWhere, false, false);
		}

		public OperationResult Update(MContext ctx, BASOrganisationModel model)
		{
			return BASOrganisationRepository.Update(ctx, model);
		}

		public BASOrganisationModel GetModel(MContext ctx)
		{
			ctx.IsSys = true;
			return dal.GetDataModel(ctx, ctx.MOrgID, false);
		}

		public bool IsOrgExist(MContext ctx, string displayName, string excludeId)
		{
			bool isSys = ctx.IsSys;
			//bool flag = false;
			try
			{
				ctx.IsSys = true;
				return BASOrganisationRepository.GetOrgListByName(ctx, displayName, excludeId).Any();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = isSys;
			}
		}

		public BASOrganisationModel GetOrgBasicInfo(MContext ctx)
		{
			BASOrganisationModel orgBasicInfo = BASOrganisationRepository.GetOrgBasicInfo(ctx);
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.IsNullOrWhiteSpace(orgBasicInfo.MStateName) ? orgBasicInfo.MStateID : orgBasicInfo.MStateName;
			string text2 = '┇'.ToString();
			if (ctx.MLCID == "0x0009")
			{
				if (!string.IsNullOrWhiteSpace(orgBasicInfo.MStreet))
				{
					stringBuilder.Append(orgBasicInfo.MStreet);
				}
				if (!string.IsNullOrWhiteSpace(orgBasicInfo.MCityID))
				{
					stringBuilder.Append(text2);
					stringBuilder.Append(orgBasicInfo.MCityID);
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(text);
				}
				if (!string.IsNullOrWhiteSpace(orgBasicInfo.MPostalNo))
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(orgBasicInfo.MPostalNo);
				}
				stringBuilder.Append(text2);
				stringBuilder.Append(orgBasicInfo.MCountryName);
			}
			else
			{
				stringBuilder.AppendFormat("{0}{1}{2}{3}", orgBasicInfo.MCountryName, text, orgBasicInfo.MCityID, orgBasicInfo.MStreet);
				if (!string.IsNullOrWhiteSpace(orgBasicInfo.MPostalNo))
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(orgBasicInfo.MPostalNo);
				}
			}
			orgBasicInfo.MRegAddress = stringBuilder.ToString().Trim(',').Trim('┇')
				.Replace(text2, "\n")
				.Trim();
			return orgBasicInfo;
		}

		private void SetAddressDetail(MContext ctx, ref BASOrgInfoModel info, ParamBase param)
		{
			ctx.IsSys = true;
			string mOrgID = info.MOrgID;
			BASAddressTypeEnum bASAddressTypeEnum = BASAddressTypeEnum.Postal;
			BASOrgAddressModel model = BASOrgAddressRepository.GetModel(ctx, mOrgID, bASAddressTypeEnum.ToString(), param);
			if (model != null)
			{
				info.MPostalID = model.MItemID;
				info.MPostalStreet = model.MStreet;
				info.MPostalTownCity = model.MTownCity;
				info.MPostalStateRegion = model.MStateRegion;
				info.MPostalZipcode = model.MZipcode;
				info.MPostalCountry = model.MCountry;
				info.MPostalAttention = model.MAttention;
			}
			string mOrgID2 = info.MOrgID;
			bASAddressTypeEnum = BASAddressTypeEnum.Physical;
			BASOrgAddressModel model2 = BASOrgAddressRepository.GetModel(ctx, mOrgID2, bASAddressTypeEnum.ToString(), param);
			if (model != null)
			{
				info.MPhysicalID = model2.MItemID;
				info.MPhysicalStreet = model2.MStreet;
				info.MPhysicalTownCity = model2.MTownCity;
				info.MPhysicalStateRegion = model2.MStateRegion;
				info.MPhysicalZipcode = model2.MZipcode;
				info.MPhysicalCountry = model2.MCountry;
				info.MPhysicalAttention = model2.MAttention;
			}
		}

		private void SetContactDetail(MContext ctx, ref BASOrgInfoModel info, ParamBase param)
		{
			ctx.IsSys = true;
			BASOrgContactModel modelByOrgID = BASOrgContactRepository.GetModelByOrgID(info.MOrgID);
			if (modelByOrgID != null)
			{
				info.MContactID = modelByOrgID.MItemID;
				info.MFacebook = modelByOrgID.MFacebook;
				info.MLinkedIn = modelByOrgID.MLinkedIn;
				info.MGooglePlus = modelByOrgID.MGooglePlus;
				info.MTwitter = modelByOrgID.MTwitter;
				info.MWebsite = modelByOrgID.MWebsite;
				info.MTBShop = modelByOrgID.MTBShop;
				info.MWeChat = modelByOrgID.MWeChat;
				info.MWeibo = modelByOrgID.MWeibo;
				info.MQQ = modelByOrgID.MQQ;
			}
		}

		private void AddDefaultAddress(MContext ctx, BASOrganisationModel model)
		{
			ctx.IsSys = true;
			BASOrgAddressRepository bASOrgAddressRepository = new BASOrgAddressRepository();
			BASOrgAddressModel bASOrgAddressModel = new BASOrgAddressModel();
			BASOrgAddressModel bASOrgAddressModel2 = bASOrgAddressModel;
			BASAddressTypeEnum bASAddressTypeEnum = BASAddressTypeEnum.Postal;
			bASOrgAddressModel2.MAddressType = bASAddressTypeEnum.ToString();
			bASOrgAddressModel.MOrgID = model.MItemID;
			bASOrgAddressRepository.InsertOrUpdate(ctx, bASOrgAddressModel, null);
			BASOrgAddressModel bASOrgAddressModel3 = new BASOrgAddressModel();
			BASOrgAddressModel bASOrgAddressModel4 = bASOrgAddressModel3;
			bASAddressTypeEnum = BASAddressTypeEnum.Physical;
			bASOrgAddressModel4.MAddressType = bASAddressTypeEnum.ToString();
			bASOrgAddressModel3.MOrgID = model.MItemID;
			bASOrgAddressRepository.InsertOrUpdate(ctx, bASOrgAddressModel3, null);
		}

		private void AddDefaultContactDetail(MContext ctx, BASOrganisationModel model)
		{
			BASOrgContactRepository bASOrgContactRepository = new BASOrgContactRepository();
			BASOrgContactModel bASOrgContactModel = new BASOrgContactModel();
			bASOrgContactModel.MOrgID = model.MItemID;
			bASOrgContactRepository.InsertOrUpdate(ctx, bASOrgContactModel, null);
		}

		private List<CommandInfo> UpdateAddressDetail(MContext ctx, BASOrgInfoModel info)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BASOrgAddressModel bASOrgAddressModel = new BASOrgAddressModel();
			bASOrgAddressModel.MItemID = info.MPostalID;
			BASOrgAddressModel bASOrgAddressModel2 = bASOrgAddressModel;
			BASAddressTypeEnum bASAddressTypeEnum = BASAddressTypeEnum.Postal;
			bASOrgAddressModel2.MAddressType = bASAddressTypeEnum.ToString();
			bASOrgAddressModel.MOrgID = info.MOrgID;
			bASOrgAddressModel.MStreet = info.MPostalStreet;
			bASOrgAddressModel.MTownCity = info.MPostalTownCity;
			bASOrgAddressModel.MStateRegion = info.MPostalStateRegion;
			bASOrgAddressModel.MZipcode = info.MPostalZipcode;
			bASOrgAddressModel.MCountry = info.MPostalCountry;
			bASOrgAddressModel.MAttention = info.MPostalAttention;
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BASOrgAddressModel>(ctx, bASOrgAddressModel, null, true));
			BASOrgAddressModel bASOrgAddressModel3 = new BASOrgAddressModel();
			bASOrgAddressModel3.MItemID = info.MPhysicalID;
			BASOrgAddressModel bASOrgAddressModel4 = bASOrgAddressModel3;
			bASAddressTypeEnum = BASAddressTypeEnum.Physical;
			bASOrgAddressModel4.MAddressType = bASAddressTypeEnum.ToString();
			bASOrgAddressModel3.MOrgID = info.MOrgID;
			bASOrgAddressModel3.MStreet = info.MPhysicalStreet;
			bASOrgAddressModel3.MTownCity = info.MPhysicalTownCity;
			bASOrgAddressModel3.MStateRegion = info.MPhysicalStateRegion;
			bASOrgAddressModel3.MZipcode = info.MPhysicalZipcode;
			bASOrgAddressModel3.MCountry = info.MPhysicalCountry;
			bASOrgAddressModel3.MAttention = info.MPostalAttention;
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BASOrgAddressModel>(ctx, bASOrgAddressModel3, null, true));
			return list;
		}

		private List<CommandInfo> UpdateContactDetail(MContext ctx, BASOrgInfoModel info)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BASOrgContactModel bASOrgContactModel = new BASOrgContactModel();
			bASOrgContactModel.MItemID = info.MContactID;
			bASOrgContactModel.MOrgID = info.MOrgID;
			bASOrgContactModel.MFacebook = info.MFacebook;
			bASOrgContactModel.MTwitter = info.MTwitter;
			bASOrgContactModel.MLinkedIn = info.MLinkedIn;
			bASOrgContactModel.MGooglePlus = info.MGooglePlus;
			bASOrgContactModel.MWebsite = info.MWebsite;
			bASOrgContactModel.MTBShop = info.MTBShop;
			bASOrgContactModel.MWeChat = info.MWeChat;
			bASOrgContactModel.MWeibo = info.MWeibo;
			bASOrgContactModel.MQQ = info.MQQ;
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BASOrgContactModel>(ctx, bASOrgContactModel, null, true));
			return list;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			ctx.IsSys = true;
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BASOrganisationModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BASOrganisationModel> modelData, string fields = null)
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

		public BASOrganisationModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		[NoAuthorization]
		public BASOrganisationModel GetModelById(MContext ctx, string pkID)
		{
			return dal.GetDataModel(ctx, pkID, false);
		}

		public BASOrganisationModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BASOrganisationModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BASOrganisationModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public void ChangeOrgById(MContext ctx, string orgId)
		{
			string mLCID = ctx.MLCID;
			MContext mContext = ContextHelper.ClearContextOrgInfo(ctx, false);
			mContext.MOrgID = orgId;
			mContext.MAppID = "1";
			BASOrganisationBusiness bASOrganisationBusiness = new BASOrganisationBusiness();
			BASOrganisationModel dataModel = bASOrganisationBusiness.GetDataModel(new MContext
			{
				IsSys = true
			}, orgId, false);
			if (dataModel == null)
			{
				mContext.MOrgID = string.Empty;
				mContext.MAppID = string.Empty;
				ContextHelper.MContext = mContext;
			}
			else
			{
				mContext.MOrgVersionID = dataModel.MVersionID;
				mContext.MDefaultLocaleID = dataModel.MDefaulLocaleID;
				mContext.MMaster = dataModel.MMasterID;
				mContext.MBeginDate = ((dataModel.MConversionDate.Year <= 1900) ? new DateTime(2000, 1, 1) : dataModel.MConversionDate);
				mContext.MRegProgress = dataModel.MRegProgress;
				mContext.MExpireDate = dataModel.MExpiredDate;
				mContext.MFABeginDate = dataModel.MFABeginDate;
				mContext.MIsBeta = dataModel.MIsBeta;
				BASOrgInitSettingModel dataModelByFilter = new BASOrgInitSettingBusiness().GetDataModelByFilter(mContext, new SqlWhere().Equal("MOrgID", orgId));
				if (dataModelByFilter != null)
				{
					mContext.MGLBeginDate = dataModelByFilter.MConversionDate;
					mContext.MAccountTableID = dataModelByFilter.MAccountingStandard;
				}
				List<BASOrgModuleModel> orgModuleList = bASOrganisationBusiness.GetOrgModuleList(ctx, null);
				mContext.MEnabledModules = new List<ModuleEnum>();
				Array values = Enum.GetValues(typeof(ModuleEnum));
				for (int i = 0; i < values.Length; i++)
				{
					int moduleId = (int)values.GetValue(i);
					if (orgModuleList.Any((BASOrgModuleModel f) => Convert.ToInt32(f.MModuleID) == moduleId))
					{
						mContext.MEnabledModules.Add((ModuleEnum)moduleId);
					}
				}
				mContext.MActiveLocaleIDS = new BASLangBusiness().GetOrgLangIDList(mContext);
				SYSOrgAppBusiness sYSOrgAppBusiness = new SYSOrgAppBusiness();
				SYSOrgAppModel dataModelByFilter2 = sYSOrgAppBusiness.GetDataModelByFilter(mContext, new SqlWhere().Equal("MOrgID", orgId));
				BASOrganisationModel model = GetModel(ctx);
				if (model != null)
				{
					mContext.MExistsOrg = true;
					dataModelByFilter2.MOrgName = model.MName;
					mContext.MLegalTradingName = model.MLegalTradingName;
					mContext.MInitBalanceOver = model.MInitBalanceOver;
				}
				mContext.IsSys = false;
				mContext.MUsedStatusID = dataModelByFilter2.MUsedStatusID;
				mContext.MOrgName = dataModelByFilter2.MOrgName;
				mContext.MLastLoginOrgId = mContext.MOrgID;
				mContext.MLastLoginOrgName = mContext.MOrgName;
				REGGlobalizationModel orgGlobalizationDetail = _globalBiz.GetOrgGlobalizationDetail(mContext, orgId);
				_globalBiz.SetGlobalizationContext(ctx, orgGlobalizationDetail);
				BASCurrencyViewModel baseCurrency = new REGCurrencyBusiness().GetBaseCurrency(mContext);
				if (baseCurrency != null)
				{
					mContext.MBasCurrencyID = baseCurrency.MCurrencyID;
				}
				REGFinancialModel dataModelByFilter3 = new REGFinancialBusiness().GetDataModelByFilter(mContext, new SqlWhere().Equal("MOrgID", ctx.MOrgID));
				if (dataModelByFilter3 != null)
				{
					mContext.MOrgTaxType = Convert.ToInt32(dataModelByFilter3.MTaxPayer);
					mContext.MTaxCode = dataModelByFilter3.MTaxNo;
				}
				SECOrgUserModel orgUserModel = SECOrgUserRepository.GetOrgUserModel(ctx);
				if (orgUserModel != null)
				{
					if (!string.IsNullOrEmpty(orgUserModel.MPosition))
					{
						mContext.MPosition = orgUserModel.MPosition.Split(',').ToList();
					}
					mContext.MRole = orgUserModel.MRole;
					mContext.IsSelfData = orgUserModel.IsSelfData;
				}
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MOrgID", orgId);
				BASOrgPrefixSettingModel orgPrefixSettingModel = new BASOrgPrefixSettingRepository().GetOrgPrefixSettingModel(ctx, "GeneralLedger");
				if (orgPrefixSettingModel != null)
				{
					string mVoucherNumberFilledChar = (orgPrefixSettingModel.MNumberCount < 3) ? "0" : orgPrefixSettingModel.MFillBlankChar;
					int mVoucherNumberLength = (orgPrefixSettingModel.MNumberCount < 3) ? 3 : orgPrefixSettingModel.MNumberCount;
					mContext.MVoucherNumberFilledChar = mVoucherNumberFilledChar;
					mContext.MVoucherNumberLength = mVoucherNumberLength;
					ctx.MVoucherNumberFilledChar = mVoucherNumberFilledChar;
					ctx.MVoucherNumberLength = mVoucherNumberLength;
				}
				ContextHelper.MContext = mContext;
				((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).Clear();
			}
		}

		public BASOrganisationModel GetOrganisationInSysDB(MContext ctx, string orgID)
		{
			if (string.IsNullOrWhiteSpace(orgID))
			{
				return null;
			}
			return dal.GetOrgModelInSysDB(ctx, orgID);
		}

		public int UpdateDemoData()
		{
			BASOrganisationRepository bASOrganisationRepository = new BASOrganisationRepository();
			return bASOrganisationRepository.UpdateDemoData();
		}
	}
}
