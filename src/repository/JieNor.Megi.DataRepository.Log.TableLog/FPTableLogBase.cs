using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Linq;

namespace JieNor.Megi.DataRepository.Log.TableLog
{
	public class FPTableLogBase : BizLogBase, IFPTableLog
	{
		protected virtual OptLogTemplate TemplateCreate => OptLogTemplate.Sale_FaPiao_Table_Created;

		protected virtual OptLogTemplate TemplateEdit => OptLogTemplate.Sale_FaPiao_Table_Edited;

		protected virtual OptLogTemplate TemplateDeleted => OptLogTemplate.Sale_FaPiao_Table_Deleted;

		protected virtual OptLogTemplate TemplateManualReconcile => OptLogTemplate.FaPiao_ManualReconcie;

		protected virtual OptLogTemplate TemplateRemoveReconcile => OptLogTemplate.Fapiao_RemoveReconcile;

		public void CreateLog(MContext ctx, FPTableViewModel model)
		{
			base.AddLog(TemplateCreate, ctx, model.MItemID, model.MNumber.ToString().ToFixLengthString(4, "0"), FaPiaoType(ctx, model), model.MBizDate, model.MExplanation, model.MTaxAmount, model.MAjustAmount);
		}

		public void EditLog(MContext ctx, FPTableViewModel model)
		{
			base.AddLog(TemplateEdit, ctx, model.MItemID, model.MNumber.ToString().ToFixLengthString(4, "0"), FaPiaoType(ctx, model), model.MBizDate, model.MExplanation, model.MTaxAmount, model.MAjustAmount);
		}

		public void DeleteLog(MContext ctx, FPTableViewModel model)
		{
			base.AddLog(TemplateDeleted, ctx, model.MItemID, model.MNumber.ToString().ToFixLengthString(4, "0"), null, model.MBizDate, model.MExplanation, model.MTaxAmount, model.MAjustAmount);
		}

		public string FaPiaoType(MContext ctx, FPTableViewModel model)
		{
			string empty = string.Empty;
			if (model.MType == 0)
			{
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "VATinvoice", "增值税普通发票");
			}
			return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "VATspecialinvoice", "增值税专用发票");
		}

		public CommandInfo GetDeleteLogCmd(MContext ctx, FPTableViewModel model)
		{
			return base.GetLogCmd(TemplateDeleted, ctx, model.MItemID, true, model.MItemID, model.MNumber.ToString().ToFixLengthString(4, "0"), null, model.MBizDate, model.MExplanation, model.MTaxAmount, model.MAjustAmount);
		}

		public CommandInfo GetCreateLogCmd(MContext ctx, FPTableViewModel model)
		{
			return base.GetLogCmd(TemplateCreate, ctx, model.MItemID, true, model.MNumber.ToString().ToFixLengthString(4, "0"), FaPiaoType(ctx, model), model.MBizDate, model.MExplanation, model.MTaxAmount, model.MAjustAmount);
		}

		public CommandInfo GetEditLogCmd(MContext ctx, FPTableViewModel model)
		{
			return base.GetLogCmd(TemplateEdit, ctx, model.MItemID, true, model.MNumber.ToString().ToFixLengthString(4, "0"), FaPiaoType(ctx, model), model.MBizDate, model.MExplanation, model.MTaxAmount, model.MAjustAmount);
		}

		public CommandInfo GetManualReconcileLogCmd(MContext ctx, FPFapiaoReconcileModel model)
		{
			return base.GetLogCmd(TemplateManualReconcile, ctx, model.MTable.MItemID, true, "[" + string.Join(",", from x in model.MFapiaoList
			select x.MNumber) + "]");
		}

		public CommandInfo GetRemoveReconcileLogCmd(MContext ctx, FPFapiaoReconcileModel model)
		{
			return base.GetLogCmd(TemplateRemoveReconcile, ctx, model.MTable.MItemID, false, "[" + string.Join(",", from x in model.MFapiaoList
			select x.MNumber) + "]");
		}
	}
}
