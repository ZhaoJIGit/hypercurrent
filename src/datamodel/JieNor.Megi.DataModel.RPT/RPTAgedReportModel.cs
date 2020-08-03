using JieNor.Megi.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	[KnownType(typeof(ReportFilterBase))]
	[KnownType(typeof(AgedByField))]
	[KnownType(typeof(AgedShowType))]
	[KnownType(typeof(RPTAgedRptFilterEnum))]
	public class RPTAgedReportModel
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
		public string MType
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
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCurrentAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMonthAmt1
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMonthAmt2
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMonthAmt3
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOlderAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmt
		{
			get
			{
				return MCurrentAmt + MMonthAmt1 + MMonthAmt2 + MMonthAmt3 + MOlderAmt;
			}
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

		[DataMember]
		public List<string> CurrentAmtInvoiceIDS
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MonthAmt1InvoiceIDS
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MonthAmt2InvoiceIDS
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MonthAmt3InvoiceIDS
		{
			get;
			set;
		}

		[DataMember]
		public List<string> OlderAmtInvoiceIDS
		{
			get;
			set;
		}

		[DataMember]
		public List<string> TotalAmtInvoiceIDS
		{
			get;
			set;
		}

		public RPTAgedReportModel()
		{
			CurrentAmtInvoiceIDS = new List<string>();
			MonthAmt1InvoiceIDS = new List<string>();
			MonthAmt2InvoiceIDS = new List<string>();
			MonthAmt3InvoiceIDS = new List<string>();
			OlderAmtInvoiceIDS = new List<string>();
			TotalAmtInvoiceIDS = new List<string>();
			MRowType = BizReportRowType.Item;
			MReference = "";
			MInvoiceNo = "";
		}
	}
}
