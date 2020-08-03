using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDAccountMatchLog
	{
		[OperationContract]
		MActionResult<List<IOAccountModel>> GetMatchLogList(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveMatchLog(List<BDAccountEditModel> acctList, string accessToken = null);

		[OperationContract]
		MActionResult<bool> ExistsByFilter(SqlWhere filter, string accessToken = null);
	}
}
