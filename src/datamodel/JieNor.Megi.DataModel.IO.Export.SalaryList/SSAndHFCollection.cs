using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.SalaryList
{
	public class SSAndHFCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(SSAndHFModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "SSAndHFModels";
		}
	}
}
