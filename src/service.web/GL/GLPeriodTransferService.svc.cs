using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.GL;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.GL
{
	public class GLPeriodTransferService : ServiceT<GLPeriodTransferModel>, IGLPeriodTransfer
	{
		private readonly IGLPeriodTransferBusiness biz = new GLPeriodTransferBusiness();

		public MActionResult<GLPeriodTransferModel> GetPeriodTransfer(GLPeriodTransferModel model, bool test = true, string accessToken = null)
		{
			IGLPeriodTransferBusiness iGLPeriodTransferBusiness = biz;
			return base.RunFunc(iGLPeriodTransferBusiness.GetPeriodTransfer, model, test, accessToken);
		}

		public MActionResult<GLPeriodTransferModel> CalculatePeriodTransfer(GLPeriodTransferModel model, string accessToken = null)
		{
			IGLPeriodTransferBusiness iGLPeriodTransferBusiness = biz;
			return base.RunFunc(iGLPeriodTransferBusiness.CalculatePeriodTransfer, model, accessToken);
		}

		public MActionResult<List<GLPeriodTransferModel>> GetExsitsAndCalculatedPeriodTransfer(GLPeriodTransferModel model, string accessToken = null)
		{
			IGLPeriodTransferBusiness iGLPeriodTransferBusiness = biz;
			return base.RunFunc(iGLPeriodTransferBusiness.GetExsitsAndCalculatedPeriodTransfer, model, accessToken);
		}
	}
}
