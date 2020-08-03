using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLBalanceModel : BDModel
	{
		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
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
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public string MContactName
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginBalance
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
		public decimal MDebit
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
		public decimal MEndBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndBalance
		{
			get;
			set;
		}

		[DataMember]
		public int MAdjustPeriod
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

		[DataMember]
		public int MDC
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
		public string MAccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public string AccountTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTypeName
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public string KeyWord
		{
			get;
			set;
		}

		[DataMember]
		public string Status
		{
			get;
			set;
		}

		[DataMember]
		public BDAccountModel MAccountModel
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExcludeTransferVoucherActualAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExcludeTransferVoucherActualAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExcludeTransferVoucherYTDAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExcludeTransferVoucherYTDAmountFor
		{
			get;
			set;
		}

		public string MOrderField
		{
			get;
			set;
		}

		public GLCheckGroupModel MCheckGroup
		{
			get
			{
				return new GLCheckGroupModel
				{
					MContactID = (string.IsNullOrWhiteSpace(MContactID) ? CheckTypeStatusEnum.Disabled : CheckTypeStatusEnum.Required)
				};
			}
			private set
			{
			}
		}

		public GLCheckGroupValueModel MCheckGroupValue
		{
			get
			{
				return new GLCheckGroupValueModel
				{
					MContactID = (string.IsNullOrWhiteSpace(MContactID) ? null : MContactID)
				};
			}
			private set
			{
			}
		}

		public GLBalanceModel()
			: base("T_GL_BALANCE")
		{
		}

		public GLBalanceModel(string tableName)
			: base(tableName)
		{
		}
	}
}
