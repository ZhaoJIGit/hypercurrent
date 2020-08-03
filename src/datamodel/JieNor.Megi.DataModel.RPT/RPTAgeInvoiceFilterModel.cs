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
	public class RPTAgeInvoiceFilterModel : ReportFilterBase
	{
		[DataMember]
		public RPTAgedRptFilterEnum AgedType
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
		public DateTime DateFrom
		{
			get;
			set;
		}

		[DataMember]
		public DateTime DateTo
		{
			get;
			set;
		}

		[DataMember]
		public AgedByField AgedByField
		{
			get;
			set;
		}

		[DataMember]
		public DateTime AsAt
		{
			get;
			set;
		}

		public RPTAgeInvoiceFilterModel()
		{
			MContactID = "";
		}
	}
}
