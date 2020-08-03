using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTReportModel : BizDataModel
	{
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MSubtitle
		{
			get;
			set;
		}

		[DataMember]
		public string MSheetName
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MReportDate
		{
			get;
			set;
		}

		[DataMember]
		public string MAuthor
		{
			get;
			set;
		}

		[DataMember]
		public string MContent
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsShowCoverPage
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsShowTableContent
		{
			get;
			set;
		}

		[DataMember]
		public string MPDFBorderColor
		{
			get;
			set;
		}

		[DataMember]
		public string MFooterText
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MPublishDate
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsShow
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get;
			set;
		}

		public RPTReportModel()
			: base("T_RPT_Report")
		{
		}
	}
}
