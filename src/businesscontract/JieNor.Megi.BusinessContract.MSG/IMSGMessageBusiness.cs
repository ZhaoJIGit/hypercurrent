using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.MSG;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.MSG
{
	public interface IMSGMessageBusiness
	{
		OperationResult SendMessage(MContext ctx, MSGMessageEditModel msgModel);

		void ReadMessage(MContext ctx, string msgId);

		List<MSGMessageOrgUserModel> GetRelativeUserList(MContext ctx);

		int GetReceiveMessageCount(MContext ctx);

		DataGridJson<MSGMessageViewModel> GetReceiveMessageList(MContext ctx, MSGMessageListFilterModel filter);

		DataGridJson<MSGMessageViewModel> GetSentMessageList(MContext ctx, MSGMessageListFilterModel filter);

		MSGMessageViewModel GetMessageViewModel(MContext ctx, string msgId);
	}
}
