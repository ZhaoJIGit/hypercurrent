using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.GL;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.GL
{
	public class GLVoucherService : ServiceT<GLVoucherModel>, IGLVoucher
	{
		private readonly IGLVoucherBusiness biz = new GLVoucherBusiness();

		public MActionResult<GLVoucherModel> GetVoucherModel(string MItemID = null, int year = 0, int period = 0, int day = 0, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetVoucherModel, MItemID, year, period, day, accessToken);
		}

		public MActionResult<List<GLVoucherModel>> GetVoucherModelList(List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetVoucherModelList, pkIDS, includeDraft, year, period, accessToken);
		}

		public MActionResult<List<GLVoucherViewModel>> GetVoucherViewModelList(List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetVoucherViewModelList, pkIDS, includeDraft, year, period, accessToken);
		}

		public MActionResult<List<GLVoucherViewModel>> GetVoucherListForPrint(GLVoucherListFilterModel filter, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetVoucherListForPrint, filter, accessToken);
		}

		public MActionResult<GLVoucherModel> UpdateVoucher(GLVoucherModel model, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.UpdateVoucher, model, accessToken);
		}

		public MActionResult<OperationResult> DeleteVoucherModels(List<string> pkIDS, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.DeleteVoucherModels, pkIDS, accessToken);
		}

		public MActionResult<OperationResult> ApproveVouchers(List<string> itemIDS, string status, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.ApproveVouchers, itemIDS, status, accessToken);
		}

		public MActionResult<List<GLVoucherModel>> GetRelateDeleteVoucherList(List<string> pkIDS, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetRelateDeleteVoucherList, pkIDS, accessToken);
		}

		public MActionResult<bool> ReorderVoucherNumber(int year, int period, int start, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.ReorderVoucherNumber, year, period, start, accessToken);
		}

		public MActionResult<string> GetNextVoucherNumber(int year, int period, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetNextVoucherNumber, year, period, accessToken);
		}

		public MActionResult<bool> IsMNumberUsed(int year, int period, string MNumber, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.IsMNumberUsed, year, period, MNumber, accessToken);
		}

		public MActionResult<bool> CheckVoucherHasUnapproved(GLSettlementModel model, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.CheckVoucherHasUnapproved, model, accessToken);
		}

		public MActionResult<GLVoucherModel> GetVoucherByPeriodTransfer(GLPeriodTransferModel model, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetVoucherByPeriodTransfer, model, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(bool isFromExport = false, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetImportTemplateModel, isFromExport, accessToken);
		}

		public MActionResult<OperationResult> ImportVoucherList(List<GLVoucherModel> models, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.ImportVoucherList, models, accessToken);
		}

		public MActionResult<GLDashboardModel> GetDashboardData(int year, int period, int type = 0, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetDashboardData, year, period, type, accessToken);
		}

		public MActionResult<GLVoucherModel> GetVoucherEditModel(GLVoucherModel model, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetVoucherEditModel, model, accessToken);
		}

		public MActionResult<DataGridJson<GLVoucherViewModel>> GetVoucherModelPageList(GLVoucherListFilterModel model, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetVoucherModelPageList, model, accessToken);
		}

		public MActionResult<GLDashboardInfoModel> GetDashboardInfo(string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.RunFunc(iGLVoucherBusiness.GetDashboardInfo, accessToken);
		}

		public MActionResult<bool> Exists(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IGLVoucherBusiness iGLVoucherBusiness = biz;
			return base.Exists(iGLVoucherBusiness.Exists, pkID, includeDelete, accessToken);
		}
	}
}
