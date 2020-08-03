using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTExpenseClaimBusiness : IRPTExpenseClaimBusiness, IRPTBizReportBusiness<RPTExpenseClaimFilterModel>
	{
		private RPTExpenseClaimRepository repository;

		public string GetBizReportJson(MContext ctx, RPTExpenseClaimFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				string baseCurrencyID = GetBaseCurrencyID(ctx);
				repository = new RPTExpenseClaimRepository(baseCurrencyID);
				return repository.ExpenseClaimList(ctx, filter);
			});
		}

		private string GetBaseCurrencyID(MContext ctx)
		{
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			BASCurrencyViewModel @base = rEGCurrencyRepository.GetBase(ctx, false, null, null);
			return @base.MCurrencyID;
		}

		public string GetBizSubReportJson(MContext ctx, RPTExpenseClaimDeatailFilterModel filter)
		{
			IRPTReportBusiness iRPTReportBusiness = new RPTReportBusiness();
			return iRPTReportBusiness.GetBizReportJson(ctx, filter, delegate
			{
				string baseCurrencyID = GetBaseCurrencyID(ctx);
				repository = new RPTExpenseClaimRepository(baseCurrencyID);
				return repository.ExpenseClaimDetailList(ctx, filter);
			});
		}
	}
}
