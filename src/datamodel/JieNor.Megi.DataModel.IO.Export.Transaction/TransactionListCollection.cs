using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Transaction
{
	public class TransactionListCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(TransactionListRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(TransactionListRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(TransactionListModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "TransactionList";
		}
	}
}
