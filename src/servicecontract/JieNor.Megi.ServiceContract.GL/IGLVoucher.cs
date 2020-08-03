using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.GL
{
	[ServiceContract]
	public interface IGLVoucher
	{
		[OperationContract]
		MActionResult<GLVoucherModel> GetVoucherModel(string MItemID = null, int year = 0, int period = 0, int day = 0, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLVoucherModel>> GetVoucherModelList(List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLVoucherViewModel>> GetVoucherViewModelList(List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0, string accessToken = null);

		[OperationContract]
		MActionResult<GLVoucherModel> UpdateVoucher(GLVoucherModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteVoucherModels(List<string> pkIDS, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLVoucherModel>> GetRelateDeleteVoucherList(List<string> pkIDS, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ApproveVouchers(List<string> itemIDS, string status, string accessToken = null);

		[OperationContract]
		MActionResult<bool> ReorderVoucherNumber(int year, int period, int start = 1, string accessToken = null);

		[OperationContract]
		MActionResult<bool> IsMNumberUsed(int year, int period, string MNumber, string accessToken = null);

		[OperationContract]
		MActionResult<bool> CheckVoucherHasUnapproved(GLSettlementModel model, string accessToken = null);

		[OperationContract]
		MActionResult<GLVoucherModel> GetVoucherByPeriodTransfer(GLPeriodTransferModel model, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(bool isFromExport = false, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetNextVoucherNumber(int year, int period, string accessToken = null);

		[OperationContract]
		MActionResult<GLDashboardModel> GetDashboardData(int year, int period, int type = 0, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportVoucherList(List<GLVoucherModel> models, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLVoucherViewModel>> GetVoucherListForPrint(GLVoucherListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<GLVoucherModel> GetVoucherEditModel(GLVoucherModel model, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<GLVoucherViewModel>> GetVoucherModelPageList(GLVoucherListFilterModel model, string accessToken = null);

		[OperationContract]
		MActionResult<GLDashboardInfoModel> GetDashboardInfo(string accessToken = null);

		[OperationContract]
		MActionResult<bool> Exists(string pkID, bool includeDelete = false, string accessToken = null);
	}
}
