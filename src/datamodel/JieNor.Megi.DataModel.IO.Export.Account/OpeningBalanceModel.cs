using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Account
{
	[DataContract]
	public class OpeningBalanceModel : ExportBaseModel
	{
		public string CodeTitle
		{
			get;
			set;
		}

		public string NameTitle
		{
			get;
			set;
		}

		public string GroupTitle
		{
			get;
			set;
		}

		public string TypeTitle
		{
			get;
			set;
		}

		public string DirectionTitle
		{
			get;
			set;
		}

		public string CurrencyTitle
		{
			get;
			set;
		}

		public string InitialBalanceTitle
		{
			get;
			set;
		}

		public string CumulativeDebitThisYearTitle
		{
			get;
			set;
		}

		public string CumulativeCreditThisYearTitle
		{
			get;
			set;
		}

		public bool HideCumulativeField
		{
			get;
			set;
		}

		public OpeningBalanceRowCollection OpeningBalanceRows
		{
			get;
			set;
		}
	}
}
