using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.ContactList
{
	public class ContactListCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(ContactListRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(ContactListRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(ContactListModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "ContactList";
		}
	}
}
