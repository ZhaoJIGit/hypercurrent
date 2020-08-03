using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using System;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVBankApiBusiness : IIVBankApiBusiness
	{
		public string GetHistoryCurrentBalance(MContext ctx, string accountId, DateTime endDate)
		{
			return IVBankAPIReqRepository.GetHistoryCurrentBalance(ctx, accountId, endDate);
		}

		public string LogoutOnlineBank(MContext ctx)
		{
			return IVBankAPIReqRepository.LogoutOnlineBank(ctx);
		}

		public string GetAccountInfoDetail(MContext ctx, string accountId)
		{
			return IVBankAPIReqRepository.GetAccountInfoDetail(ctx, accountId);
		}

		public OperationResult LogonOnlineBank(MContext ctx, string userid, string password)
		{
			return IVBankAPIReqRepository.LogonOnlineBank(ctx, userid, password);
		}

		public OperationResult GetBankFeeds(MContext ctx, IVBankFeedsModel feedModel)
		{
			return IVBankAPIReqRepository.GetBankFeeds(ctx, feedModel);
		}
	}
}
