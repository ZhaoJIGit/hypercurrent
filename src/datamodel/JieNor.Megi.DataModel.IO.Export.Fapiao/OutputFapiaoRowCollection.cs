using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Fapiao
{
	public class OutputFapiaoRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(ContactListRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "OutputFapiaoRows";
		}
	}
}
