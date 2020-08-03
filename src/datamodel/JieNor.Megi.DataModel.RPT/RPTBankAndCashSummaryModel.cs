using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTBankAndCashSummaryModel
	{
		[DataMember]
		public string MBankAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MBankAccountID
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
		public decimal IniBlance
		{
			get;
			set;
		}

		[DataMember]
		public decimal OpeningBlance
		{
			get;
			set;
		}

		[DataMember]
		public decimal FXGain
		{
			get
			{
				if (MCyRate == decimal.Zero)
				{
					return decimal.Zero;
				}
				return TReceived - TSpent - (TReceivedFor - TSpentFor) / MCyRate;
			}
		}

		[DataMember]
		public decimal ClosingBlance
		{
			get
			{
				return IniBlance + OpeningBlance + TReceived - TSpent;
			}
		}

		[DataMember]
		public string MOrgName
		{
			get;
			set;
		}

		[DataMember]
		public string MTableType
		{
			get;
			set;
		}

		[DataMember]
		public decimal TSpent
		{
			get;
			set;
		}

		[DataMember]
		public decimal TReceived
		{
			get;
			set;
		}

		[DataMember]
		public decimal TSpentFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal TReceivedFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCyRate
		{
			get;
			set;
		}

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}
	}
}
