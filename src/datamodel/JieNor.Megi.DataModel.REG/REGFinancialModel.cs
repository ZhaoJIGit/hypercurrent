using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGFinancialModel : BDModel
	{
		[DataMember]
		public string MAppID
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
		public int MYearEndDay
		{
			get;
			set;
		}

		[DataMember]
		public int MYearEndMonth
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxPayer
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxBasisID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxNo
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxName
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxPeriodID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLockDatePeriod
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLockDateEndOfYear
		{
			get;
			set;
		}

		[DataMember]
		public string MTimeZoneID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxRegCertCopyAttachId
		{
			get;
			set;
		}

		public bool IsUpdateTaxRegCert
		{
			get;
			set;
		}

		[DataMember]
		public string MLocalTaxRegCertCopyAttachId
		{
			get;
			set;
		}

		public bool IsUpdateLocalTaxRegCert
		{
			get;
			set;
		}

		[DataMember]
		public BDAttachmentModel MTaxRegCertCopyAttachModel
		{
			get;
			set;
		}

		[DataMember]
		public BDAttachmentModel MLocalTaxRegCertCopyAttachModel
		{
			get;
			set;
		}

		[DataMember]
		public string TobeDelOriginalAttachIds
		{
			get;
			set;
		}

		[DataMember]
		public string MConversionDate
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountingStandard
		{
			get;
			set;
		}

		public REGFinancialModel()
			: base("T_REG_Financial")
		{
		}
	}
}
