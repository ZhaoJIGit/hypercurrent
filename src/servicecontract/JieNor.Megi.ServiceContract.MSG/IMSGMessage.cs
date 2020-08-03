using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.MSG;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.MSG
{
	[ServiceContract]
	public interface IMSGMessage
	{
		[OperationContract]
		MActionResult<OperationResult> SendMessage(MSGMessageEditModel msgModel, string accessToken = null);

		[OperationContract]
		MActionResult<string> ReadMessage(string msgId, string accessToken = null);

		[OperationContract]
		MActionResult<List<MSGMessageOrgUserModel>> GetRelativeUserList(string accessToken = null);

		[OperationContract]
		MActionResult<int> GetReceiveMessageCount(string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<MSGMessageViewModel>> GetReceiveMessageList(MSGMessageListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<MSGMessageViewModel>> GetSentMessageList(MSGMessageListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<MSGMessageViewModel> GetMessageViewModel(string msgId, string accessToken = null);
	}
}
