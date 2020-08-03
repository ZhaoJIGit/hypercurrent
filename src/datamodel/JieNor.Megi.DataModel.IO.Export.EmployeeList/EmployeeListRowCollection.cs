using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.EmployeeList
{
	public class EmployeeListRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(EmployeeListRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "EmployeeListRows";
		}
	}
}
