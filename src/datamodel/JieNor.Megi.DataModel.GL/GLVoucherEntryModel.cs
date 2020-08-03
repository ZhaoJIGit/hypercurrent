using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.API;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLVoucherEntryModel : BizEntryDataModel
	{
		[DataMember]
		[ApiMember("JournalLineID")]
		public string MJournalLineID
		{
			get
			{
				return base.MEntryID;
			}
			set
			{
				base.MEntryID = value;
			}
		}

		[DataMember]
		[ApiMember("Explanation")]
		public string MExplanation
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("AccountID", IgnoreInGet = true)]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("AccountCode", IgnoreLengthValidate = true)]
		public string MAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountNo
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountNameOnly
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
		[ApiMember("OriginalCurrencyAmount")]
		[ApiPrecision(2)]
		public decimal MAmountFor
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Amount")]
		[ApiPrecision(2)]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("CurrencyCode")]
		public string MCurrencyID
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
		[ApiMember("CurrencyRate")]
		[ApiPrecision(6)]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Direction")]
		[ApiEnum(EnumMappingType.AccountDirection, IsRequired = true)]
		public int MDC
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
		public string MDebitString
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditString
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebitFor
		{
			get
			{
				return (MDC == 1) ? MAmountFor : decimal.Zero;
			}
			private set
			{
			}
		}

		[DataMember]
		public decimal MCreditFor
		{
			get
			{
				return (MDC == 1) ? decimal.Zero : MAmountFor;
			}
			private set
			{
			}
		}

		[DataMember]
		[ApiMember("EntrySeq", IgnoreInGet = true)]
		public int MEntrySeq
		{
			get;
			set;
		}

		[DataMember]
		public string MSideEntrySeq
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCheckForCurrency
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCanRelateContact
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
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
		public string MCheckGroupID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("CheckGroupValueID", IgnoreInGet = true)]
		public string MCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		public DateTime MDate
		{
			get;
			set;
		}

		[DataMember]
		public int MAttachments
		{
			get;
			set;
		}

		[DataMember]
		public int MTransferTypeID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebittotal
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCreditTotal
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountNumber
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
		public int MDocType
		{
			get;
			set;
		}

		[DataMember]
		public string MDocVoucherID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("CheckGroupValueModel", IgnoreInGet = true)]
		public GLCheckGroupValueModel MCheckGroupValueModel
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
		[ApiMember("AccountingDimension")]
		public List<APIAccountDimensionModel> MAccountDimensions
		{
			get;
			set;
		}

		[DataMember]
		public int MRowIndex
		{
			get;
			set;
		}

		public bool MModified
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
		public string MEmployeeID
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemGroupID
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
		public string MTrackItem1Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5Name
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
		public string MEmployeeName
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemName
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemName
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemName
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemGroupName
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
					MContactID = (string.IsNullOrWhiteSpace(MContactID) ? CheckTypeStatusEnum.Disabled : CheckTypeStatusEnum.Required),
					MTrackItem1 = (string.IsNullOrWhiteSpace(MTrackItem1) ? CheckTypeStatusEnum.Disabled : CheckTypeStatusEnum.Optional),
					MTrackItem2 = (string.IsNullOrWhiteSpace(MTrackItem2) ? CheckTypeStatusEnum.Disabled : CheckTypeStatusEnum.Optional),
					MTrackItem3 = (string.IsNullOrWhiteSpace(MTrackItem3) ? CheckTypeStatusEnum.Disabled : CheckTypeStatusEnum.Optional),
					MTrackItem4 = (string.IsNullOrWhiteSpace(MTrackItem4) ? CheckTypeStatusEnum.Disabled : CheckTypeStatusEnum.Optional),
					MTrackItem5 = (string.IsNullOrWhiteSpace(MTrackItem5) ? CheckTypeStatusEnum.Disabled : CheckTypeStatusEnum.Optional)
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
					MContactID = (string.IsNullOrWhiteSpace(MContactID) ? null : MContactID),
					MTrackItem1 = (string.IsNullOrWhiteSpace(MTrackItem1) ? null : MTrackItem1),
					MTrackItem2 = (string.IsNullOrWhiteSpace(MTrackItem2) ? null : MTrackItem2),
					MTrackItem3 = (string.IsNullOrWhiteSpace(MTrackItem3) ? null : MTrackItem3),
					MTrackItem4 = (string.IsNullOrWhiteSpace(MTrackItem4) ? null : MTrackItem4),
					MTrackItem5 = (string.IsNullOrWhiteSpace(MTrackItem5) ? null : MTrackItem5)
				};
			}
			private set
			{
			}
		}

		[DataMember]
		public decimal MDebitForTemp
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCreditForTemp
		{
			get;
			set;
		}

		public GLVoucherEntryModel()
			: base("T_GL_VoucherEntry")
		{
		}

		public GLVoucherEntryModel(string tableName)
			: base(tableName)
		{
		}
	}
}
