using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.Log.GlLog
{
	public class GlVoucherLogHelper
	{
		private static readonly GlVoucherLogBase svcLog = new GlVoucherLogBase();

		public static void SaveLog(MContext ctx, GLVoucherModel model)
		{
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				if (model.IsNew)
				{
					svcLog.CreateLog(ctx, model);
				}
				if (!model.IsNew)
				{
					svcLog.EditLog(ctx, model);
				}
				if (model.MStatus == 1)
				{
					svcLog.ApproveLog(ctx, model);
				}
			}
		}

		public static void UpdateStatusLog(MContext ctx, GLVoucherModel model, string status)
		{
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				if (int.Parse(status) == 1)
				{
					ApproveLog(ctx, model);
				}
				else if (int.Parse(status) == 0)
				{
					ReverseApproveLog(ctx, model);
				}
			}
		}

		public static List<CommandInfo> GetUpdateStatusLog(MContext ctx, GLVoucherModel model, string status)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				if (int.Parse(status) == 1)
				{
					list.AddRange(GetApproveLogCmd(ctx, model));
				}
				else if (int.Parse(status) == 0)
				{
					list.AddRange(GetReverseApproveLogCmd(ctx, model));
				}
			}
			return list;
		}

		public static List<CommandInfo> GetBatchUpdateStatusLogCmds(MContext ctx, List<GLVoucherModel> modelList, string status)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (modelList == null || modelList.Count() == 0)
			{
				return list;
			}
			List<GLVoucherModel> list2 = new List<GLVoucherModel>();
			foreach (GLVoucherModel model in modelList)
			{
				if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
				{
					list2.Add(model);
				}
			}
			if (list2 == null || list2.Count() == 0)
			{
				return list;
			}
			if (int.Parse(status) == 1)
			{
				list.AddRange(GetApproveLogCmd(ctx, list2));
			}
			else if (int.Parse(status) == 0)
			{
				list.AddRange(GetReverseApproveLogCmd(ctx, list2));
			}
			return list;
		}

		public static List<CommandInfo> GetSaveLog(MContext ctx, GLVoucherModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				if (model.IsNew)
				{
					list.Add(svcLog.GetCreateLogCmd(ctx, model));
				}
				if (!model.IsNew)
				{
					list.Add(svcLog.GetEditLogCmd(ctx, model));
				}
				if (model.MStatus == 1)
				{
					list.Add(svcLog.GetApproveLogCmd(ctx, model));
				}
			}
			return list;
		}

		public static void DeleteLog(MContext ctx, GLVoucherModel model)
		{
			svcLog.DeleteLog(ctx, model);
		}

		public static void ApproveLog(MContext ctx, GLVoucherModel model)
		{
			svcLog.ApproveLog(ctx, model);
		}

		public static void ReverseApproveLog(MContext ctx, GLVoucherModel model)
		{
			svcLog.ReverseApproveLog(ctx, model);
		}

		public static List<CommandInfo> GetApproveLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetApproveLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetCreateReverseCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MOVoucherID))
			{
				return new List<CommandInfo>();
			}
			CommandInfo createReverseLogCmd = svcLog.GetCreateReverseLogCmd(ctx, model);
			if (createReverseLogCmd == null)
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				createReverseLogCmd
			};
		}

		public static List<CommandInfo> GetApplyReverseCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MItemID))
			{
				return new List<CommandInfo>();
			}
			CommandInfo applyReverseLogCmd = svcLog.GetApplyReverseLogCmd(ctx, model);
			if (applyReverseLogCmd == null)
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				applyReverseLogCmd
			};
		}

		public static List<CommandInfo> GetApproveLogCmd(MContext ctx, List<GLVoucherModel> modelList)
		{
			if (modelList == null || modelList.Count() == 0)
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetApproveLogCmdList(ctx, modelList)
			};
		}

		public static List<CommandInfo> GetReverseApproveLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetReverseApproveLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetReverseApproveLogCmd(MContext ctx, List<GLVoucherModel> modelList)
		{
			if (modelList == null || modelList.Count() == 0)
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetReverseApproveLogCmd(ctx, modelList)
			};
		}

		public static List<CommandInfo> GetDeleteLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetDeleteLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetDeleteLogCmds(MContext ctx, List<GLVoucherModel> list)
		{
			if (list == null || !list.Any() || list.Exists((GLVoucherModel x) => string.IsNullOrWhiteSpace(x.MNumber)))
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetDeleteLogCmds(ctx, list)
			};
		}

		public static List<CommandInfo> GetCreateLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetCreateLogCmd(ctx, model)
			};
		}

		public static List<CommandInfo> GetCreateLogCmds(MContext ctx, List<GLVoucherModel> list)
		{
			if (list == null || !list.Any())
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetCreateLogCmds(ctx, list)
			};
		}

		public static List<CommandInfo> GetEditLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return new List<CommandInfo>();
			}
			return new List<CommandInfo>
			{
				svcLog.GetEditLogCmd(ctx, model)
			};
		}
	}
}
