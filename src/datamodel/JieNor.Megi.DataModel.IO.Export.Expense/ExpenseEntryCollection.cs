using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Expense
{
	public class ExpenseEntryCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(ExpenseEntryModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "ExpenseEntrys";
		}
	}
}
