using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Report
{
	public class ReportCombineCollection<T> : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (listAccessors != null && listAccessors.Length != 0)
			{
				PropertyDescriptor propertyDescriptor = listAccessors[listAccessors.Length - 1];
				if (propertyDescriptor.PropertyType.Equals(typeof(ReportHeaderCollection)) || propertyDescriptor.PropertyType.Equals(typeof(ReportGroupCollection)) || propertyDescriptor.PropertyType.Equals(typeof(ReportItemCollection)) || propertyDescriptor.PropertyType.Equals(typeof(ReportSubTotalCollection)) || propertyDescriptor.PropertyType.Equals(typeof(ReportTotalCollection)))
				{
					return TypeDescriptor.GetProperties(typeof(T));
				}
			}
			return TypeDescriptor.GetProperties(typeof(ReportCombineModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "ReportCombines";
		}
	}
}
