using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPFapiaoModel : BizDataModel
	{
		[DataMember]
		[ApiMember("FapiaoID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MFapiaoID
		{
			get
			{
				return base.MID;
			}
			set
			{
				base.MID = value;
			}
		}

		[DataMember]
		[ApiMember("Type", IgnoreLengthValidate = true)]
		[ApiEnum(EnumMappingType.FapiaoType)]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Category", IgnoreLengthValidate = true, IgnoreInPost = true)]
		[ApiEnum(EnumMappingType.FapiaoCategory)]
		public int MInvoiceType
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCredit
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Status", IgnoreInPost = true, IgnoreLengthValidate = true)]
		[ApiEnum(EnumMappingType.FapiaoStatus)]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public bool MChangeToObsolete
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsVerifyTypeChanged
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Code")]
		public string MCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BuyerName")]
		public string MPContactName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BuyerTaxNumber")]
		public string MPContactTaxCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BuyerAddressPhone")]
		public string MPContactAddressPhone
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BuyerBankAccount")]
		public string MPContactBankInfo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SellerName")]
		public string MSContactName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SellerTaxNumber")]
		public string MSContactTaxCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SellerAddressPhone")]
		public string MSContactAddressPhone
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SellerBankAccount")]
		public string MSContactBankInfo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Source", IgnoreInGet = true)]
		public int MSource
		{
			get;
			set;
		}

		[DataMember]
		public string MImportID
		{
			get;
			set;
		}

		[DataMember]
		public string MRemark
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Date", IsLocalDate = true)]
		public DateTime MBizDate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SubTotal", IgnoreInPost = true)]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TotalTax", IgnoreInPost = true)]
		public decimal MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Total", IgnoreInPost = true)]
		public decimal MTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("VerifiedStatus", IgnoreInPost = true, IgnoreLengthValidate = true)]
		[ApiEnum(EnumMappingType.FapiaoVerifiedStatus)]
		public int MVerifyType
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("VerifiedDate", IgnoreInPost = true)]
		public DateTime MVerifyDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDeductionDate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Explanation")]
		public string MExplanation
		{
			get;
			set;
		}

		[DataMember]
		public bool MHasDetail
		{
			get;
			set;
		}

		[DataMember]
		public string MMachineCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ValidateCode")]
		public string MValidateCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Password")]
		public string MPassword
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Receiver")]
		public string MReceiver
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Reviewer")]
		public string MReaduitor
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Drawer")]
		public string MDrawer
		{
			get;
			set;
		}

		[DataMember]
		public string MItemName
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
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
		public decimal MMaxAmount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ReconcileStatus", IgnoreInGet = true)]
		public int MReconcileStatus
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("CodingStatus", IgnoreInGet = true)]
		public int MCodingStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherNumber
		{
			get;
			set;
		}

		[DataMember]
		[ModelEntry]
		[ApiDetail]
		[ApiMember("LineItems")]
		public List<FPFapiaoEntryModel> MFapiaoEntrys
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
		public FPLogModel MFapiaoLog
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public string MTitle
		{
			get;
			set;
		}

		public string GUID
		{
			get;
			set;
		}

		[ApiMember("Number")]
		[DBField("MNumber")]
		public string MFaPiaoNumber
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
		[ApiMember("ByAgency")]
		public bool MByAgency
		{
			get;
			set;
		}

		public FPFapiaoModel()
			: base("T_FP_Fapiao")
		{
		}

		public FPFapiaoModel(string tableName)
			: base(tableName)
		{
		}
	}
}
