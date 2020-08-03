using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class BalanceBizReportRowModel : BizReportRowModel
	{
		public string MAccountID
		{
			get;
			set;
		}

		public string MCheckGroupValueID
		{
			get;
			set;
		}

		public decimal CurrentTotalAmount
		{
			get;
			set;
		}
	}
}
