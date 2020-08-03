using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.GL;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.GL
{
	public class GLDocVoucherService : ServiceT<GLDocVoucherModel>, IGLDocVoucher
	{
		private readonly IGLDocVoucherBusiness biz = new GLDocVoucherBusiness();

		public MActionResult<DataGridJson<GLDocEntryVoucherModel>> GetDocVoucherModelList(GLDocVoucherFilterModel filter, string accessToken = null)
		{
			IGLDocVoucherBusiness iGLDocVoucherBusiness = biz;
			return base.RunFunc(iGLDocVoucherBusiness.GetDocVoucherModelList, filter, accessToken);
		}

		public MActionResult<OperationResult> CreateDocVoucher(List<GLDocEntryVoucherModel> list, bool create, string accessToken = null)
		{
			IGLDocVoucherBusiness iGLDocVoucherBusiness = biz;
			return base.RunFunc(iGLDocVoucherBusiness.CreateDocVoucher, list, create, accessToken);
		}

		public MActionResult<OperationResult> DeleteDocVoucher(List<GLDocEntryVoucherModel> list, string accessToken = null)
		{
			IGLDocVoucherBusiness iGLDocVoucherBusiness = biz;
			return base.RunFunc(iGLDocVoucherBusiness.DeleteDocVoucher, list, accessToken);
		}

		public MActionResult<List<string>> GetUpdatedDocTable(DateTime lastQueryTime, string accessToken = null)
		{
			IGLDocVoucherBusiness iGLDocVoucherBusiness = biz;
			return base.RunFunc(iGLDocVoucherBusiness.GetUpdatedDocTable, lastQueryTime, accessToken);
		}

		public MActionResult<OperationResult> ResetDocVoucher(List<string> docIDs, int docType, string accessToken = null)
		{
			IGLDocVoucherBusiness iGLDocVoucherBusiness = biz;
			return base.RunFunc(iGLDocVoucherBusiness.ResetDocVoucher, docIDs, docType, accessToken);
		}
	}
}
