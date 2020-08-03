using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASTimezoneBusiness
	{
		List<BASTimezoneModel> GetList(MContext context);
	}
}
