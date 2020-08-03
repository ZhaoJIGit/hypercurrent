using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	[KnownType(typeof(BizReportCellType))]
	[KnownType(typeof(BizReportCellStatuType))]
	[KnownType(typeof(BizSubRptCreateModel))]
	[KnownType(typeof(BizReportCellLinkModel))]
	[KnownType(typeof(BizReportNoteModel))]
	public class BizReportCellModel
	{
		private string _value = string.Empty;

		private BizReportCellType _celltype = BizReportCellType.Money;

		private BizReportCellStatuType _cellStatuType = BizReportCellStatuType.None;

		private List<BizReportNoteModel> _notes = null;

		[DataMember]
		public string Value
		{
			get
			{
				if (_celltype == BizReportCellType.Date || _celltype == BizReportCellType.DateTime)
				{
					if (_value.IndexOf("Date(") != -1)
					{
						return _value;
					}
					JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
					DateTime now = DateTime.Now;
					if (!DateTime.TryParse(_value, out now))
					{
						return javaScriptSerializer.Serialize(new DateTime(1900, 1, 1));
					}
					return javaScriptSerializer.Serialize(now);
				}
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		[DataMember]
		public BizReportCellType CellType
		{
			get
			{
				return _celltype;
			}
			set
			{
				_celltype = value;
			}
		}

		[DataMember]
		public BizReportCellStatuType CellStatuType
		{
			get
			{
				return _cellStatuType;
			}
			set
			{
				_cellStatuType = value;
			}
		}

		[DataMember]
		public List<BizReportNoteModel> Notes
		{
			get
			{
				return _notes;
			}
			set
			{
				_notes = value;
			}
		}

		[DataMember]
		public BizSubRptCreateModel SubReport
		{
			get;
			set;
		}

		[DataMember]
		public BizReportCellLinkModel CellLink
		{
			get;
			set;
		}

		[DataMember]
		public string Cls
		{
			get;
			set;
		}

		[DataMember]
		public string BillID
		{
			get
			{
				if (BillIDS != null && BillIDS.Count > 0)
				{
					return BillIDS[0];
				}
				return "";
			}
			set
			{
			}
		}

		[DataMember]
		public List<string> BillIDS
		{
			get;
			set;
		}

		[DataMember]
		public int ColumnSpan
		{
			get;
			set;
		}

		[DataMember]
		public int RowSpan
		{
			get;
			set;
		}

		public BizReportCellModel()
		{
			_notes = new List<BizReportNoteModel>();
			BillIDS = new List<string>();
		}

		public void AddNote(BizReportNoteModel model)
		{
			_notes.Add(model);
		}

		public void AddBillID(string billID)
		{
			BillIDS.Add(billID);
		}
	}
}
