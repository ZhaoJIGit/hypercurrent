using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLInitBalanceModel : BDModel
	{
		private string contactType;

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
		public string MContactTypeFromBill
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
		public decimal MInitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitDebitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitCreditBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public bool IsEmptyData
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
		public decimal MYtdDebitFor
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
		public decimal MYtdCreditFor
		{
			get;
			set;
		}

		[DataMember]
		public int MMatched
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MBankTypeName
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
		public bool MHasYtdData
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
		public int MBillCount
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

		public string OldCheckGroupValueID
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
		public GLCheckGroupValueModel MCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		public string MBillID
		{
			get;
			set;
		}

		[DataMember]
		public string MBillType
		{
			get;
			set;
		}

		public string MOrderField
		{
			get;
			set;
		}

		[DataMember]
		public string MContactType
		{
			get
			{
				if (string.IsNullOrWhiteSpace(MContactTypeFromBill))
				{
					return contactType;
				}
				return MContactTypeFromBill;
			}
			set
			{
				contactType = value;
			}
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

		[DataMember]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public string DataOrigin
		{
			get;
			set;
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

		public GLInitBalanceModel()
			: base("T_GL_INITBALANCE")
		{
		}

		public GLInitBalanceModel(string tableName)
			: base(tableName)
		{
		}
	}
}
