using JieNor.Megi.EntityModel.Context;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLDashboardInfoModel
	{
		[DataMember]
		public bool CanEdit
		{
			get;
			set;
		}

		[DataMember]
		public bool CanView
		{
			get;
			set;
		}

		[DataMember]
		public bool CanApprove
		{
			get;
			set;
		}

		[DataMember]
		public bool CanChangeAttachment
		{
			get;
			set;
		}

		[DataMember]
		public bool CanExport
		{
			get;
			set;
		}

		[DataMember]
		public bool CanImport
		{
			get;
			set;
		}

		[DataMember]
		public bool CanViewReport
		{
			get;
			set;
		}

		[DataMember]
		public bool CanViewFixAssets
		{
			get;
			set;
		}

		[DataMember]
		public bool CanEditFixAssets
		{
			get;
			set;
		}

		[DataMember]
		public bool CanApproveFixAssets
		{
			get;
			set;
		}

		[DataMember]
		public string FixAssetsLastFinishedPeriod
		{
			get;
			set;
		}

		[DataMember]
		public string LastFinishedPeriod
		{
			get;
			set;
		}

		[DataMember]
		public string FixAssetsDateString
		{
			get
			{
				return string.IsNullOrWhiteSpace(FixAssetsLastFinishedPeriod) ? string.Empty : ("minDate:'" + FixAssetsLastFinishedPeriod.Split(',')[0] + "',maxDate:'" + FixAssetsLastFinishedPeriod.Split(',')[1] + "'");
			}
			set
			{
			}
		}

		[DataMember]
		public string FixAssetsDisposalDateString
		{
			get
			{
				return string.IsNullOrWhiteSpace(FixAssetsLastFinishedPeriod) ? string.Empty : ("minDate:'" + FixAssetsLastFinishedPeriod.Split(',')[0] + "'");
			}
			set
			{
			}
		}

		[DataMember]
		public string FixAssetsDefaultDate
		{
			get
			{
				return string.IsNullOrWhiteSpace(FixAssetsLastFinishedPeriod) ? string.Empty : FixAssetsLastFinishedPeriod.Split(',')[2].Substring(0, FixAssetsLastFinishedPeriod.Split(',')[2].LastIndexOf('-'));
			}
			set
			{
			}
		}

		[DataMember]
		public string DateString
		{
			get
			{
				return string.IsNullOrWhiteSpace(LastFinishedPeriod) ? string.Empty : ("minDate:'" + LastFinishedPeriod.Split(',')[0] + "',maxDate:'" + LastFinishedPeriod.Split(',')[1] + "'");
			}
			set
			{
			}
		}

		[DataMember]
		public string DefaultDate
		{
			get
			{
				return string.IsNullOrWhiteSpace(LastFinishedPeriod) ? string.Empty : LastFinishedPeriod.Split(',')[2].Substring(0, LastFinishedPeriod.Split(',')[2].LastIndexOf('-'));
			}
			set
			{
			}
		}

		[DataMember]
		public string LangID
		{
			get;
			set;
		}

		[DataMember]
		public bool IsUDAS
		{
			get;
			set;
		}

		[DataMember]
		public MContext ctx
		{
			get;
			set;
		}
	}
}
