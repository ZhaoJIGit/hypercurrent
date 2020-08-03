using JieNor.Megi.BusinessContract.MSG;
using JieNor.Megi.BusinessService.MSG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.MSG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.MSG;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.MSG
{
	public class MSGMessageService : ServiceT<MSGMessageModel>, IMSGMessage
	{
		private readonly IMSGMessageBusiness biz = new MSGMessageBusiness();

		public MActionResult<OperationResult> SendMessage(MSGMessageEditModel msgModel, string accessToken = null)
		{
			IMSGMessageBusiness iMSGMessageBusiness = biz;
			return base.RunFunc(iMSGMessageBusiness.SendMessage, msgModel, accessToken);
		}

		public MActionResult<string> ReadMessage(string msgId, string accessToken = null)
		{
			IMSGMessageBusiness iMSGMessageBusiness = biz;
			return base.RunAction<string, string>(iMSGMessageBusiness.ReadMessage, msgId, accessToken);
		}

		public MActionResult<List<MSGMessageOrgUserModel>> GetRelativeUserList(string accessToken = null)
		{
			IMSGMessageBusiness iMSGMessageBusiness = biz;
			return base.RunFunc(iMSGMessageBusiness.GetRelativeUserList, accessToken);
		}

		public MActionResult<int> GetReceiveMessageCount(string accessToken = null)
		{
			IMSGMessageBusiness iMSGMessageBusiness = biz;
			return base.RunFunc(iMSGMessageBusiness.GetReceiveMessageCount, accessToken);
		}

		public MActionResult<DataGridJson<MSGMessageViewModel>> GetReceiveMessageList(MSGMessageListFilterModel filter, string accessToken = null)
		{
			IMSGMessageBusiness iMSGMessageBusiness = biz;
			return base.RunFunc(iMSGMessageBusiness.GetReceiveMessageList, filter, accessToken);
		}

		public MActionResult<DataGridJson<MSGMessageViewModel>> GetSentMessageList(MSGMessageListFilterModel filter, string accessToken = null)
		{
			IMSGMessageBusiness iMSGMessageBusiness = biz;
			return base.RunFunc(iMSGMessageBusiness.GetSentMessageList, filter, accessToken);
		}

		public MActionResult<MSGMessageViewModel> GetMessageViewModel(string msgId, string accessToken = null)
		{
			IMSGMessageBusiness iMSGMessageBusiness = biz;
			return base.RunFunc(iMSGMessageBusiness.GetMessageViewModel, msgId, accessToken);
		}
	}
}
