using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDAccountMatchLogBusiness : IDataContract<BDAccountMatchLogModel>
	{
		List<IOAccountModel> GetMatchLogList(MContext ctx);

		OperationResult SaveMatchLog(MContext ctx, List<BDAccountEditModel> acctList);
	}
}
