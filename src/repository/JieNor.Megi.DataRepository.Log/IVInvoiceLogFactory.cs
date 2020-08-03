namespace JieNor.Megi.DataRepository.Log
{
	public class IVInvoiceLogFactory
	{
		public static IIVInvoiceLog CreateInstance(string bizType)
		{
			switch (bizType)
			{
			case "Invoice_Sale":
				return new IVInvoiceSaleLog();
			case "Invoice_Sale_Red":
				return new IVInvoiceSaleRedLog();
			case "Invoice_Purchase":
				return new IVInvoicePurchaseLog();
			case "Invoice_Purchase_Red":
				return new IVInvoicePurchaseRedLog();
			default:
				return new IVInvoiceSaleLog();
			}
		}
	}
}
