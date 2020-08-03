using JieNor.Megi.DataModel.GL;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[Serializable]
	public class BDInitDocumentFilterModel
	{
		[DataMember]
		public string MCurrentAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public List<int> MTypeList
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MCheckGroupValueModel
		{
			get;
			set;
		}
	}
}
