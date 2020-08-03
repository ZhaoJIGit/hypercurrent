using JieNor.Megi.Core.Log;

namespace JieNor.Megi.DataRepository.Log
{
	public class IVInvoiceSaleLog : IVInvoiceLogBase
	{
		protected override OptLogTemplate TemplateCreate => OptLogTemplate.Sale_Invoice_Created;

		protected override OptLogTemplate TemplateEdit => OptLogTemplate.Sale_Invoice_Edited;

		protected override OptLogTemplate TemplateSubmitForApproval => OptLogTemplate.Sale_Invoice_Submit_For_Approval;

		protected override OptLogTemplate TemplateApprove => OptLogTemplate.Sale_Invoice_Approved;

		protected override OptLogTemplate TemplateUnApprove => OptLogTemplate.Sale_Invoice_Reverse_Approved;

		protected override OptLogTemplate TemplateDelete => OptLogTemplate.Sale_Invoice_Deleted;

		protected override OptLogTemplate TemplatePartiallyPaid => OptLogTemplate.Sale_Invoice_PartiallyPaid;

		protected override OptLogTemplate TemplatePaid => OptLogTemplate.Sale_Invoice_Paid;

		protected override OptLogTemplate TemplateVerification => OptLogTemplate.Sale_Invoice_Credit;

		protected override OptLogTemplate TemplateVerificationDelete => OptLogTemplate.Sale_Invoice_Credit_Delete;

		protected override OptLogTemplate TemplateEditAccount => OptLogTemplate.Sale_Invoice_Edit_Account;

		protected override OptLogTemplate TemplateGenerateVoucher => OptLogTemplate.Sale_Invoice_Generate_Voucher;
	}
}
