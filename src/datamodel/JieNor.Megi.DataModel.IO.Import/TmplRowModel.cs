using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import
{
	[DataContract]
	public class TmplRowModel
	{
		[DataMember]
		public string SheetName
		{
			get;
			set;
		}

		[DataMember]
		public int RowIndex
		{
			get;
			set;
		}

		public Dictionary<string, string> DicAddrTitleMapping
		{
			get;
			set;
		}

		[DataMember]
		public string Title
		{
			get;
			set;
		}

		[DataMember]
		public string TaxBaseTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SalaryAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TaxRateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DeductAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public List<TmplCellModel> CellList
		{
			get;
			set;
		}

		public TmplRowModel(string sheetName, int rowIndex)
		{
			SheetName = sheetName;
			RowIndex = rowIndex;
		}
	}
}
