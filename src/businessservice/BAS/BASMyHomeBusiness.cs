using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.DataRepository.SYS;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASMyHomeBusiness : IBASMyHomeBusiness
	{
		private BASMyHomeRepository dal = new BASMyHomeRepository();

		public MContext GetMContextByOrgID(MContext ctx)
		{
			BASOrganisationRepository bASOrganisationRepository = new BASOrganisationRepository();
			BASOrganisationModel dataModel = bASOrganisationRepository.GetDataModel(ctx, ctx.MOrgID, false);
			ctx.MDefaultLocaleID = dataModel.MDefaulLocaleID;
			ctx.MMaster = dataModel.MMasterID;
			BASLangBusiness bASLangBusiness = new BASLangBusiness();
			ctx.MActiveLocaleIDS = bASLangBusiness.GetOrgLangIDList(ctx);
			SYSOrgAppRepository sYSOrgAppRepository = new SYSOrgAppRepository();
			SYSOrgAppModel dataModelByFilter = sYSOrgAppRepository.GetDataModelByFilter(ctx, new SqlWhere().Equal(" MOrgID ", ctx.MOrgID));
			ctx.MUsedStatusID = dataModelByFilter.MUsedStatusID;
			ctx.MExpireDate = dataModelByFilter.MExpireDate;
			ctx.MOrgName = dataModelByFilter.MOrgName;
			REGGlobalizationRepository rEGGlobalizationRepository = new REGGlobalizationRepository();
			REGGlobalizationModel orgGlobalizationDetail = rEGGlobalizationRepository.GetOrgGlobalizationDetail(ctx, ctx.MOrgID);
			ctx.MZoneFormat = ((orgGlobalizationDetail == null) ? "China Standard Time" : orgGlobalizationDetail.MSystemZone);
			ctx.MDateFormat = ((orgGlobalizationDetail == null) ? "yyyy-MM-dd" : orgGlobalizationDetail.MSystemDate);
			ctx.MTimeFormat = ((orgGlobalizationDetail == null) ? "HH:mm:ss" : orgGlobalizationDetail.MSystemTime);
			ctx.MDigitGrpFormat = ((orgGlobalizationDetail == null) ? "," : orgGlobalizationDetail.MSystemDigitGroupingSymbol);
			ctx.MIsShowCSymbol = (orgGlobalizationDetail?.MIsShowCSymbol ?? false);
			SECUserRepository sECUserRepository = new SECUserRepository();
			OperationResult operationResult = sECUserRepository.InsertOrUpdate(ctx, new SECUserModel
			{
				MLastLoginAppID = ctx.MAppID,
				MLastLoginOrgID = ctx.MOrgID,
				MItemID = ctx.MUserID
			}, "MLastLoginAppID,MLastLoginOrgID");
			if (operationResult.Success)
			{
				return ctx;
			}
			throw new Exception(operationResult.ErrorMessageDetail);
		}

		public List<BASMyHomeModel> GetOrgInfoListByUserID(MContext ctx)
		{
			List<BASMyHomeModel> orgInfoListByUserID = dal.GetOrgInfoListByUserID(ctx);
			if (orgInfoListByUserID != null && orgInfoListByUserID.Count > 0)
			{
				foreach (BASMyHomeModel item in orgInfoListByUserID)
				{
					item.Url = GetOrgUrl(item.MVersionID, item.MRegProgress);
				}
			}
			return orgInfoListByUserID;
		}

		public DataGridJson<BASMyHomeModel> GetOrgInfoPageListByUserID(MContext ctx, BDOrganistationListFilter filter)
		{
			bool isSys = ctx.IsSys;
			ctx.IsSys = true;
			DataGridJson<BASMyHomeModel> dataGridJson = new DataGridJson<BASMyHomeModel>();
			try
			{
				dataGridJson = dal.GetOrgInfoListByPage(ctx, filter);
				foreach (BASMyHomeModel row in dataGridJson.rows)
				{
					row.Url = GetOrgUrl(row.MVersionID, row.MRegProgress);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = isSys;
			}
			return dataGridJson;
		}

		private string GetOrgUrl(int versionId, int progress)
		{
			if (progress == 0)
			{
				progress = 1;
			}
			BASOrgScheduleTypeEnum bASOrgScheduleTypeEnum = (BASOrgScheduleTypeEnum)progress;
			BASOrgScheduleTypeEnum bASOrgScheduleTypeEnum2 = BASOrgScheduleTypeEnum.GLSuccess;
			if (progress >= (int)bASOrgScheduleTypeEnum2)
			{
				return "/";
			}
			return $"/BD/Setup/{bASOrgScheduleTypeEnum}";
		}

		public OperationResult OrgRegisterForTry(MContext ctx, BASOrganisationModel model)
		{
			return dal.OrgRegisterForTry(ctx, model);
		}

		public int DeleteOrgById(MContext ctx, string orgId)
		{
			if (!SECPermissionRepository.HavePermission(ctx, "Org", "Change", orgId))
			{
				return -2;
			}
			int num = dal.DeleteOrgById(ctx, orgId);
			if (num > 0)
			{
				if (ctx.MOrgID == orgId)
				{
					ctx.MOrgID = null;
				}
				if (ctx.MLastLoginOrgId == orgId)
				{
					ctx.MLastLoginOrgId = null;
					ctx.MLastLoginOrgName = null;
				}
				ContextHelper.MContext = ctx;
			}
			return num;
		}
	}
}
