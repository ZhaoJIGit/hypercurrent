using JieNor.Megi.EntityModel.Enum;

namespace JieNor.Megi.Core.Utility
{
	public static class FriendlyMessage
	{
		public static string ToMessage(this SysOperate sysOp, bool status)
		{
			string result = "";
			switch (sysOp)
			{
			case SysOperate.Add:
				result = (status ? SysMessage.AddSuccess.ToMessage() : SysMessage.AddError.ToMessage());
				break;
			case SysOperate.Load:
				result = (status ? SysMessage.LoadSuccess.ToMessage() : SysMessage.LoadError.ToMessage());
				break;
			case SysOperate.Update:
				result = (status ? SysMessage.UpdateSuccess.ToMessage() : SysMessage.UpdateError.ToMessage());
				break;
			case SysOperate.Delete:
				result = (status ? SysMessage.DeleteSuccess.ToMessage() : SysMessage.DeleteError.ToMessage());
				break;
			case SysOperate.Operate:
				result = (status ? SysMessage.OperateSuccess.ToMessage() : SysMessage.OperateError.ToMessage());
				break;
			case SysOperate.UnkownError:
				result = SysMessage.UnkownError.ToMessage();
				break;
			case SysOperate.ParamError:
				result = SysMessage.ParamError.ToMessage();
				break;
			}
			return result;
		}

		public static string ToMessage(this SysMessage code)
		{
			//string text = "";
			switch (code)
			{
			case SysMessage.AddSuccess:
				return "添加成功!";
			case SysMessage.AddError:
				return "添加失败!";
			case SysMessage.DeleteSuccess:
				return "删除成功!";
			case SysMessage.DeleteError:
				return "删除失败!";
			case SysMessage.LoadSuccess:
				return "加载成功!";
			case SysMessage.LoadError:
				return "加载失败!";
			case SysMessage.OperateSuccess:
				return "操作成功!";
			case SysMessage.OperateError:
				return "操作失败!";
			case SysMessage.UpdateSuccess:
				return "更新成功!";
			case SysMessage.UpdateError:
				return "更新失败!";
			case SysMessage.UnkownError:
				return "未知错误!";
			case SysMessage.ParamError:
				return "参数错误!";
			default:
				return "错误";
			}
		}
	}
}
