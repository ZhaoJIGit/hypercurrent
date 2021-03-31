using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BAS
{
	public class BASOrganisationService : ServiceT<BASOrganisationModel>, IBASOrganisation
	{
		private readonly IBASOrganisationBusiness biz = new BASOrganisationBusiness();

		public MActionResult<OperationResult> CreateDemoCompany(string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.CreateDemoCompany, accessToken);
		}

		public MActionResult<BASOrganisationModel> GetDemoOrg(string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.GetDemoOrg, accessToken);
		}

		public MActionResult<OperationResult> Register(BASOrganisationModel model, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.Register, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateOrgInfo(BASOrgInfoModel info, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.UpdateOrgInfo, info, accessToken);
		}

		public MActionResult<BASOrgInfoModel> GetOrgInfo(ParamBase param, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.GetOrgInfo, param, accessToken);
		}

		public MActionResult<OperationResult> UpdateRegProgress(BASOrgScheduleTypeEnum type, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.UpdateRegProgress, type, accessToken);
		}

		public MActionResult<OperationResult> Update(BASOrganisationModel model, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.Update, model, accessToken);
		}

		public MActionResult<BASOrganisationModel> GetModel(string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.GetModel, accessToken);
		}

		public MActionResult<bool> IsOrgExist(string displayName, string excludeId, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.IsOrgExist, displayName, excludeId, accessToken);
		}

		public MActionResult<BASOrganisationModel> GetOrgBasicInfo(string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.GetOrgBasicInfo, accessToken);
		}

		public MActionResult<List<BASOrgModuleModel>> GetOrgModuleList(string moduleId = null, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.GetOrgModuleList, moduleId, accessToken);
		}

		public MActionResult<string> ChangeOrgById(string orgId, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunAction<string, string>(iBASOrganisationBusiness.ChangeOrgById, orgId, accessToken);
		}

		public MActionResult<BASOrganisationModel> GetModelById(string pkID, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.GetModelById, pkID, accessToken);
		}

		public MActionResult<BASOrganisationModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.GetDataModel(iBASOrganisationBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}

		public MActionResult<bool> ExistsByFilter(SqlWhere filter, string accessToken = null)
		{
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.ExistsByFilter(iBASOrganisationBusiness.ExistsByFilter, filter, accessToken);
		}

        public MActionResult<DataGridJson<BASOrganisationModel>> GetOrgList( string name, int pageIndex = 0, int pageSize = 10)
        {
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;
			return base.RunFunc(iBASOrganisationBusiness.GetOrgList, name, pageIndex, pageSize);
		}

        public MActionResult<OperationResult> UpdateStatus(string mItemId, int status)
        {
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;

			return base.RunFunc(iBASOrganisationBusiness.UpdateStatus, mItemId, status);
		}

        public MActionResult<OperationResult> Renew(string mItemId, DateTime time)
        {
			IBASOrganisationBusiness iBASOrganisationBusiness = biz;

			return base.RunFunc(iBASOrganisationBusiness.Renew, mItemId, time);
		}
    }
}
