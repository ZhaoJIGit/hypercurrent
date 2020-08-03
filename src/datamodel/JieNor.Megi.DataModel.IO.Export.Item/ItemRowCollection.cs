using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Item
{
	public class ItemRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(ItemRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "ItemRows";
		}
	}
}
