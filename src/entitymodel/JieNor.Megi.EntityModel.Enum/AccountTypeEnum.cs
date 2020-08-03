using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public class AccountTypeEnum
	{
		private string _tableID = "1";

		public string CurrentAsset
		{
			get
			{
				return _tableID + "01";
			}
			private set
			{
			}
		}

		public string NoCurrentAsset
		{
			get
			{
				return _tableID + "02";
			}
			private set
			{
			}
		}

		public string CurrentLiability
		{
			get
			{
				return _tableID + "03";
			}
			private set
			{
			}
		}

		public string NoCurrentLiability
		{
			get
			{
				return _tableID + "04";
			}
			private set
			{
			}
		}

		public string Common
		{
			get
			{
				return _tableID + "05";
			}
			private set
			{
			}
		}

		public string OwnerEquity
		{
			get
			{
				return _tableID + "06";
			}
			private set
			{
			}
		}

		public string PriorYearIncomeAdjustment
		{
			get
			{
				return _tableID + "14";
			}
			private set
			{
			}
		}

		public string Cost
		{
			get
			{
				return _tableID + "07";
			}
			private set
			{
			}
		}

		public string OperatingCostsAndTaxes
		{
			get
			{
				return _tableID + "10";
			}
			private set
			{
			}
		}

		public string OtherLoss
		{
			get
			{
				return _tableID + "11";
			}
			private set
			{
			}
		}

		public string PeriodCharge
		{
			get
			{
				return _tableID + "12";
			}
			private set
			{
			}
		}

		public string IncomeTax
		{
			get
			{
				return _tableID + "13";
			}
			private set
			{
			}
		}

		public string OperatingRevenue
		{
			get
			{
				return _tableID + "08";
			}
			private set
			{
			}
		}

		public string OtherRevenue
		{
			get
			{
				return _tableID + "09";
			}
			private set
			{
			}
		}

		public AccountTypeEnum(string tableID = "1")
		{
			_tableID = tableID;
		}
	}
}
