using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class SalesTaxReportRowModel : BaseReportRowModel
	{
		[DataMember]
		[DisplayName("Tax Name")]
		public string TaxName
		{
			get;
			set;
		}

		[DataMember]
		public string Rate
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Net
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Tax
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Gross
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Column6
		{
			get;
			set;
		}

		[DataMember]
		public string NetStr
		{
			get;
			set;
		}

		[DataMember]
		public string TaxStr
		{
			get;
			set;
		}

		[DataMember]
		public string GrossStr
		{
			get;
			set;
		}

		[DataMember]
		public string Column6Str
		{
			get;
			set;
		}
	}
}
