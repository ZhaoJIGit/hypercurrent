using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessContract.PA;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.PA;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.PA;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.PA
{
	public class PAPayrollBasicService : ServiceT<PAPaySettingModel>, IPAPayrollBasic
	{
		private readonly IPAPaySettingBussiness PaySettingBLL = new PAPaySettingBussiness();

		private readonly IBDEmpPayrollDetailBll EmpPayrollDetailBLL = new BDEmpPayrollDetailBll();

		private readonly IPAPayItemBussiness PAPayItemBLL = new PAPayItemBussiness();

		private readonly IPAPayItemGroupBussiness PayItemGroupBLL = new PAPayItemGroupBussiness();

		public MActionResult<OperationResult> UpdatePaySetting(PAPaySettingModel model, string accessToken = null)
		{
			IPAPaySettingBussiness paySettingBLL = PaySettingBLL;
			return base.RunFunc(paySettingBLL.InsertOrUpdate, model, accessToken);
		}

		public MActionResult<PAPaySettingModel> GetPaySetting(string accessToken = null)
		{
			IPAPaySettingBussiness paySettingBLL = PaySettingBLL;
			return base.RunFunc(paySettingBLL.GetModel, accessToken);
		}

		public MActionResult<BDPayrollDetailModel> GetEmpPayrollDetail(string employeeID, string accessToken = null)
		{
			IBDEmpPayrollDetailBll empPayrollDetailBLL = EmpPayrollDetailBLL;
			return base.RunFunc(empPayrollDetailBLL.GetModel, employeeID, accessToken);
		}

		public MActionResult<OperationResult> UpdateEmpPayrollDetail(BDPayrollDetailModel model, string accessToken = null)
		{
			IBDEmpPayrollDetailBll empPayrollDetailBLL = EmpPayrollDetailBLL;
			return base.RunFunc(empPayrollDetailBLL.InsertOrUpdate, model, accessToken);
		}

		public MActionResult<List<BDPayrollDetailModel>> GetEmpPayrollList(string employeeIds, string accessToken = null)
		{
			IBDEmpPayrollDetailBll empPayrollDetailBLL = EmpPayrollDetailBLL;
			return base.RunFunc(empPayrollDetailBLL.GetList, employeeIds, accessToken);
		}

		public MActionResult<List<PAPayItemGroupModel>> GetSalaryItemGroupList(string accessToken = null)
		{
			IPAPayItemGroupBussiness payItemGroupBLL = PayItemGroupBLL;
			return base.RunFunc(payItemGroupBLL.GetSalaryItemList, accessToken);
		}

		public MActionResult<PAPayItemGroupModel> GetSalaryGroupItemById(string id, string accessToken = null)
		{
			IPAPayItemGroupBussiness payItemGroupBLL = PayItemGroupBLL;
			return base.RunFunc(payItemGroupBLL.GetSalaryGroupItemById, id, accessToken);
		}

		public MActionResult<List<PAPayItemModel>> GetSalaryItemList(string accessToken = null)
		{
			IPAPayItemBussiness pAPayItemBLL = PAPayItemBLL;
			return base.RunFunc(pAPayItemBLL.GetSalaryItemList, accessToken);
		}

		public MActionResult<OperationResult> UpdateSalaryGroupItem(PAPayItemGroupModel model, string accessToken = null)
		{
			IPAPayItemGroupBussiness payItemGroupBLL = PayItemGroupBLL;
			return base.RunFunc(payItemGroupBLL.UpdateModel, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateSalaryItem(PAPayItemModel model, string accessToken = null)
		{
			IPAPayItemBussiness pAPayItemBLL = PAPayItemBLL;
			return base.RunFunc(pAPayItemBLL.UpdateModel, model, accessToken);
		}

		public MActionResult<PAPayItemModel> GetSalaryItemById(string id, string accessToken = null)
		{
			IPAPayItemBussiness pAPayItemBLL = PAPayItemBLL;
			return base.RunFunc(pAPayItemBLL.GetSalaryItemById, id, accessToken);
		}

		public MActionResult<List<SalaryItemTreeModel>> GetSalaryItemTreeList(bool includeInActive, string accessToken = null)
		{
			IPAPayItemBussiness pAPayItemBLL = PAPayItemBLL;
			return base.RunFunc(pAPayItemBLL.GetSalaryItemTreeList, includeInActive, accessToken);
		}

		public MActionResult<OperationResult> ForbiddenSalaryItem(string ids, string accessToken = null)
		{
			IPAPayItemBussiness pAPayItemBLL = PAPayItemBLL;
			return base.RunFunc(pAPayItemBLL.ForbiddenSalaryItem, ids, accessToken);
		}

		public MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null)
		{
			IPAPayItemBussiness pAPayItemBLL = PAPayItemBLL;
			return base.RunFunc(pAPayItemBLL.IsCanDeleteOrInactive, param, accessToken);
		}

		public MActionResult<List<PAPayItemModel>> GetDisableItemList(string accessToken = null)
		{
			IPAPayItemBussiness pAPayItemBLL = PAPayItemBLL;
			return base.RunFunc(pAPayItemBLL.GetDisableItemList, accessToken);
		}

		public MActionResult<OperationResult> Delete(ParamBase param, string accessToken = null)
		{
			IPAPayItemGroupBussiness payItemGroupBLL = PayItemGroupBLL;
			return base.RunFunc(payItemGroupBLL.Delete, param, accessToken);
		}
	}
}
