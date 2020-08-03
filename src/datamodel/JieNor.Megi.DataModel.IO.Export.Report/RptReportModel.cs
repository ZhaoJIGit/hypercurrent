using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Report
{
	[DataContract]
	public class RptReportModel<T>
	{
		[DataMember]
		public string ReportID
		{
			get;
			set;
		}

		[DataMember]
		public int Type
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Header Title")]
		public string HeaderTitle
		{
			get;
			set;
		}

		[DataMember]
		[DisplayName("Header Content")]
		public string HeaderContent
		{
			get;
			set;
		}

		[DataMember]
		public string Title1
		{
			get;
			set;
		}

		[DataMember]
		public string Title2
		{
			get;
			set;
		}

		[DataMember]
		public string Title3
		{
			get;
			set;
		}

		[DataMember]
		public string Title4
		{
			get;
			set;
		}

		[DataMember]
		public string NotesTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TitleDesc
		{
			get;
			set;
		}

		[DataMember]
		public ReportCombineCollection<T> ReportCombines
		{
			get;
			set;
		}

		[DataMember]
		public ReportNoteCollection ReportNotes
		{
			get;
			set;
		}
	}
}
