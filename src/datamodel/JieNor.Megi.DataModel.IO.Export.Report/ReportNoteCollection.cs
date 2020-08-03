using JieNor.Megi.Core;
using System.Collections;
using System.ComponentModel;

namespace JieNor.Megi.DataModel.IO.Export.Report
{
	public class ReportNoteCollection : ArrayList, ITypedList
	{
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return TypeDescriptor.GetProperties(typeof(BizReportNoteModel));
		}

		public string GetListName(PropertyDescriptor[] listAccessors)
		{
			return "ReportNotes";
		}
	}
}
