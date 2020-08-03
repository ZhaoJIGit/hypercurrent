using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Formula;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.GL;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.GL
{
	public class GLExcelService : ServiceT<GLExcelModel>, IGLExcel
	{
		private readonly IGLExcelBusiness biz = new GLExcelBusiness();

		public MActionResult<List<GLVoucherModel>> GetVoucherListByFilter(GLBalanceListFilterModel filter, string accessToken = null)
		{
			IGLExcelBusiness iGLExcelBusiness = biz;
			return base.RunFunc(iGLExcelBusiness.GetVoucherListByFilter, filter, accessToken);
		}

		public MActionResult<List<GLBalanceModel>> GetBalanceListByFilter(GLBalanceListFilterModel filter, string accessToken = null)
		{
			IGLExcelBusiness iGLExcelBusiness = biz;
			return base.RunFunc(iGLExcelBusiness.GetBalanceListByFilter, filter, accessToken);
		}

		public MActionResult<List<GLBalanceModel>> GetBalanceListWithTrackByFilter(GLBalanceListFilterModel filter, string accessToken = null)
		{
			IGLExcelBusiness iGLExcelBusiness = biz;
			return base.RunFunc(iGLExcelBusiness.GetBalanceListWithTrackByFilter, filter, accessToken);
		}

		public MActionResult<List<GLVoucherModel>> GetTransferVoucherList(GLBalanceListFilterModel filter, string accessToken = null)
		{
			IGLExcelBusiness iGLExcelBusiness = biz;
			return base.RunFunc(iGLExcelBusiness.GetTransferVoucherList, filter, accessToken);
		}

		public MActionResult<List<GLInitBalanceModel>> GetInitBalanceList(GLBalanceListFilterModel filter, string accessToken = null)
		{
			IGLExcelBusiness iGLExcelBusiness = biz;
			return base.RunFunc(iGLExcelBusiness.GetInitBalanceList, filter, accessToken);
		}

		public MActionResult<List<BDAccountModel>> GetAccountList(string accessToken = null)
		{
			IGLExcelBusiness iGLExcelBusiness = biz;
			return base.RunFunc(iGLExcelBusiness.GetAccountList, accessToken);
		}

		public MActionResult<DateTime> GetGLBeginDate(string accessToken = null)
		{
			IGLExcelBusiness iGLExcelBusiness = biz;
			return base.RunFunc(iGLExcelBusiness.GetGLBeginDate, accessToken);
		}

		public MActionResult<List<BatchFormaluModel>> RefreshFormula(List<BatchFormaluModel> formaluList, string accessToken = null)
		{
			IGLExcelBusiness iGLExcelBusiness = biz;
			return base.RunFunc(iGLExcelBusiness.RefreshFormula, formaluList, accessToken);
		}
	}
}
