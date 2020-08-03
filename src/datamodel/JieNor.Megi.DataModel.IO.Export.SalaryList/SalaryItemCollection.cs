using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.SalaryList
{
	public class SalaryItemCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(SalaryItemModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "SalaryItems";
		}
	}
}
