using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.COM
{
	public interface ICOMTableInfoBusiness
	{
		List<List<MTableColumnModel>> GetTableColumnModels(MContext ctx, string tableName);
	}
}
