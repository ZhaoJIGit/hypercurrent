using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Item
{
	public class ItemCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(ItemRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(ItemRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(ItemModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "Items";
		}
	}
}
