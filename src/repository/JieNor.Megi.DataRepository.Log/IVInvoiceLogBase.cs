using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System;

namespace JieNor.Megi.DataRepository.Log
{
	public class IVInvoiceLogBase : BizLogBase, IIVInvoiceLog
	{
		protected virtual OptLogTemplate TemplateCreate => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateEdit => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateSubmitForApproval => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateApprove => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateUnApprove => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateDelete => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplatePartiallyPaid => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplatePaid => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateVerification => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateVerificationDelete => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateEditAccount => OptLogTemplate.None;

		protected virtual OptLogTemplate TemplateGenerateVoucher => OptLogTemplate.None;

		protected string GetReferenceData(IVInvoiceModel model)
		{
			if (model.MType == "Invoice_Sale")
			{
				return model.MNumber;
			}
			return model.MReference;
		}

		public void CreateLog(MContext ctx, IVInvoiceModel model)
		{
			base.AddLog(TemplateCreate, ctx, model.MID, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void EditLog(MContext ctx, IVInvoiceModel model)
		{
			base.AddLog(TemplateEdit, ctx, model.MID, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void SubmitForApprovalLog(MContext ctx, IVInvoiceModel model)
		{
			base.AddLog(TemplateSubmitForApproval, ctx, model.MID, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void DeleteLog(MContext ctx, IVInvoiceModel model)
		{
			throw new NotImplementedException();
		}

		public void ApproveLog(MContext ctx, IVInvoiceModel model)
		{
			base.AddLog(TemplateApprove, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void UnApproveLog(MContext ctx, IVInvoiceModel model)
		{
			base.AddLog(TemplateUnApprove, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void PaidLog(MContext ctx, IVInvoiceModel model, DateTime paidDate, decimal paidAmtFor)
		{
			throw new NotImplementedException();
		}

		public void AddNoteLog(MContext ctx, IVInvoiceModel model)
		{
			throw new NotImplementedException();
		}

		public void UpdateExpectedDateLog(MContext ctx, IVInvoiceModel model)
		{
			throw new NotImplementedException();
		}

		public void AddAccountLog(MContext ctx, IVInvoiceModel model)
		{
			base.AddLog(TemplateEditAccount, ctx, model.MID, true, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetCreateLogCmd(MContext ctx, IVInvoiceModel model)
		{
			return base.GetLogCmd(TemplateCreate, ctx, model.MID, false, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetEditLogCmd(MContext ctx, IVInvoiceModel model)
		{
			return base.GetLogCmd(TemplateEdit, ctx, model.MID, false, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetSubmitForApprovalLogCmd(MContext ctx, IVInvoiceModel model)
		{
			return base.GetLogCmd(TemplateSubmitForApproval, ctx, model.MID, true, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetDeleteLogCmd(MContext ctx, IVInvoiceModel model)
		{
			throw new NotImplementedException();
		}

		public CommandInfo GetApproveLogCmd(MContext ctx, IVInvoiceModel model)
		{
			return base.GetLogCmd(TemplateApprove, ctx, model.MID, true, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetUnApproveLogCmd(MContext ctx, IVInvoiceModel model)
		{
			return base.GetLogCmd(TemplateUnApprove, ctx, model.MID, false, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetPaidLogCmd(MContext ctx, IVInvoiceModel model, DateTime paidDate, decimal paidAmtFor)
		{
			OptLogTemplate optLogTemplate = OptLogTemplate.None;
			decimal num = Math.Abs(model.MTaxTotalAmtFor) - Math.Abs(model.MVerificationAmt) - Math.Abs(paidAmtFor);
			optLogTemplate = ((!(num <= decimal.Zero)) ? TemplatePartiallyPaid : TemplatePaid);
			return base.GetLogCmd(optLogTemplate, ctx, model.MID, false, model.MContactID, paidDate, Math.Abs(paidAmtFor).To2Decimal(), num);
		}

		public CommandInfo GetVerificationLogCmd(MContext ctx, IVInvoiceModel model, decimal amtFor)
		{
			return base.GetLogCmd(TemplateVerification, ctx, model.MID, false, ctx.DateNow, Math.Abs(amtFor).To2Decimal());
		}

		public CommandInfo GetDeleteVerificationLogCmd(MContext ctx, IVInvoiceModel model, decimal amtFor)
		{
			return base.GetLogCmd(TemplateVerificationDelete, ctx, model.MID, false, ctx.DateNow, Math.Abs(amtFor).To2Decimal());
		}

		public CommandInfo GetAddNoteLogCmd(MContext ctx, IVInvoiceModel model)
		{
			throw new NotImplementedException();
		}

		public CommandInfo GetUpdateExpectedDateLogCmd(MContext ctx, IVInvoiceModel model)
		{
			throw new NotImplementedException();
		}

		public CommandInfo GetEditAccountLogCmd(MContext ctx, IVInvoiceModel model)
		{
			return base.GetLogCmd(TemplateEditAccount, ctx, model.MID, false, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetGenrateVoucherLogCmd(MContext ctx, IVInvoiceModel model)
		{
			return base.GetLogCmd(TemplateGenerateVoucher, ctx, model.MID, true, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}
	}
}
