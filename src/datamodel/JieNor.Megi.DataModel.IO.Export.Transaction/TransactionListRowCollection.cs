using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Transaction
{
	public class TransactionListRowCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(TransactionListRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "TransactionListRows";
		}
	}
}
