using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.DataRepository
{
	public class CommonLogHelper
	{
		public static CommandInfo GetUploadAttachmentLogCmd(MContext ctx, string pkId, string fileName)
		{
			return OptLog.GetAddLogCommand(OptLogTemplate.Attachment_Upload, ctx, pkId, true, fileName);
		}

		public static void UploadAttachmentLog(MContext ctx, string pkId, string fileName)
		{
			OptLog.AddLog(OptLogTemplate.Attachment_Upload, ctx, pkId, fileName);
		}

		public static void RemoveAttachmentLog(MContext ctx, string pkId, string fileName)
		{
			OptLog.AddLog(OptLogTemplate.Attachment_Remove, ctx, pkId, fileName);
		}

		public static void DeleteAttachmentLog(MContext ctx, string pkId, string fileName)
		{
			OptLog.AddLog(OptLogTemplate.Attachment_Delete, ctx, pkId, fileName);
		}
	}
}
