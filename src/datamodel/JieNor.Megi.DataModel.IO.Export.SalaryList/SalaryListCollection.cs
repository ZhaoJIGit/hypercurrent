using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.SalaryList
{
	public class SalaryListCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(SalaryItemCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(SalaryItemModel));
				}
				if (propertyDescriptor.PropertyType.Equals(typeof(SSAndHFCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(SSAndHFModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(SalaryListModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "SalaryList";
		}
	}
}
