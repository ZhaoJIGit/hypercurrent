using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Account
{
	public class AccountCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(AccountRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(AccountRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(AccountModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "Accounts";
		}
	}
}
