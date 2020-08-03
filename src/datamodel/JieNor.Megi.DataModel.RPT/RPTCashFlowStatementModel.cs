using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTCashFlowStatementModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MCode
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYearAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPeriodAmt
		{
			get;
			set;
		}
	}
}
