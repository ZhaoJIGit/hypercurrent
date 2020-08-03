using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPTableViewFilterModel : SqlWhere
	{
		[DataMember]
		public string Sort
		{
			get;
			set;
		}

		[DataMember]
		public string Order
		{
			get;
			set;
		}

		[DataMember]
		public string Keyword
		{
			get;
			set;
		}

		[DataMember]
		public string NumberAmount
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MTableDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public string MIssueStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MFapiaoType
		{
			get;
			set;
		}

		[DataMember]
		public string MInvoiceType
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
		public bool NoNeedFapiaoInvoice
		{
			get;
			set;
		}
	}
}
