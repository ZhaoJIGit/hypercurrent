using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.SEC
{
	public class SECUserLoginLog : ISECUserLoginLog
	{
		private SECUserLoginLogRepository dal = new SECUserLoginLogRepository();

		public OperationResult InsertLoginLog(MContext ctx, SECUserLoginLogModel model)
		{
			if (ctx == null)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			if (model == null)
			{
				model = new SECUserLoginLogModel();
				model.IsNew = true;
				model.MUserID = ctx.MUserID;
				model.MOrgID = ctx.MOrgID;
				model.MLoginDate = ctx.DateNow;
			}
			return dal.InsertLoginLog(ctx, model);
		}

		public DataGridJson<SECUserLoginLogModel> GetUserLoginLogPageListByOrgId(MContext ctx, string orgId, SECUserLoginLogListFilter filter)
		{
			return dal.GetUserLoginLogPageListByOrgId(ctx, orgId, filter);
		}
	}
}
