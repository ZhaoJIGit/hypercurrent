using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVGoldenTaxInvoiceListFilterModel : SqlWhere
	{
		[DataMember]
		public IVInvoiceSearchWithinEnum MSearchWithin
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public int MInvoiceSource
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
		public string Keyword
		{
			get;
			set;
		}

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
	}
}
