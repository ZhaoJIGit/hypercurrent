using JieNor.Megi.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTDepreciationDetailFilterModel : ReportFilterBase
	{
		[DataMember]
		public string MStartPeroid
		{
			get;
			set;
		}

		[DataMember]
		public string MEndPeroid
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeCheckType
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> CheckTypeValueList
		{
			get;
			set;
		}

		[DataMember]
		public string CheckGroupValueId
		{
			get;
			set;
		}
	}
}
