using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASCountryBusiness
	{
		List<BASCountryModel> GetCountryList(MContext ctx);

		List<BASProvinceModel> GetProvinceList(MContext ctx, string countryId);
	}
}
