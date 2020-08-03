using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Statement
{
	public class StatementGroupCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(StatementGroupRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(StatementGroupRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(StatementGroupModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "StatementGroups";
		}
	}
}
