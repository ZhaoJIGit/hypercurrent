using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Invoice
{
	public class InvoiceCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(InvoiceEntryCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(InvoiceEntryModel));
				}
				if (propertyDescriptor.PropertyType.Equals(typeof(InvoiceTaxCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(InvoiceTaxModel));
				}
				if (propertyDescriptor.PropertyType.Equals(typeof(InvoiceVerificationCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(InvoiceVerificationModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(InvoiceModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "Invoices";
		}
	}
}
