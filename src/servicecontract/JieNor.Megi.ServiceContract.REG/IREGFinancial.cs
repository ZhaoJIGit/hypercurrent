using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.REG
{
	[ServiceContract]
	public interface IREGFinancial
	{
		[OperationContract]
		MActionResult<OperationResult> UpdateByOrgID(REGFinancialModel model, string accessToken = null);

		[OperationContract]
		MActionResult<REGFinancialModel> GetByOrgID(REGFinancialModel model, string accessToken = null);

		[OperationContract]
		MActionResult<TaxPayerEnum> GetTaxPayer(string accessToken = null);

		[OperationContract]
		MActionResult<REGFinancialModel> GetDataModelByFilter(SqlWhere filter, string accessToken = null);
	}
}
