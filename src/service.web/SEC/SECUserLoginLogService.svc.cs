using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.BusinessService.SEC;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.SEC;

namespace JieNor.Megi.Service.Web.SEC
{
	public class SECUserLoginLogService : ServiceT<SECUserLoginLogModel>, JieNor.Megi.ServiceContract.SEC.ISECUserLoginLog
	{
		private readonly JieNor.Megi.BusinessContract.SEC.ISECUserLoginLog biz = new SECUserLoginLog();

		public MActionResult<OperationResult> InsertLoginLog(SECUserLoginLogModel model, string accessToken = null)
		{
			JieNor.Megi.BusinessContract.SEC.ISECUserLoginLog iSECUserLoginLog = biz;
			return base.RunFunc(iSECUserLoginLog.InsertLoginLog, model, accessToken);
		}

		public MActionResult<DataGridJson<SECUserLoginLogModel>> GetUserLoginLogPageListByOrgId(string orgId, SECUserLoginLogListFilter filter, string accessToken = null)
		{
			JieNor.Megi.BusinessContract.SEC.ISECUserLoginLog iSECUserLoginLog = biz;
			return base.RunFunc(iSECUserLoginLog.GetUserLoginLogPageListByOrgId, orgId, filter, accessToken);
		}
	}
}
