using JieNor.Megi.BusinessContract.REG;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ServiceContract.REG;

namespace JieNor.Megi.Service.Web.REG
{
	public class REGFinancialService : ServiceT<REGFinancialModel>, IREGFinancial
	{
		private readonly IREGFinancialBusiness biz = new REGFinancialBusiness();

		public MActionResult<OperationResult> UpdateByOrgID(REGFinancialModel model, string accessToken = null)
		{
			IREGFinancialBusiness iREGFinancialBusiness = biz;
			return base.RunFunc(iREGFinancialBusiness.UpdateByOrgID, model, accessToken);
		}

		public MActionResult<REGFinancialModel> GetByOrgID(REGFinancialModel model, string accessToken = null)
		{
			IREGFinancialBusiness iREGFinancialBusiness = biz;
			return base.RunFunc(iREGFinancialBusiness.GetByOrgID, model, accessToken);
		}

		public MActionResult<TaxPayerEnum> GetTaxPayer(string accessToken = null)
		{
			IREGFinancialBusiness iREGFinancialBusiness = biz;
			return base.RunFunc(iREGFinancialBusiness.GetTaxPayer, accessToken);
		}

		public MActionResult<REGFinancialModel> GetDataModelByFilter(SqlWhere filter, string accessToken = null)
		{
			IREGFinancialBusiness iREGFinancialBusiness = biz;
			return base.GetDataModelByFilter(iREGFinancialBusiness.GetDataModelByFilter, filter, accessToken);
		}
	}
}
