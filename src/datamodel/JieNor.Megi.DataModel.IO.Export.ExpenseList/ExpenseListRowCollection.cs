using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.ExpenseList
{
	public class ExpenseListRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(ExpenseListRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "ExpenseListRows";
		}
	}
}
