using JieNor.Megi.Core;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTReportConfigModel
	{
		public string MItemID
		{
			get;
			set;
		}

		public string MParentID
		{
			get;
			set;
		}

		public string MAcctTableID
		{
			get;
			set;
		}

		public string MReportType
		{
			get;
			set;
		}

		public string MName
		{
			get;
			set;
		}

		public string MNumber
		{
			get;
			set;
		}

		public string MRowType
		{
			get;
			set;
		}

		public BizReportRowType MReportRowType
		{
			get
			{
				if (string.IsNullOrEmpty(MRowType))
				{
					return BizReportRowType.Item;
				}
				int result = 0;
				if (int.TryParse(MRowType, out result))
				{
					return (BizReportRowType)result;
				}
				return BizReportRowType.Item;
			}
		}

		public string MFormula
		{
			get;
			set;
		}

		public string MRemark
		{
			get;
			set;
		}

		public int MSequence
		{
			get;
			set;
		}

		public decimal MAmount
		{
			get;
			set;
		}

		public string MTag
		{
			get;
			set;
		}
	}
}
