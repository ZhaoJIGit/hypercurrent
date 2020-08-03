using JieNor.Megi.Core;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTBizReport<T> where T : ReportFilterBase
	{
		[OperationContract]
		MActionResult<string> GetBizReportJson(T filter, string accessToken = null);
	}
}
