using JieNor.Megi.DataModel.IO.Import.PA;
using System.Collections.Generic;
using System.IO;

namespace JieNor.Megi.DataModel.IO.Import
{
	public class ImportTemplateModel
	{
		public string TemplateType
		{
			get;
			set;
		}

		public string LocaleID
		{
			get;
			set;
		}

		public List<string> RequiredColumnList
		{
			get;
			set;
		}

		public List<ImportRequiredModel> RequiredInfoList
		{
			get;
			set;
		}

		public Dictionary<string, string> ColumnList
		{
			get;
			set;
		}

		public Dictionary<string, string> AliasColumnList
		{
			get;
			set;
		}

		public List<ImportTemplateDataSource> TemplateDictionaryList
		{
			get;
			set;
		}

		public List<string> DsExtInfo
		{
			get;
			set;
		}

		public List<ImportColumnRegexModel> ColumnRegexList
		{
			get;
			set;
		}

		public Dictionary<string, string[]> ExampleDataList
		{
			get;
			set;
		}

		public List<string> AlignRightFieldList
		{
			get;
			set;
		}

		public Dictionary<string, int> ColumnWidthList
		{
			get;
			set;
		}

		public Dictionary<string, string> HeaderCommentList
		{
			get;
			set;
		}

		public List<TmplRowModel> TmplHeaderRows
		{
			get;
			set;
		}

		public ImportSalaryModel SalaryInfo
		{
			get;
			set;
		}

		public Stream TemplateStream
		{
			get;
			set;
		}

		public string TemplateName
		{
			get;
			set;
		}

		public List<IOTemplateConfigModel> FieldConfigList
		{
			get;
			set;
		}

		public List<IOImportHelperSheetModel> HelperSheetList
		{
			get;
			set;
		}

		public bool IsMainSheet
		{
			get;
			set;
		}
	}
}
