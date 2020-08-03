using JieNor.Megi.Core.Log;

namespace JieNor.Megi.DataRepository.Log
{
	public class IVInvoiceSaleRedLog : IVInvoiceLogBase
	{
		protected override OptLogTemplate TemplateCreate => OptLogTemplate.Sale_Credit_Note_Created;

		protected override OptLogTemplate TemplateEdit => OptLogTemplate.Sale_Credit_Note_Edited;

		protected override OptLogTemplate TemplateSubmitForApproval => OptLogTemplate.Sale_Credit_Note_Submit_For_Approval;

		protected override OptLogTemplate TemplateApprove => OptLogTemplate.Sale_Credit_Note_Approved;

		protected override OptLogTemplate TemplateUnApprove => OptLogTemplate.Credit_Note_Reverse_Approved;

		protected override OptLogTemplate TemplateDelete => OptLogTemplate.Sale_Credit_Note_Deleted;

		protected override OptLogTemplate TemplatePartiallyPaid => OptLogTemplate.Sale_Credit_Note_PartiallyPaid;

		protected override OptLogTemplate TemplatePaid => OptLogTemplate.Sale_Credit_Note_Paid;

		protected override OptLogTemplate TemplateVerification => OptLogTemplate.Credit_Note_Apply;

		protected override OptLogTemplate TemplateVerificationDelete => OptLogTemplate.Credit_Note_Apply_Delete;

		protected override OptLogTemplate TemplateEditAccount => OptLogTemplate.Sale_Credit_Note_Edit_Account;

		protected override OptLogTemplate TemplateGenerateVoucher => OptLogTemplate.Sale_Credit_Note_Generate_Voucher;
	}
}
