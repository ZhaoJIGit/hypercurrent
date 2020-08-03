using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVBankApiBusiness
	{
		string GetHistoryCurrentBalance(MContext ctx, string accountId, DateTime endDate);

		string LogoutOnlineBank(MContext ctx);

		string GetAccountInfoDetail(MContext ctx, string accountId);

		OperationResult LogonOnlineBank(MContext ctx, string userid, string password);

		OperationResult GetBankFeeds(MContext ctx, IVBankFeedsModel feedModel);
	}
}
