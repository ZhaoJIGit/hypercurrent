using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Invoice
{
	public class InvoiceVerificationCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(InvoiceVerificationModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "InvoiceVerifications";
		}
	}
}
