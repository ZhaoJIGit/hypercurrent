using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTGeneralLedgerFilterModel : GLReportBaseFilterModel
	{
		[DataMember]
		public string MBaseCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public BizReportType ReportType
		{
			get;
			set;
		}
	}
}
