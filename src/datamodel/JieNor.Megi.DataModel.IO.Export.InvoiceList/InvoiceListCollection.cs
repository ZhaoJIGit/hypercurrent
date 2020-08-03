using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.InvoiceList
{
	public class InvoiceListCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(InvoiceListRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(InvoiceListRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(InvoiceListModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "InvoiceList";
		}
	}
}
