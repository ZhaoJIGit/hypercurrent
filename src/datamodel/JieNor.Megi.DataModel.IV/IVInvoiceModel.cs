using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Const;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[AutoBillNo("MNumber", "", OperateTime.Save)]
	[DataContract]
	public class IVInvoiceModel : IVBaseModel<IVInvoiceEntryModel>
	{
		[DataMember(Order = 205)]
		[ApiMember("DueDate", IsLocalDate = true)]
		public DateTime MDueDate
		{
			get;
			set;
		}

		[DataMember(Order = 206)]
		[ApiMember("ExpectedPaymentDate", IsLocalDate = true)]
		public DateTime MExpectedDate
		{
			get;
			set;
		}

		[DataMember]
		public string MBranding
		{
			get;
			set;
		}

		[DataMember(Order = 207)]
		[ApiMember("Status")]
		[ApiEnum(EnumMappingType.InvoiceStatus)]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SentToContact")]
		public bool MIsSent
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCopyCredit
		{
			get;
			set;
		}

		[DataMember]
		public string MInvCopyID
		{
			get;
			set;
		}

		[DataMember]
		public string VerificationInforPurchase
		{
			get;
			set;
		}

		[DataMember(Order = 301)]
		[ModelEntry]
		[ApiDetail]
		[ApiMember("LineItems", MemberType = ApiMemberType.ObjectList)]
		public List<IVInvoiceEntryModel> InvoiceEntry
		{
			get
			{
				return (base.IncludeDetail.HasValue && !base.IncludeDetail.Value) ? null : base.MEntryList;
			}
			set
			{
				base.MEntryList = value;
			}
		}

		[DataMember]
		public List<IVInvoiceAttachmentModel> InvoiceAttachment
		{
			get;
			set;
		}

		[DataMember]
		public IVInvoicePermissionModel MActionPermission
		{
			get;
			set;
		}

		[DataMember]
		public string MTableID
		{
			get;
			set;
		}

		[DataMember]
		public FPTableViewModel FPTableView
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
		public int MRowIndex
		{
			get;
			set;
		}

		[DataMember]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastName
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
		public string MNextID
		{
			get;
			set;
		}

		[DataMember]
		public string MNextType
		{
			get;
			set;
		}

		[DataMember(Order = 202)]
		[ApiMember("Contact", ErrorType = ValidationErrorType.Contact, IsObjectCanUpdate = true, IsDynamicShow = true)]
		public BDContactsInfoModel MContactInfo
		{
			get;
			set;
		}

		[DataMember(Order = 201)]
		[ApiMember("Type", IgnoreLengthValidate = true)]
		[ApiEnum(EnumMappingType.InvoiceType)]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public bool ForceToGenerate
		{
			get;
			set;
		}

		public IVInvoiceModel()
			: base("T_IV_Invoice")
		{
			InvoiceAttachment = new List<IVInvoiceAttachmentModel>();
			InvoiceEntry = new List<IVInvoiceEntryModel>();
			base.Verification = new List<IVVerificationListModel>();
			MActionPermission = new IVInvoicePermissionModel();
		}
	}
}
