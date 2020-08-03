using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.IV;
using System;

namespace JieNor.Megi.Service.Web.IV
{
	public class IVBankApiService : ServiceT<IVBankBillModel>, IIVBankApi
	{
		private readonly IIVBankApiBusiness biz = new IVBankApiBusiness();

		public MActionResult<OperationResult> LogonOnlineBank(string userid, string password, string accessToken = null)
		{
			IIVBankApiBusiness iIVBankApiBusiness = biz;
			return base.RunFunc(iIVBankApiBusiness.LogonOnlineBank, userid, password, accessToken);
		}

		public MActionResult<OperationResult> GetBankFeeds(IVBankFeedsModel feedModel, string accessToken = null)
		{
			IIVBankApiBusiness iIVBankApiBusiness = biz;
			return base.RunFunc(iIVBankApiBusiness.GetBankFeeds, feedModel, accessToken);
		}

		public MActionResult<string> LogoutOnlineBank(string accessToken)
		{
			IIVBankApiBusiness iIVBankApiBusiness = biz;
			return base.RunFunc(iIVBankApiBusiness.LogoutOnlineBank, accessToken);
		}

		public MActionResult<string> GetHistoryCurrentBalance(string accountId, DateTime endDate, string accessToken = null)
		{
			IIVBankApiBusiness iIVBankApiBusiness = biz;
			return base.RunFunc(iIVBankApiBusiness.GetHistoryCurrentBalance, accountId, endDate, accessToken);
		}
	}
}
