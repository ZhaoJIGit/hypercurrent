using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Report
{
	public class ReportGroupCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(BaseReportRowModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "ReportGroups";
		}
	}
}
