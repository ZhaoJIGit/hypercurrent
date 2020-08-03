using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import
{
	[DataContract]
	public class IOImportHelperSheetModel
	{
		[DataMember]
		public string MSheetName
		{
			get;
			set;
		}

		[DataMember]
		public List<IOImportHelperColumnModel> MColumnDataList
		{
			get;
			set;
		}
	}
}
