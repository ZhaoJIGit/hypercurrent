using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDAccountMatchLogService : ServiceT<BDAccountMatchLogModel>, IBDAccountMatchLog
	{
		private readonly IBDAccountMatchLogBusiness biz = new BDAccountMatchLogBusiness();

		public MActionResult<bool> ExistsByFilter(SqlWhere filter, string accessToken = null)
		{
			IBDAccountMatchLogBusiness iBDAccountMatchLogBusiness = biz;
			return base.ExistsByFilter(iBDAccountMatchLogBusiness.ExistsByFilter, filter, accessToken);
		}

		public MActionResult<List<IOAccountModel>> GetMatchLogList(string accessToken = null)
		{
			IBDAccountMatchLogBusiness iBDAccountMatchLogBusiness = biz;
			return base.RunFunc(iBDAccountMatchLogBusiness.GetMatchLogList, accessToken);
		}

		public MActionResult<OperationResult> SaveMatchLog(List<BDAccountEditModel> acctList, string accessToken = null)
		{
			IBDAccountMatchLogBusiness iBDAccountMatchLogBusiness = biz;
			return base.RunFunc(iBDAccountMatchLogBusiness.SaveMatchLog, acctList, accessToken);
		}
	}
}
