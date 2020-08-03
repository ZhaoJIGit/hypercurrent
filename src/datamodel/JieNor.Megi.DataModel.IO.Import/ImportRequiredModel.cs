using System.Collections.Generic;

namespace JieNor.Megi.DataModel.IO.Import
{
	public class ImportRequiredModel
	{
		public string DbField
		{
			get;
			set;
		}

		public List<int> HeaderColumnIndexList
		{
			get;
			set;
		}

		public string RequiredColumnIndex
		{
			get;
			set;
		}
	}
}
