using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT.GL
{
	public class RPTAccountDemensionSummaryFilterModel : GLReportBaseFilterModel
	{
		[DataMember]
		public string BaseCurrencyId
		{
			get;
			set;
		}

		[DataMember]
		public bool IsSubtotalByAccountDemension
		{
			get;
			set;
		}

		[DataMember]
		public bool IsSubtotalByAccount
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCrossPeriodSubtotal
		{
			get;
			set;
		}

		[DataMember]
		public bool IsShowDisabledAccount
		{
			get;
			set;
		}

		[DataMember]
		public string FilterSchemeId
		{
			get;
			set;
		}
	}
}
