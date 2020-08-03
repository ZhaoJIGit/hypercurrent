using JieNor.Megi.Core.Log;

namespace JieNor.Megi.DataRepository.Log
{
	public class IVInvoicePurchaseRedLog : IVInvoiceLogBase
	{
		protected override OptLogTemplate TemplateCreate => OptLogTemplate.Bill_Credit_Note_Created;

		protected override OptLogTemplate TemplateEdit => OptLogTemplate.Bill_Credit_Note_Edited;

		protected override OptLogTemplate TemplateSubmitForApproval => OptLogTemplate.Bill_Credit_Note_Submit_For_Approval;

		protected override OptLogTemplate TemplateApprove => OptLogTemplate.Bill_Credit_Note_Approved;

		protected override OptLogTemplate TemplateUnApprove => OptLogTemplate.Bill_Credit_Note_Reverse_Approved;

		protected override OptLogTemplate TemplateDelete => OptLogTemplate.Bill_Credit_Note_Deleted;

		protected override OptLogTemplate TemplatePartiallyPaid => OptLogTemplate.Bill_Credit_Note_PartiallyPaid;

		protected override OptLogTemplate TemplatePaid => OptLogTemplate.Bill_Credit_Note_Paid;

		protected override OptLogTemplate TemplateVerification => OptLogTemplate.Credit_Note_Apply;

		protected override OptLogTemplate TemplateVerificationDelete => OptLogTemplate.Credit_Note_Apply_Delete;

		protected override OptLogTemplate TemplateEditAccount => OptLogTemplate.Bill_Edit_Account;

		protected override OptLogTemplate TemplateGenerateVoucher => OptLogTemplate.Bill_Credit_Note_Generate_Voucher;
	}
}
