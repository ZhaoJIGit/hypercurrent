using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASOrganisationBusiness : IDataContract<BASOrganisationModel>
	{
		OperationResult CreateDemoCompany(MContext ctx);

		BASOrganisationModel GetDemoOrg(MContext ctx);

		OperationResult Register(MContext ctx, BASOrganisationModel model);

		OperationResult UpdateOrgInfo(MContext ctx, BASOrgInfoModel info);

		BASOrgInfoModel GetOrgInfo(MContext ctx, ParamBase param);

		OperationResult UpdateRegSchedule(MContext ctx, BASOrganisationModel model);

		OperationResult UpdateRegProgress(MContext context, BASOrgScheduleTypeEnum type);

		OperationResult Update(MContext ctx, BASOrganisationModel model);

		BASOrganisationModel GetModel(MContext ctx);

		void ChangeOrgById(MContext ctx, string orgId);

		BASOrganisationModel GetModelById(MContext ctx, string pkID);

		bool IsOrgExist(MContext ctx, string displayName, string excludeId);

		BASOrganisationModel GetOrgBasicInfo(MContext ctx);

		List<BASOrgModuleModel> GetOrgModuleList(MContext ctx, string moduleId = null);
	}
}
