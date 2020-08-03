using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.Log
{
	public class FPFapiaoLogBase : BizLogBase, IFPFapiaoLog
	{
		protected virtual OptLogTemplate TemplateCreate => OptLogTemplate.Sale_FaPiao_Created;

		protected virtual OptLogTemplate BatchUpdateFPStatus => OptLogTemplate.Fapiao_BatchUpdateFPStatus;

		protected virtual OptLogTemplate BatchUpdateFPVerifyType => OptLogTemplate.Fapiao_BatchUpdateFPVerifyType;

		protected virtual OptLogTemplate TemplateEdit => OptLogTemplate.Sale_FaPiao_Edited;

		protected virtual OptLogTemplate TemplateDeleted => OptLogTemplate.Sale_FaPiao_Deleted;

		public void CreateLog(MContext ctx, FPFapiaoModel model)
		{
			base.AddLog(TemplateCreate, ctx, model.MID, model.MNumber, model.MBizDate, model.MExplanation);
		}

		public void EditLog(MContext ctx, FPFapiaoModel model)
		{
			base.AddLog(TemplateEdit, ctx, model.MID, model.MNumber, model.MBizDate, model.MExplanation);
		}

		public List<CommandInfo> BatchUpdateFPStatusLog(MContext ctx, List<FPFapiaoModel> fpList, int status)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (FPFapiaoModel fp in fpList)
			{
				list.Add(base.GetLogCmd(BatchUpdateFPStatus, ctx, fp.MID, false, fp.MStatus, status));
			}
			return list;
		}

		public List<CommandInfo> BatchUpdateFPVerifyTypeLog(MContext ctx, List<FPFapiaoModel> entityList, List<FPFapiaoModel> modelList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (FPFapiaoModel model in modelList)
			{
				FPFapiaoModel fPFapiaoModel = entityList.FirstOrDefault((FPFapiaoModel a) => a.MID == model.MID);
				if (fPFapiaoModel != null)
				{
					list.Add(base.GetLogCmd(BatchUpdateFPVerifyType, ctx, model.MID, false, fPFapiaoModel.MVerifyType, fPFapiaoModel.MVerifyDate, model.MVerifyType, model.MVerifyDate));
				}
			}
			return list;
		}

		public void DeleteLog(MContext ctx, FPFapiaoModel model)
		{
			base.AddLog(TemplateDeleted, ctx, model.MID, model.MNumber, model.MBizDate, model.MExplanation);
		}

		public CommandInfo GetCreateLogCmd(MContext ctx, FPFapiaoModel model)
		{
			return base.GetLogCmd(TemplateCreate, ctx, model.MID, true, model.MNumber, model.MBizDate, model.MExplanation);
		}

		public CommandInfo GetEditLogCmd(MContext ctx, FPFapiaoModel model)
		{
			return base.GetLogCmd(TemplateEdit, ctx, model.MID, true, model.MNumber, model.MBizDate, model.MExplanation);
		}
	}
}
