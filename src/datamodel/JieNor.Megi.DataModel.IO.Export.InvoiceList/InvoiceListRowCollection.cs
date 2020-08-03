using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.InvoiceList
{
	public class InvoiceListRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(InvoiceListRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "InvoiceListRows";
		}
	}
}
