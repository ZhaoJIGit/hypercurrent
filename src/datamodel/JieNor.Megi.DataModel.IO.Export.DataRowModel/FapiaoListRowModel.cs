using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	public class FapiaoListRowModel : ExportListBaseModel
	{
		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public string MFapiaoTypeName
		{
			get;
			set;
		}

		[DataMember]
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
		public string MCode
		{
			get;
			set;
		}

		[DataMember]
		public string MPContactName
		{
			get;
			set;
		}

		[DataMember]
		public string MPContactTaxCode
		{
			get;
			set;
		}

		[DataMember]
		public string MPContactAddressPhone
		{
			get;
			set;
		}

		[DataMember]
		public string MPContactBankInfo
		{
			get;
			set;
		}

		[DataMember]
		public string MSContactName
		{
			get;
			set;
		}

		[DataMember]
		public string MSContactTaxCode
		{
			get;
			set;
		}

		[DataMember]
		public string MSContactAddressPhone
		{
			get;
			set;
		}

		[DataMember]
		public string MSContactBankInfo
		{
			get;
			set;
		}

		[DataMember]
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
		public string MBizDate
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
		public decimal MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		public int MVerifyType
		{
			get;
			set;
		}

		[DataMember]
		public string MVerifyTypeName
		{
			get;
			set;
		}

		[DataMember]
		public string MVerifyDate
		{
			get;
			set;
		}

		[DataMember]
		public string MStatusName
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
		public string MExplanation
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
		public string MItemType
		{
			get;
			set;
		}

		[DataMember]
		public string MUnit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MQuantity
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPrice
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEntryAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxPercent
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEntryTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEntryTotalAmount
		{
			get;
			set;
		}
	}
}
