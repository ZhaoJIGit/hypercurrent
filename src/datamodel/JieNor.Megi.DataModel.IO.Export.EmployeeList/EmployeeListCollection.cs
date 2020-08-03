using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.EmployeeList
{
	public class EmployeeListCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(EmployeeListRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(EmployeeListRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(EmployeeListModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "EmployeeList";
		}
	}
}
