using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	[KnownType(typeof(BalanceBizReportRowModel))]
	public class BizReportRowModel
	{
		private BizReportRowType _rowType = BizReportRowType.Item;

		private List<BizReportCellModel> _cells = null;

		[DataMember]
		public BizReportRowType RowType
		{
			get
			{
				return _rowType;
			}
			set
			{
				_rowType = value;
			}
		}

		[DataMember]
		public string UniqueValue
		{
			get;
			set;
		}

		[DataMember]
		public List<BizReportCellModel> Cells
		{
			get
			{
				return _cells;
			}
			set
			{
				_cells = value;
			}
		}

		[DataMember]
		public List<BizReportRowModel> SubRows
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

		[DataMember]
		public decimal TotalValue
		{
			get;
			set;
		}

		public BizReportRowModel()
		{
			_cells = new List<BizReportCellModel>();
			SubRows = new List<BizReportRowModel>();
		}

		public void AddCell(BizReportCellModel model)
		{
			_cells.Add(model);
		}
	}
}
