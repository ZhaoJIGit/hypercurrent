using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Account
{
	public class AccountRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(AccountRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "AccountRows";
		}
	}
}
