using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Invoice
{
	public class InvoiceEntryCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(InvoiceEntryModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "InvoiceEntries";
		}
	}
}
