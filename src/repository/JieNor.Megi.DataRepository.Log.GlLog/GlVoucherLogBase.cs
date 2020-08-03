using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.Log.GlLog
{
	public class GlVoucherLogBase : BizLogBase, IGLVoucherLog
	{
		protected virtual OptLogTemplate TemplateCreate => OptLogTemplate.GL_Voucher_Created;

		protected virtual OptLogTemplate TemplateEdit => OptLogTemplate.GL_Voucher_Edited;

		protected virtual OptLogTemplate TemplateDeleted => OptLogTemplate.GL_Voucher_Deleted;

		protected virtual OptLogTemplate TemplateApproval => OptLogTemplate.GL_Voucher_Approval;

		protected virtual OptLogTemplate TemplateReverseApprove => OptLogTemplate.GL_Voucher_ReverseApprove;

		protected virtual OptLogTemplate TemplateCreateReverse => OptLogTemplate.GL_Voucher_CreateReverse;

		protected virtual OptLogTemplate TemplateApplyReverse => OptLogTemplate.GL_Voucher_ApplyReverse;

		public void CreateLog(MContext ctx, GLVoucherModel model)
		{
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				base.AddLog(TemplateCreate, ctx, model.MItemID, model.MNumber, model.MAttachments);
			}
		}

		public CommandInfo GetCreateReverseLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MOVoucherID))
			{
				return null;
			}
			return base.GetLogCmd(TemplateCreateReverse, ctx, model.MOVoucherID, false, ctx.DateNow);
		}

		public CommandInfo GetApplyReverseLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MItemID))
			{
				return null;
			}
			return base.GetLogCmd(TemplateApplyReverse, ctx, model.MItemID, true, ctx.DateNow);
		}

		public void EditLog(MContext ctx, GLVoucherModel model)
		{
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				base.AddLog(TemplateEdit, ctx, model.MItemID, model.MNumber, model.MAttachments);
			}
		}

		public void DeleteLog(MContext ctx, GLVoucherModel model)
		{
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				base.AddLog(TemplateDeleted, ctx, model.MItemID, DateTime.Now);
			}
		}

		public void ApproveLog(MContext ctx, GLVoucherModel model)
		{
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				base.AddLog(TemplateApproval, ctx, model.MItemID, DateTime.Now);
			}
		}

		public void ReverseApproveLog(MContext ctx, GLVoucherModel model)
		{
			if (model != null && !string.IsNullOrWhiteSpace(model.MNumber))
			{
				base.AddLog(TemplateReverseApprove, ctx, model.MItemID, DateTime.Now);
			}
		}

		public CommandInfo GetDeleteLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return null;
			}
			return base.GetLogCmd(TemplateDeleted, ctx, model.MItemID, true, DateTime.Now);
		}

		public CommandInfo GetDeleteLogCmds(MContext ctx, List<GLVoucherModel> list)
		{
			if (list == null || !list.Any() || list.Exists((GLVoucherModel x) => string.IsNullOrWhiteSpace(x.MNumber)))
			{
				return null;
			}
			return base.GetLogCmdList(TemplateDeleted, ctx, (from x in list
			select x.MItemID).ToList(), true, new Dictionary<string, List<object>>());
		}

		public CommandInfo GetApproveLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return null;
			}
			return base.GetLogCmd(TemplateApproval, ctx, model.MItemID, true, DateTime.Now);
		}

		public CommandInfo GetApproveLogCmdList(MContext ctx, List<GLVoucherModel> modelList)
		{
			if (modelList == null || modelList.Count() == 0)
			{
				return null;
			}
			List<string> list = (from x in modelList
			select x.MItemID).ToList();
			Dictionary<string, List<object>> formatValueList = new Dictionary<string, List<object>>();
			list.ForEach(delegate(string x)
			{
				if (!formatValueList.Keys.Contains(x))
				{
					formatValueList.Add(x, new List<object>
					{
						(object)ctx.DateNow
					});
				}
			});
			return base.GetLogCmdList(TemplateApproval, ctx, list, true, formatValueList);
		}

		public CommandInfo GetReverseApproveLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return null;
			}
			return base.GetLogCmd(TemplateReverseApprove, ctx, model.MItemID, true, DateTime.Now);
		}

		public CommandInfo GetReverseApproveLogCmd(MContext ctx, List<GLVoucherModel> modelList)
		{
			if (modelList == null || modelList.Count() == 0)
			{
				return null;
			}
			List<string> list = (from x in modelList
			select x.MItemID).ToList();
			Dictionary<string, List<object>> formatValueList = new Dictionary<string, List<object>>();
			list.ForEach(delegate(string x)
			{
				if (!formatValueList.Keys.Contains(x))
				{
					formatValueList.Add(x, new List<object>
					{
						(object)ctx.DateNow
					});
				}
			});
			return base.GetLogCmdList(TemplateReverseApprove, ctx, list, true, formatValueList);
		}

		public CommandInfo GetCreateLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return null;
			}
			return base.GetLogCmd(TemplateCreate, ctx, model.MItemID, false, model.MNumber, model.MAttachments);
		}

		public CommandInfo GetCreateLogCmds(MContext ctx, List<GLVoucherModel> list)
		{
			if (list == null || !list.Any())
			{
				return null;
			}
			Dictionary<string, List<object>> paramValues = new Dictionary<string, List<object>>();
			list.ForEach(delegate(GLVoucherModel x)
			{
				paramValues.Add(x.MItemID, new List<object>
				{
					(object)x.MNumber,
					(object)x.MAttachments
				});
			});
			return base.GetLogCmdList(TemplateCreate, ctx, (from x in list
			select x.MItemID).ToList(), false, paramValues);
		}

		public CommandInfo GetEditLogCmd(MContext ctx, GLVoucherModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MNumber))
			{
				return null;
			}
			return base.GetLogCmd(TemplateEdit, ctx, model.MItemID, false, model.MNumber, model.MAttachments);
		}
	}
}
