using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;

namespace JieNor.Megi.BusinessContract.REG
{
	public interface IREGFinancialBusiness : IDataContract<REGFinancialModel>
	{
		OperationResult UpdateByOrgID(MContext ctx, REGFinancialModel model);

		REGFinancialModel GetByOrgID(MContext ctx, REGFinancialModel model);

		TaxPayerEnum GetTaxPayer(MContext ctx);
	}
}
