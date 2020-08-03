using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Fapiao
{
	public class InputFapiaoCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(InputFapiaoListRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(FapiaoListRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(InputFapiaoModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "InputFapiao";
		}
	}
}
