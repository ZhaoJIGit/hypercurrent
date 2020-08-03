using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDLayoutModel
	{
		[DataMember]
		public List<BDLayoutListModel> Left
		{
			get;
			set;
		}

		[DataMember]
		public List<BDLayoutListModel> Right
		{
			get;
			set;
		}
	}
}
