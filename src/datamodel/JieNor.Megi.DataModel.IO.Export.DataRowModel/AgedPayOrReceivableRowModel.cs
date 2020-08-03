using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class AgedPayOrReceivableRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Contact
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Current
		{
			get;
			set;
		}

		[DataMember]
		public decimal? PreviousMonth1
		{
			get;
			set;
		}

		[DataMember]
		public decimal? PreviousMonth2
		{
			get;
			set;
		}

		[DataMember]
		public decimal? PreviousMonth3
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Older
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Total
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentStr
		{
			get;
			set;
		}

		[DataMember]
		public string PreviousMonth1Str
		{
			get;
			set;
		}

		[DataMember]
		public string PreviousMonth2Str
		{
			get;
			set;
		}

		[DataMember]
		public string PreviousMonth3Str
		{
			get;
			set;
		}

		[DataMember]
		public string OlderStr
		{
			get;
			set;
		}

		[DataMember]
		public string TotalStr
		{
			get;
			set;
		}
	}
}
