using JieNor.Megi.BusinessContract.MSG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.MSG;
using JieNor.Megi.DataRepository.MSG;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.MSG
{
	public class MSGMessageBusiness : IMSGMessageBusiness
	{
		private static MSGMessageRepository _msg = new MSGMessageRepository();

		public OperationResult SendMessage(MContext ctx, MSGMessageEditModel msgModel)
		{
			return _msg.SendMessage(ctx, msgModel);
		}

		public void ReadMessage(MContext ctx, string msgId)
		{
			_msg.ReadMessage(msgId, ctx);
		}

		public List<MSGMessageOrgUserModel> GetRelativeUserList(MContext ctx)
		{
			return _msg.GetRelativeUserList(ctx);
		}

		public int GetReceiveMessageCount(MContext ctx)
		{
			return _msg.GetReceiveMessageCount(ctx);
		}

		public DataGridJson<MSGMessageViewModel> GetReceiveMessageList(MContext ctx, MSGMessageListFilterModel filter)
		{
			return _msg.GetReceiveMessageList(ctx, filter);
		}

		public DataGridJson<MSGMessageViewModel> GetSentMessageList(MContext ctx, MSGMessageListFilterModel filter)
		{
			return _msg.GetSentMessageList(ctx, filter);
		}

		public MSGMessageViewModel GetMessageViewModel(MContext ctx, string msgId)
		{
			return _msg.GetMessageViewModel(msgId, ctx);
		}
	}
}
