using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	[KnownType(typeof(ReportFilterBase))]
	[KnownType(typeof(BizReportRowModel))]
	public class BizReportModel
	{
		private List<BizReportRowModel> _rows = null;

		private string _headerTitle = "Summary";

		private bool _isNew = true;

		private bool _readOnly = false;

		[DataMember]
		public string ReportID
		{
			get;
			set;
		}

		[DataMember]
		public string HeaderTitle
		{
			get
			{
				return _headerTitle;
			}
			set
			{
				_headerTitle = value;
			}
		}

		[DataMember]
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
		public string LCID
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

		public bool IsNew
		{
			get
			{
				return _isNew;
			}
			set
			{
				_isNew = value;
			}
		}

		[DataMember]
		public bool IsShow
		{
			get;
			set;
		}

		[DataMember]
		public bool IsActive
		{
			get;
			set;
		}

		[DataMember]
		public ReportFilterBase Filter
		{
			get;
			set;
		}

		[DataMember]
		public List<BizReportRowModel> Rows
		{
			get
			{
				return _rows;
			}
			set
			{
				_rows = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
				if (value && _rows.Count != 0)
				{
					foreach (BizReportRowModel row in _rows)
					{
						if (row.Cells.Count != 0)
						{
							foreach (BizReportCellModel cell in row.Cells)
							{
								cell.SubReport = null;
							}
						}
					}
				}
			}
		}

		public BizReportModel()
		{
			_rows = new List<BizReportRowModel>();
		}

		public void AddRow(BizReportRowModel model)
		{
			_rows.Add(model);
		}
	}
}
