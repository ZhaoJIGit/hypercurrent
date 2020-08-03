using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLVoucherBusiness : IDataContract<GLVoucherModel>
	{
		GLVoucherModel GetVoucherModel(MContext ctx, string MItemID = null, int year = 0, int period = 0, int day = 0);

		List<GLVoucherModel> GetVoucherModelList(MContext ctx, List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0);

		List<GLVoucherViewModel> GetVoucherViewModelList(MContext ctx, List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0);

		GLVoucherModel UpdateVoucher(MContext ctx, GLVoucherModel model);

		OperationResult UpdateVouchers(MContext ctx, List<GLVoucherModel> models, List<CommandInfo> list = null, int nowStatus = -2, int oldStatus = -2);

		List<GLVoucherModel> GetRelateDeleteVoucherList(MContext ctx, List<string> pkIDS);

		OperationResult DeleteVoucherModels(MContext ctx, List<string> pkIDS);

		OperationResult ApproveVouchers(MContext ctx, List<string> itemIDS, string status);

		bool ReorderVoucherNumber(MContext ctx, int year, int period, int start = 1);

		bool IsMNumberUsed(MContext ctx, int year, int period, string MNumber);

		bool CheckVoucherHasUnapproved(MContext ctx, GLSettlementModel model);

		GLVoucherModel GetVoucherByPeriodTransfer(MContext ctx, GLPeriodTransferModel model);

		ImportTemplateModel GetImportTemplateModel(MContext ctx, bool isFromExport = false);

		string GetNextVoucherNumber(MContext ctx, int year, int period);

		GLDashboardModel GetDashboardData(MContext ctx, int year, int period, int type = 0);

		OperationResult ImportVoucherList(MContext ctx, List<GLVoucherModel> models);

		List<GLVoucherViewModel> GetVoucherListForPrint(MContext ctx, GLVoucherListFilterModel filter);

		GLVoucherModel GetVoucherEditModel(MContext ctx, GLVoucherModel model);

		DataGridJson<GLVoucherViewModel> GetVoucherModelPageList(MContext ctx, GLVoucherListFilterModel filter);

		GLDashboardInfoModel GetDashboardInfo(MContext ctx);
	}
}
