using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Expense
{
	public class ExpenseCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(ExpenseEntryCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(ExpenseEntryModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(ExpenseModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "Expenses";
		}
	}
}
