using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASTimezoneBusiness : IBASTimezoneBusiness
	{
		public List<BASTimezoneModel> GetList(MContext context)
		{
			return BASTimezoneRepository.GetList(context.MLCID);
		}
	}
}
