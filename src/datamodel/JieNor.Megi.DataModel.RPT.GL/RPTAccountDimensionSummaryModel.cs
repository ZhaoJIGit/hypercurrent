using JieNor.Megi.DataModel.GL;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT.GL
{
	[DataContract]
	public class RPTAccountDimensionSummaryModel
	{
		[DataMember]
		public string AccountId
		{
			get;
			set;
		}

		[DataMember]
		public string AccountName
		{
			get;
			set;
		}

		[DataMember]
		public string AccountNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public List<string> CheckTypeValueList
		{
			get;
			set;
		}

		public string CheckTypeValueString
		{
			get
			{
				if (CheckTypeValueList != null && CheckTypeValueList.Count() > 0)
				{
					return string.Join(",", CheckTypeValueList);
				}
				return "";
			}
		}

		[DataMember]
		public decimal MBeginDebitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginCreditBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebitFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCreditFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdDebitFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdCreditFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndDebitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndCreditBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public int MYearPeriod
		{
			get;
			set;
		}

		public List<GLBalanceModel> BalanceList
		{
			get;
			set;
		}

		public RPTAccountDimensionSummaryModel()
		{
			CheckTypeValueList = new List<string>();
		}
	}
}
