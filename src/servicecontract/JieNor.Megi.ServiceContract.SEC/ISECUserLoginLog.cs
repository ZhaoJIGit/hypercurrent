using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.SEC
{
	[ServiceContract]
	public interface ISECUserLoginLog
	{
		[OperationContract]
		MActionResult<OperationResult> InsertLoginLog(SECUserLoginLogModel model, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<SECUserLoginLogModel>> GetUserLoginLogPageListByOrgId(string orgId, SECUserLoginLogListFilter filter, string accessToken = null);
	}
}
