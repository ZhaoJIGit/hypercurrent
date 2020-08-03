using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.PA
{
	[ServiceContract]
	public interface IPAPayrollBasic
	{
		[OperationContract]
		MActionResult<OperationResult> UpdatePaySetting(PAPaySettingModel model, string accessToken = null);

		[OperationContract]
		MActionResult<PAPaySettingModel> GetPaySetting(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateEmpPayrollDetail(BDPayrollDetailModel model, string accessToken = null);

		[OperationContract]
		MActionResult<BDPayrollDetailModel> GetEmpPayrollDetail(string employeeID, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDPayrollDetailModel>> GetEmpPayrollList(string employeeIds, string accessToken = null);

		[OperationContract]
		MActionResult<List<PAPayItemGroupModel>> GetSalaryItemGroupList(string accessToken = null);

		[OperationContract]
		MActionResult<PAPayItemGroupModel> GetSalaryGroupItemById(string id, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateSalaryGroupItem(PAPayItemGroupModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<PAPayItemModel>> GetSalaryItemList(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateSalaryItem(PAPayItemModel model, string accessToken = null);

		[OperationContract]
		MActionResult<PAPayItemModel> GetSalaryItemById(string id, string accessToken = null);

		[OperationContract]
		MActionResult<List<SalaryItemTreeModel>> GetSalaryItemTreeList(bool includeInActive, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ForbiddenSalaryItem(string id, string accessToken = null);

		[OperationContract]
		MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<List<PAPayItemModel>> GetDisableItemList(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Delete(ParamBase param, string accessToken = null);
	}
}
