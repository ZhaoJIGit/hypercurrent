using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class AssetChangeAddRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Category
		{
			get;
			set;
		}

		[DataMember]
		public string Code
		{
			get;
			set;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string CheckGroup
		{
			get;
			set;
		}

		[DataMember]
		public string PurchaseDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OriginAmount
		{
			get;
			set;
		}

		[DataMember]
		public string OriginAmountStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Depreciation
		{
			get;
			set;
		}

		[DataMember]
		public string DepreciationStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? PrepareDeprecitaion
		{
			get;
			set;
		}

		[DataMember]
		public string PrepareDeprecitaionStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? NetWorth
		{
			get;
			set;
		}

		[DataMember]
		public string NetWorthStr
		{
			get;
			set;
		}
	}
}
