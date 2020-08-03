using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.VoucherList
{
	public class VoucherListCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(VoucherListRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(VoucherListRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(VoucherListModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "VoucherList";
		}
	}
}
