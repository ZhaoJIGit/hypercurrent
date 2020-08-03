using JieNor.Megi.BusinessContract.COM;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.COM
{
	public class COMTableInfoBusiness : ICOMTableInfoBusiness
	{
		[NoAuthorization]
		public List<List<MTableColumnModel>> GetTableColumnModels(MContext ctx, string tableName)
		{
			return new COMTableInfoRepository().GetTableColumnModels(tableName, null);
		}
	}
}
