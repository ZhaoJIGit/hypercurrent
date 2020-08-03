using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASOrganisation
	{
		[OperationContract]
		MActionResult<OperationResult> CreateDemoCompany(string accessToken = null);

		[OperationContract]
		MActionResult<BASOrganisationModel> GetDemoOrg(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Register(BASOrganisationModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateOrgInfo(BASOrgInfoModel info, string accessToken = null);

		[OperationContract]
		MActionResult<BASOrgInfoModel> GetOrgInfo(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateRegProgress(BASOrgScheduleTypeEnum type, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Update(BASOrganisationModel model, string accessToken = null);

		[OperationContract]
		MActionResult<BASOrganisationModel> GetModel(string accessToken = null);

		[OperationContract]
		MActionResult<string> ChangeOrgById(string orgId, string accessToken = null);

		[OperationContract]
		MActionResult<BASOrganisationModel> GetModelById(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<bool> IsOrgExist(string displayName, string excludeId, string accessToken = null);

		[OperationContract]
		MActionResult<BASOrganisationModel> GetOrgBasicInfo(string accessToken = null);

		[OperationContract]
		MActionResult<List<BASOrgModuleModel>> GetOrgModuleList(string moduleId = null, string accessToken = null);

		[OperationContract]
		MActionResult<BASOrganisationModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<bool> ExistsByFilter(SqlWhere filter, string accessToken = null);
	}
}
