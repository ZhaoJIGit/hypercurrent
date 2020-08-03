using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.Log.TableLog
{
	public class FPTableLogHelper
	{
		private static readonly FPTableLogBase svcLog = new FPTableLogBase();

		public static void SaveLog(MContext ctx, FPTableViewModel model)
		{
			if (model.IsNew)
			{
				svcLog.CreateLog(ctx, model);
			}
			else
			{
				svcLog.EditLog(ctx, model);
			}
		}

		public static List<CommandInfo> SaveCmdLog(MContext ctx, FPTableViewModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (model.IsNew)
			{
				list.Add(svcLog.GetCreateLogCmd(ctx, model));
			}
			else
			{
				list.Add(svcLog.GetEditLogCmd(ctx, model));
			}
			return list;
		}

		public static void DeleteLog(MContext ctx, FPTableViewModel model)
		{
			svcLog.DeleteLog(ctx, model);
		}

		public static List<CommandInfo> GetDeleteLogCmdLog(MContext ctx, FPTableViewModel model)
		{
			return new List<CommandInfo>
			{
				svcLog.GetDeleteLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetCreateLogCmdLog(MContext ctx, FPTableViewModel model)
		{
			return new List<CommandInfo>
			{
				svcLog.GetCreateLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetEditLogCmd(MContext ctx, FPTableViewModel model)
		{
			return new List<CommandInfo>
			{
				svcLog.GetEditLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetManualReconcileLogCmd(MContext ctx, FPFapiaoReconcileModel model)
		{
			return new List<CommandInfo>
			{
				svcLog.GetManualReconcileLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetRemoveReoncileLogCmd(MContext ctx, FPFapiaoReconcileModel model)
		{
			return new List<CommandInfo>
			{
				svcLog.GetRemoveReconcileLogCmd(ctx, model)
			};
		}
	}
}
