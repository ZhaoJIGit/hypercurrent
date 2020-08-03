using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System;

namespace JieNor.Megi.DataRepository.Log
{
	public interface IIVInvoiceLog
	{
		void CreateLog(MContext ctx, IVInvoiceModel model);

		void EditLog(MContext ctx, IVInvoiceModel model);

		void SubmitForApprovalLog(MContext ctx, IVInvoiceModel model);

		void DeleteLog(MContext ctx, IVInvoiceModel model);

		void ApproveLog(MContext ctx, IVInvoiceModel model);

		void UnApproveLog(MContext ctx, IVInvoiceModel model);

		void PaidLog(MContext ctx, IVInvoiceModel model, DateTime paidDate, decimal paidAmtFor);

		void AddNoteLog(MContext ctx, IVInvoiceModel model);

		void UpdateExpectedDateLog(MContext ctx, IVInvoiceModel model);

		void AddAccountLog(MContext ctx, IVInvoiceModel model);

		CommandInfo GetCreateLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetEditLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetSubmitForApprovalLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetDeleteLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetApproveLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetUnApproveLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetPaidLogCmd(MContext ctx, IVInvoiceModel model, DateTime paidDate, decimal paidAmtFor);

		CommandInfo GetVerificationLogCmd(MContext ctx, IVInvoiceModel model, decimal amtFor);

		CommandInfo GetDeleteVerificationLogCmd(MContext ctx, IVInvoiceModel model, decimal amtFor);

		CommandInfo GetAddNoteLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetUpdateExpectedDateLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetEditAccountLogCmd(MContext ctx, IVInvoiceModel model);

		CommandInfo GetGenrateVoucherLogCmd(MContext ctx, IVInvoiceModel model);
	}
}
