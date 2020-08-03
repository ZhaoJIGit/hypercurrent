using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Statement
{
	public class StatementGroupRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(StatementGroupRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "StatementGroupRows";
		}
	}
}
