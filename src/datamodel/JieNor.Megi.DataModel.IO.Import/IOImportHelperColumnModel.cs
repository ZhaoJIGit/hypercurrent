using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import
{
	[DataContract]
	public class IOImportHelperColumnModel
	{
		[DataMember]
		public string MComment
		{
			get;
			set;
		}

		[DataMember]
		public string MTitle
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MDataList
		{
			get;
			set;
		}
	}
}
