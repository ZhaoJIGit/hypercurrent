using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FC
{
	[DataContract]
	public class FCVoucherModuleEntryModel : BizEntryDataModel
	{
		[DataMember]
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
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public string MExplanation
		{
			get;
			set;
		}

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
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public int MEntrySeq
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

		[DataMember]
		public string MCheckGroupID
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
		public string MCheckGroupValueID
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

		public FCVoucherModuleEntryModel()
			: base("T_FC_VoucherModuleEntry")
		{
		}

		public FCVoucherModuleEntryModel(string tableName)
			: base(tableName)
		{
		}
	}
}
