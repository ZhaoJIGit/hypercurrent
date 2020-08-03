using System.Collections.Generic;

namespace JieNor.Megi.DataModel.IO.Import
{
	public class ImportTemplateDictionary
	{
		public ImportTemplateColumnType Type
		{
			get;
			set;
		}

		public List<string> RelatedColumnList
		{
			get;
			set;
		}

		public string Key
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}
	}
}
