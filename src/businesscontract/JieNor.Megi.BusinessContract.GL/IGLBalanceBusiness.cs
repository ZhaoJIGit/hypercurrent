using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLBalanceBusiness : IDataContract<GLBalanceModel>
	{
		Dictionary<string, string> GetAccountingPeriod(MContext ctx);
	}
}
