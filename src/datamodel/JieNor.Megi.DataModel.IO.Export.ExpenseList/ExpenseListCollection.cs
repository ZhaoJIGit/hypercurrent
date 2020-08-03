using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.ExpenseList
{
	public class ExpenseListCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(ExpenseListRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(ExpenseListRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(ExpenseListModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "ExpenseList";
		}
	}
}
