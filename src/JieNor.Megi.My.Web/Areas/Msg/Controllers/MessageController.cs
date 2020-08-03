using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.MSG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.My.Web.Controllers;
using JieNor.Megi.ServiceContract.MSG;
using System.Web.Mvc;

namespace JieNor.Megi.My.Web.Areas.Msg.Controllers
{
	public class MessageController : MyControllerBase
	{
		private IMSGMessage _msg;

		public MessageController(IMSGMessage msg)
		{
			_msg = msg;
		}

		public JsonResult SendMessage(MSGMessageEditModel msgModel)
		{
			MActionResult<OperationResult> data = _msg.SendMessage(msgModel, null);
			return base.Json(data);
		}

		public ActionResult MessageList()
		{
			base.SetModule("inbox");
			return base.View();
		}

		public ActionResult MessageCreate()
		{
			base.ViewData["UserList"] = _msg.GetRelativeUserList(null).ResultData;
			return base.View();
		}

		public ActionResult MessageReply(string id)
		{
			string text = base.Request["type"];
			text = (string.IsNullOrEmpty(text) ? "reply" : text);
			base.ViewData["MsgModel"] = _msg.GetMessageViewModel(ContextHelper.MAccessToken, id).ResultData;
			base.ViewData["UserList"] = _msg.GetRelativeUserList(ContextHelper.MAccessToken).ResultData;
			base.ViewData["Type"] = text;
			if (text == "reply")
			{
				_msg.ReadMessage(ContextHelper.MAccessToken, id);
			}
			return base.View();
		}

		public new JsonResult GetReceiveMessageCount()
		{
			MActionResult<int> receiveMessageCount = _msg.GetReceiveMessageCount(ContextHelper.MAccessToken);
			return base.Json(receiveMessageCount);
		}

		public JsonResult GetReceiveMessageList(MSGMessageListFilterModel filter)
		{
			MActionResult<DataGridJson<MSGMessageViewModel>> receiveMessageList = _msg.GetReceiveMessageList(filter, null);
			return base.Json(receiveMessageList);
		}

		public JsonResult GetSentMessageList(MSGMessageListFilterModel filter)
		{
			MActionResult<DataGridJson<MSGMessageViewModel>> sentMessageList = _msg.GetSentMessageList(filter, null);
			return base.Json(sentMessageList);
		}
	}
}
