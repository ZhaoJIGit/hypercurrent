using JieNor.Megi.Core.Log;

namespace JieNor.Megi.DataRepository.Log
{
	public class IVInvoicePurchaseLog : IVInvoiceLogBase
	{
		protected override OptLogTemplate TemplateCreate => OptLogTemplate.Bill_Created;

		protected override OptLogTemplate TemplateEdit => OptLogTemplate.Bill_Edited;

		protected override OptLogTemplate TemplateSubmitForApproval => OptLogTemplate.Bill_Submit_For_Approval;

		protected override OptLogTemplate TemplateApprove => OptLogTemplate.Bill_Approved;

		protected override OptLogTemplate TemplateUnApprove => OptLogTemplate.Bill_Reverse_Approved;

		protected override OptLogTemplate TemplateDelete => OptLogTemplate.Bill_Deleted;

		protected override OptLogTemplate TemplatePartiallyPaid => OptLogTemplate.Bill_PartiallyPaid;

		protected override OptLogTemplate TemplatePaid => OptLogTemplate.Bill_Paid;

		protected override OptLogTemplate TemplateVerification => OptLogTemplate.Sale_Invoice_Credit;

		protected override OptLogTemplate TemplateVerificationDelete => OptLogTemplate.Sale_Invoice_Credit_Delete;

		protected override OptLogTemplate TemplateEditAccount => OptLogTemplate.Bill_Edit_Account;

		protected override OptLogTemplate TemplateGenerateVoucher => OptLogTemplate.Bill_Generate_Voucher;
	}
}
