using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.SEC
{
	public interface ISECUserLoginLog
	{
		OperationResult InsertLoginLog(MContext ctx, SECUserLoginLogModel model);

		DataGridJson<SECUserLoginLogModel> GetUserLoginLogPageListByOrgId(MContext ctx, string orgId, SECUserLoginLogListFilter filter);
	}
}
