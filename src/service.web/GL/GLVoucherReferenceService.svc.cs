using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.GL;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.GL
{
	public class GLVoucherReferenceService : ServiceT<GLVoucherReferenceModel>, IGLVoucherReference
	{
		private readonly IGLVoucherReferenceBusiness biz = new GLVoucherReferenceBusiness();

		public MActionResult<OperationResult> InsertReference(GLVoucherReferenceModel model, string accessToken = null)
		{
			IGLVoucherReferenceBusiness iGLVoucherReferenceBusiness = biz;
			return base.RunFunc(iGLVoucherReferenceBusiness.InsertReference, model, accessToken);
		}

		public MActionResult<List<GLVoucherReferenceModel>> GetReferenceList(int size, string accessToken = null)
		{
			IGLVoucherReferenceBusiness iGLVoucherReferenceBusiness = biz;
			return base.RunFunc(iGLVoucherReferenceBusiness.GetReferenceList, size, accessToken);
		}

		public MActionResult<DataGridJson<GLVoucherReferenceModel>> GetModelPageList(SqlWhere filter, bool includeDelete = false, string accessToken = null)
		{
			IGLVoucherReferenceBusiness iGLVoucherReferenceBusiness = biz;
			return base.RunFunc(iGLVoucherReferenceBusiness.GetModelPageList, filter, includeDelete, accessToken);
		}

		public MActionResult<OperationResult> Delete(string pkID, string accessToken = null)
		{
			IGLVoucherReferenceBusiness iGLVoucherReferenceBusiness = biz;
			return base.Delete(iGLVoucherReferenceBusiness.Delete, pkID, accessToken);
		}
	}
}
