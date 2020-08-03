using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT.GL
{
	[DataContract]
	public class RPTFilterSchemeModel : BDModel
	{
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MContent
		{
			get;
			set;
		}

		[DataMember]
		public int MReportType
		{
			get;
			set;
		}

		public RPTFilterSchemeModel()
			: base("t_rpt_scheme")
		{
		}
	}
}
