using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.REG
{
	public interface IREGTaxRateBusiness : IDataContract<REGTaxRateModel>
	{
		List<REGTaxRateModel> GetTaxRateList(MContext ctx, bool ignoreLocale = false);

		DataGridJson<REGTaxRateModel> GetTaxTateListByPage(MContext ctx, REGTaxTateListFilterModel filter);

		BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param);

		OperationResult ArchiveTaxRate(MContext ctx, string keyIDs, bool isActive);

		OperationResult GetUpdateTaxInfo(MContext ctx, int changeTaxType);
	}
}
