using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Account
{
	public class OpeningBalanceCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(OpeningBalanceRowCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(OpeningBalanceRowModel));
				}
			}
			return TypeDescriptor.GetProperties(typeof(OpeningBalanceModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "OpeningBalances";
		}
	}
}
