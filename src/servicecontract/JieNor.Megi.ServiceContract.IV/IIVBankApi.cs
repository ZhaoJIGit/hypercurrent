using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVBankApi
	{
		[OperationContract]
		MActionResult<OperationResult> LogonOnlineBank(string userid, string password, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> GetBankFeeds(IVBankFeedsModel feedModel, string accessToken = null);

		[OperationContract]
		MActionResult<string> LogoutOnlineBank(string accessToken = null);

		[OperationContract]
		MActionResult<string> GetHistoryCurrentBalance(string accountId, DateTime endDate, string accessToken = null);
	}
}
