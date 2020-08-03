using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTAccountBalanceFilterModel : GLReportBaseFilterModel
	{
		[DataMember]
		public string MBaseCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIncludeContact
		{
			get;
			set;
		}
	}
}
