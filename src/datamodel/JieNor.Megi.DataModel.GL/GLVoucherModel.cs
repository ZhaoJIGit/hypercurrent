using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.MI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLVoucherModel : BizDataModel, ICloneable
	{
		[DataMember]
		[ApiMember("JournalID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MItemID
		{
			get;
			set;
		}

		public override string PKFieldName
		{
			get
			{
				return "MItemID";
			}
		}

		public override string PKFieldValue
		{
			get
			{
				return MItemID;
			}
		}

		[DataMember]
		public bool IsOld
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Date", IsLocalDate = true)]
		public DateTime MDate
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
		[ApiMember("Number", MaxLength = 30)]
		[DBField("MNumber")]
		public string MVoucherNumber
		{
			get
			{
				return base.MNumber;
			}
			set
			{
				base.MNumber = value;
			}
		}

		[DataMember]
		public int MTransferTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MTransferTypeName
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
		[ApiMember("Url")]
		public string MUrl
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherGroupNo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("NumberOfAttachment")]
		public int MAttachments
		{
			get;
			set;
		}

		[DataMember]
		public string MInternalIND
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TotalAmount", IgnoreInPost = true)]
		public decimal MDebitTotal
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
		[ApiMember("Status")]
		[ApiEnum(EnumMappingType.VoucherStatus)]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MAuditorID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MAuditDate
		{
			get;
			set;
		}

		[ApiMember("SourceBillKey", IgnoreInGet = true)]
		[DataMember]
		public string MSourceBillKey
		{
			get;
			set;
		}

		[DataMember]
		public string MCreatorName
		{
			get;
			set;
		}

		[DataMember]
		public int MSettlementStatus
		{
			get;
			set;
		}

		[DataMember]
		public decimal Percent0
		{
			get;
			set;
		}

		[DataMember]
		public decimal Percent1
		{
			get;
			set;
		}

		[DataMember]
		public decimal Percent2
		{
			get;
			set;
		}

		[DataMember]
		public string MCUID
		{
			get;
			set;
		}

		[DataMember]
		public string ErrorMessage
		{
			get;
			set;
		}

		[DataMember]
		public int ErrorCode
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCopy
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsReverse
		{
			get;
			set;
		}

		[DataMember]
		public int MDir
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
		public string MDocID
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
		public string MRVoucherID
		{
			get;
			set;
		}

		[DataMember]
		public string MOVoucherID
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryAccountPair
		{
			get;
			set;
		}

		[DataMember]
		public int MergeGroup
		{
			get;
			set;
		}

		[DataMember]
		[ModelEntry]
		[ApiDetail]
		[ApiMember("JournalLines", MemberType = ApiMemberType.ObjectList)]
		public List<GLVoucherEntryModel> MVoucherEntrys
		{
			get;
			set;
		}

		[DataMember]
		public GLPeriodTransferModel MPeriodTransfer
		{
			get;
			set;
		}

		[DataMember]
		public string[] TrackItemNameList
		{
			get;
			set;
		}

		[DataMember]
		public string PageInfo
		{
			get;
			set;
		}

		[DataMember]
		public List<MigrateLogBaseModel> MigrateLogList
		{
			get;
			set;
		}

		[DataMember]
		public MigrateConfigModel MigrateConfig
		{
			get;
			set;
		}

		[DataMember]
		public List<GLBalanceModel> BalanceList
		{
			get;
			set;
		}

		[DataMember]
		public List<GLBalanceModel> InitBalanceList
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

		[DataMember]
		public string MReference
		{
			get;
			set;
		}

		public GLVoucherModel()
			: base("T_GL_Voucher")
		{
			MVoucherEntrys = new List<GLVoucherEntryModel>();
		}

		public void AddEntry(object obj)
		{
			GLVoucherEntryModel item = obj as GLVoucherEntryModel;
			if (obj != null)
			{
				if (MVoucherEntrys == null)
				{
					MVoucherEntrys = new List<GLVoucherEntryModel>();
				}
				MVoucherEntrys.Add(item);
			}
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
