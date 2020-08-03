using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.Context;
using System;

namespace JieNor.Megi.DataRepository.Log
{
	public class IVExpenseLogBase : BizLogBase
	{
		protected virtual OptLogTemplate TemplateCreate => OptLogTemplate.ExpenseClaims_Created;

		protected virtual OptLogTemplate TemplateEdit => OptLogTemplate.ExpenseClaims_Edited;

		protected virtual OptLogTemplate TemplateSubmitForApproval => OptLogTemplate.ExpenseClaims_Submit_For_Approval;

		protected virtual OptLogTemplate TemplateApprove => OptLogTemplate.ExpenseClaims_Approved;

		protected virtual OptLogTemplate TemplateUnApprove => OptLogTemplate.ExpenseClaims_Reverse_Approved;

		protected virtual OptLogTemplate TemplateDelete => OptLogTemplate.ExpenseClaims_Deleted;

		protected virtual OptLogTemplate TemplatePartiallyPaid => OptLogTemplate.ExpenseClaims_PartiallyPaid;

		protected virtual OptLogTemplate TemplatePaid => OptLogTemplate.ExpenseClaims_Paid;

		protected virtual OptLogTemplate TemplateVerification => OptLogTemplate.ExpenseClaims_Credit;

		protected virtual OptLogTemplate TemplateVerificationDelete => OptLogTemplate.ExpenseClaims_Credit_Delete;

		protected virtual OptLogTemplate TemplateEditAccount => OptLogTemplate.ExpenseClaims_Edit_Account;

		protected virtual OptLogTemplate TemplateGenerateVoucher => OptLogTemplate.ExpenseClaims_Generate_Voucher;

		protected string GetReferenceData(IVExpenseModel model)
		{
			return model.MReference;
		}

		public void CreateLog(MContext ctx, IVExpenseModel model)
		{
			base.AddLog(TemplateCreate, ctx, model.MID, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void EditLog(MContext ctx, IVExpenseModel model)
		{
			base.AddLog(TemplateEdit, ctx, model.MID, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void SubmitForApprovalLog(MContext ctx, IVExpenseModel model)
		{
			base.AddLog(TemplateSubmitForApproval, ctx, model.MID, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void DeleteLog(MContext ctx, IVExpenseModel model)
		{
			throw new NotImplementedException();
		}

		public void ApproveLog(MContext ctx, IVExpenseModel model)
		{
			base.AddLog(TemplateApprove, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void UnApproveLog(MContext ctx, IVExpenseModel model)
		{
			base.AddLog(TemplateUnApprove, ctx, model.MID, model.MNumber, model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public void PaidLog(MContext ctx, IVExpenseModel model, DateTime paidDate, decimal paidAmtFor)
		{
			throw new NotImplementedException();
		}

		public void AddNoteLog(MContext ctx, IVExpenseModel model)
		{
			throw new NotImplementedException();
		}

		public void UpdateExpectedDateLog(MContext ctx, IVExpenseModel model)
		{
			throw new NotImplementedException();
		}

		public void AddAccountLog(MContext ctx, IVExpenseModel model)
		{
			base.AddLog(TemplateEditAccount, ctx, model.MID, true, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetCreateLogCmd(MContext ctx, IVExpenseModel model)
		{
			return base.GetLogCmd(TemplateCreate, ctx, model.MID, false, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetEditLogCmd(MContext ctx, IVExpenseModel model)
		{
			return base.GetLogCmd(TemplateEdit, ctx, model.MID, false, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetSubmitForApprovalLogCmd(MContext ctx, IVExpenseModel model)
		{
			return base.GetLogCmd(TemplateSubmitForApproval, ctx, model.MID, true, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetDeleteLogCmd(MContext ctx, IVExpenseModel model)
		{
			throw new NotImplementedException();
		}

		public CommandInfo GetApproveLogCmd(MContext ctx, IVExpenseModel model)
		{
			return base.GetLogCmd(TemplateApprove, ctx, model.MID, true, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetUnApproveLogCmd(MContext ctx, IVExpenseModel model)
		{
			return base.GetLogCmd(TemplateUnApprove, ctx, model.MID, false, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetPaidLogCmd(MContext ctx, IVExpenseModel model, DateTime paidDate, decimal paidAmtFor)
		{
			OptLogTemplate optLogTemplate = OptLogTemplate.None;
			decimal num = Math.Abs(model.MTaxTotalAmtFor) - Math.Abs(model.MVerificationAmt) - Math.Abs(paidAmtFor);
			optLogTemplate = ((!(num <= decimal.Zero)) ? TemplatePartiallyPaid : TemplatePaid);
			return base.GetLogCmd(optLogTemplate, ctx, model.MID, false, model.MContactID, paidDate, Math.Abs(paidAmtFor).To2Decimal(), num);
		}

		public CommandInfo GetVerificationLogCmd(MContext ctx, IVExpenseModel model, decimal amtFor)
		{
			return base.GetLogCmd(TemplateVerification, ctx, model.MID, false, ctx.DateNow, Math.Abs(amtFor).To2Decimal());
		}

		public CommandInfo GetDeleteVerificationLogCmd(MContext ctx, IVExpenseModel model, decimal amtFor)
		{
			return base.GetLogCmd(TemplateVerificationDelete, ctx, model.MID, false, ctx.DateNow, Math.Abs(amtFor).To2Decimal());
		}

		public CommandInfo GetAddNoteLogCmd(MContext ctx, IVExpenseModel model)
		{
			throw new NotImplementedException();
		}

		public CommandInfo GetUpdateExpectedDateLogCmd(MContext ctx, IVExpenseModel model)
		{
			throw new NotImplementedException();
		}

		public CommandInfo GetEditAccountLogCmd(MContext ctx, IVExpenseModel model)
		{
			return base.GetLogCmd(TemplateEditAccount, ctx, model.MID, false, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}

		public CommandInfo GetGenrateVoucherLogCmd(MContext ctx, IVExpenseModel model)
		{
			return base.GetLogCmd(TemplateGenerateVoucher, ctx, model.MID, true, GetReferenceData(model), model.MContactID, Math.Abs(model.MTaxTotalAmtFor));
		}
	}
}
