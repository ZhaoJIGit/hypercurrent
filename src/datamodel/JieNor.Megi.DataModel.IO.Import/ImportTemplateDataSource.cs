using System.Collections.Generic;

namespace JieNor.Megi.DataModel.IO.Import
{
	public class ImportTemplateDataSource
	{
		public ImportTemplateColumnType FieldType
		{
			get;
			set;
		}

		public List<string> FieldList
		{
			get;
			set;
		}

		public bool IsCombobox
		{
			get;
			set;
		}

		public bool IsAllowManualInput
		{
			get;
			set;
		}

		public List<ImportDataSourceInfo> DataSourceList
		{
			get;
			set;
		}

		public ImportTemplateDataSource()
		{
		}

		public ImportTemplateDataSource(bool isCombobox)
		{
			IsCombobox = isCombobox;
		}
	}
}
