using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.GL
{
	[ServiceContract]
	public interface IGLPeriodTransfer
	{
		[OperationContract]
		MActionResult<GLPeriodTransferModel> CalculatePeriodTransfer(GLPeriodTransferModel model, string accessToken = null);

		[OperationContract]
		MActionResult<GLPeriodTransferModel> GetPeriodTransfer(GLPeriodTransferModel model, bool test = true, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLPeriodTransferModel>> GetExsitsAndCalculatedPeriodTransfer(GLPeriodTransferModel model, string accessToken = null);
	}
}
