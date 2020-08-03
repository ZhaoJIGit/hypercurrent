using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Account
{
	public class OpeningBalanceRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(OpeningBalanceRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "OpeningBalanceRows";
		}
	}
}
