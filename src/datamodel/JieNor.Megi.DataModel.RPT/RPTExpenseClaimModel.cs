using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTExpenseClaimModel
	{
		[DataMember]
		public string StatisticsFieldId
		{
			get;
			set;
		}

		[DataMember]
		public string StatisticsField
		{
			get;
			set;
		}

		[DataMember]
		public string StatisticsType
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MBizDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public string ExpenseParentId
		{
			get;
			set;
		}
	}
}
