using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Invoice
{
	public class InvoiceTaxCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(InvoiceTaxModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "InvoiceTaxes";
		}
	}
}
