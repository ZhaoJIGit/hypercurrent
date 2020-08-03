using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	[KnownType(typeof(ReportFilterBase))]
	[KnownType(typeof(AgedByField))]
	[KnownType(typeof(AgedShowType))]
	[KnownType(typeof(RPTAgedRptFilterEnum))]
	public class RPTAgeInvoiceModel
	{
		[DataMember]
		public BizReportRowType MRowType
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
		public string MInvoiceID
		{
			get;
			set;
		}

		[DataMember]
		public string MInvoiceNo
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MOldBizDate
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
		public DateTime MDueDate
		{
			get;
			set;
		}

		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string MDueDay
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

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public string MCyName
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
		public decimal MDueAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDueAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MReconciledAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MReconciledAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPaidAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MHaveVerificationAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNoVerificationAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MHaveVerificationAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNoVerificationAmt
		{
			get;
			set;
		}

		public RPTAgeInvoiceModel()
		{
			MRowType = BizReportRowType.Item;
			MReference = "";
			MInvoiceNo = "";
		}
	}
}
