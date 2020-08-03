using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPTableModel : BDModel
	{
		[DataMember]
		public int MType
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
		public int MIssueStatus
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
		public string MContactTaxCode
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
		public decimal MRAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAjustAmount
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

		public FPTableModel()
			: base("T_FP_Table")
		{
		}

		public FPTableModel(string tableName)
			: base(tableName)
		{
		}
	}
}
