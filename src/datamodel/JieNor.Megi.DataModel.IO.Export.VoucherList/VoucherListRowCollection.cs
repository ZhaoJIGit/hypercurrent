using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.VoucherList
{
	public class VoucherListRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(VoucherListRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "VoucherListRows";
		}
	}
}
