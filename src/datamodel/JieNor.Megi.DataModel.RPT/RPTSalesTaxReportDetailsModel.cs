using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTSalesTaxReportDetailsModel : ReportFilterBase
	{
		[DataMember]
		public string MID
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

		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string MBankNo
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
		public string MDesc
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
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxID
		{
			get;
			set;
		}

		[DataMember]
		public string MRTTaxName
		{
			get;
			set;
		}

		[DataMember]
		public string MRTTaxRate
		{
			get;
			set;
		}

		[DataMember]
		public string MRTCompName
		{
			get;
			set;
		}

		[DataMember]
		public string MRTCompTaxRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRTGross
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRTTax
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRTNet
		{
			get;
			set;
		}
	}
}
