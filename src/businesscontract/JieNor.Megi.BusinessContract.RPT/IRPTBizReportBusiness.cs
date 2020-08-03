using JieNor.Megi.Core;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.RPT
{
	public interface IRPTBizReportBusiness<T> where T : ReportFilterBase
	{
		string GetBizReportJson(MContext ctx, T filter);
	}
}
